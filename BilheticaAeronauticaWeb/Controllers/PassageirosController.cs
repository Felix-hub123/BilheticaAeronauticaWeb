using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão dos passageiros.
    /// Inclui operações de CRUD, perfil e histórico de bilhetes.
    /// </summary>
    [Authorize]
    public class PassageirosController : Controller
    {
        private readonly IPassageiroRepository _passageiroRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;
        private readonly IBilheteRepository _bilheteRepository;

        public PassageirosController(
            IPassageiroRepository passageiroRepository,
            IUserHelper userHelper,
            IConverterHelper converterHelper,
            IImageHelper imageHelper,
            IBilheteRepository bilheteRepository)
        {
            _passageiroRepository = passageiroRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
            _bilheteRepository = bilheteRepository;
        }

        // =========================================================
        // INDEX
        // =========================================================

        [Authorize(Roles = "Admin,Funcionario")]
        public IActionResult Index()
        {
            var passageiros = _passageiroRepository
                .GetAll()
                .OrderBy(p => p.Nome)
                .ToList();

            var model = passageiros
                .Select(p => CriarPassageiroViewModel(p))
                .ToList();

            return View(model);
        }

        // =========================================================
        // DETAILS
        // =========================================================

        [Authorize(Roles = "Passageiro,Admin,Funcionario")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var passageiro =
                await _passageiroRepository
                    .GetByIdAsync(id.Value);

            if (passageiro == null)
            {
                return View("NotFound");
            }

            var userId =
                _userHelper.GetUserId(User);

            if (passageiro.UserId != userId &&
                !User.IsInRole("Admin") &&
                !User.IsInRole("Funcionario"))
            {
                return Forbid();
            }

            var model =
                CriarPassageiroViewModel(passageiro);

            return View(model);
        }

        // =========================================================
        // CREATE
        // =========================================================

        [Authorize(Roles = "Passageiro")]
        public IActionResult Create()
        {
            var userId =
                _userHelper.GetUserId(User);

            var existente =
                _passageiroRepository
                    .GetAll()
                    .FirstOrDefault(
                        p => p.UserId == userId);

            if (existente != null)
            {
                return RedirectToAction(nameof(Perfil));
            }

            return View(new PassageiroViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> Create(
            PassageiroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepararImagemViewModel(model);

                return View(model);
            }

            var userId =
                _userHelper.GetUserId(User);

            Guid imageId = Guid.Empty;

            if (model.ImageFile != null &&
                model.ImageFile.Length > 0)
            {
                imageId =
                    await _imageHelper
                        .UploadImageAsync(
                            model.ImageFile,
                            "users");
            }

            model.ImageId = imageId;

            var passageiro =
                _converterHelper.ToPassageiro(
                    model,
                    userId,
                    true);

            await _passageiroRepository
                .CreateAsync(passageiro);

            TempData["SuccessMessage"] =
                "Conta criada com sucesso!";

            return RedirectToAction(nameof(Perfil));
        }

        // =========================================================
        // CREATE FROM RESERVA
        // =========================================================

        [HttpPost]
        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> CreateFromReserva(
            [FromBody] Passageiro model)
        {
            var user =
                await _userHelper
                    .GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            model.UserId = user.Id;

            await _passageiroRepository
                .CreateAsync(model);

            return Json(new
            {
                id = model.Id,
                nome = model.Nome
            });
        }

        // =========================================================
        // PERFIL
        // =========================================================

        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> Perfil()
        {
            var passageiro =
                await ObterPassageiroAtual();

            if (passageiro == null)
            {
                return RedirectToAction(nameof(Create));
            }

            var model =
                CriarPassageiroViewModel(passageiro);

            /*
             * No perfil do próprio utilizador,
             * garantimos também que o email fica disponível,
             * mesmo que a navegação User não tenha sido carregada
             * pelo repositório.
             */
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                var user =
                    await _userHelper.GetUserAsync(User);

                if (user != null)
                {
                    model.Email = user.Email;
                }
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Passageiro")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(
            PassageiroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepararImagemViewModel(model);

                return View(model);
            }

            var passageiro =
                await _passageiroRepository
                    .GetByIdAsync(model.Id);

            if (passageiro == null)
            {
                return View("NotFound");
            }

            if (!PodeEditar(passageiro))
            {
                return Forbid();
            }

            if (model.ImageFile != null &&
                model.ImageFile.Length > 0)
            {
                if (passageiro.ImageId != Guid.Empty)
                {
                    await _imageHelper
                        .DeleteImageAsync(
                            passageiro.ImageId,
                            "users");
                }

                passageiro.ImageId =
                    await _imageHelper
                        .UploadImageAsync(
                            model.ImageFile,
                            "users");
            }

            AtualizaPassageiro(
                passageiro,
                model);

            await _passageiroRepository
                .UpdateAsync(passageiro);

            TempData["SuccessMessage"] =
                "Perfil atualizado!";

            return RedirectToAction(nameof(Perfil));
        }

        // =========================================================
        // EDIT
        // =========================================================

        [Authorize(Roles = "Passageiro,Admin,Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var passageiro =
                await _passageiroRepository
                    .GetByIdAsync(id.Value);

            if (passageiro == null)
            {
                return View("NotFound");
            }

            if (!PodeEditar(passageiro))
            {
                return Forbid();
            }

            var model =
                CriarPassageiroViewModel(passageiro);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Passageiro,Admin,Funcionario")]
        public async Task<IActionResult> Edit(
            PassageiroViewModel model)
        {
            if (!ModelState.IsValid)
            {
                PrepararImagemViewModel(model);

                return View(model);
            }

            var passageiro =
                await _passageiroRepository
                    .GetByIdAsync(model.Id);

            if (passageiro == null)
            {
                return View("NotFound");
            }

            if (!PodeEditar(passageiro))
            {
                return Forbid();
            }

            if (model.ImageFile != null &&
                model.ImageFile.Length > 0)
            {
                if (passageiro.ImageId != Guid.Empty)
                {
                    await _imageHelper
                        .DeleteImageAsync(
                            passageiro.ImageId,
                            "users");
                }

                passageiro.ImageId =
                    await _imageHelper
                        .UploadImageAsync(
                            model.ImageFile,
                            "users");
            }

            AtualizaPassageiro(
                passageiro,
                model);

            await _passageiroRepository
                .UpdateAsync(passageiro);

            TempData["SuccessMessage"] =
                "Perfil atualizado!";

            if (User.IsInRole("Passageiro"))
            {
                return RedirectToAction(nameof(Perfil));
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // HISTÓRICO
        // =========================================================

        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> Historico()
        {
            var user =
                await _userHelper
                    .GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized();
            }

            var bilhetes =
                await _bilheteRepository
                    .GetBilhetesByUserAsync(user.Id);

            return View(bilhetes);
        }

        // =========================================================
        // DELETE
        // =========================================================

        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return View("NotFound");
            }

            var passageiro =
                await _passageiroRepository
                    .GetByIdAsync(id.Value);

            if (passageiro == null)
            {
                return View("NotFound");
            }

            /*
             * Mantemos aqui a entidade Passageiro porque ainda
             * não alterámos a tua View Delete.
             */
            return View(passageiro);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var passageiro =
                await _passageiroRepository
                    .GetByIdAsync(id);

            if (passageiro == null)
            {
                return View("NotFound");
            }

            var imageId =
                passageiro.ImageId;

            await _passageiroRepository
                .DeleteAsync(passageiro);

            if (imageId != Guid.Empty)
            {
                await _imageHelper
                    .DeleteImageAsync(
                        imageId,
                        "users");
            }

            TempData["SuccessMessage"] =
                "Passageiro removido.";

            return RedirectToAction(nameof(Index));
        }

        // =========================================================
        // MÉTODOS AUXILIARES
        // =========================================================

        /// <summary>
        /// Converte a entidade Passageiro para PassageiroViewModel
        /// e prepara os dados necessários para apresentação.
        /// </summary>
        private PassageiroViewModel CriarPassageiroViewModel(
            Passageiro passageiro)
        {
            var model =
                _converterHelper
                    .ToPassageirosViewModel(passageiro);

            model.ImageUrl =
                _imageHelper.GetImageUrl(
                    passageiro.ImageId,
                    "users",
                    "users/noimage");

            model.Email =
                passageiro.User?.Email;

            return model;
        }

        /// <summary>
        /// Volta a preparar a URL da imagem quando uma View
        /// precisa de ser apresentada novamente após erro de validação.
        /// </summary>
        private void PrepararImagemViewModel(
            PassageiroViewModel model)
        {
            model.ImageUrl =
                _imageHelper.GetImageUrl(
                    model.ImageId,
                    "users",
                    "users/noimage");
        }

        /// <summary>
        /// Atualiza apenas os dados editáveis do passageiro.
        /// A imagem é tratada separadamente.
        /// </summary>
        private void AtualizaPassageiro(
            Passageiro entidade,
            PassageiroViewModel model)
        {
            entidade.Nome =
                model.Nome;

            entidade.Apelido =
                model.Apelido;

            entidade.DocumentoIdentificacao =
                model.DocumentoIdentificacao;

            entidade.NumeroDocumento =
                model.NumeroDocumento;

            entidade.DataNascimento =
                model.DataNascimento;
        }

        /// <summary>
        /// Obtém o passageiro associado ao utilizador autenticado.
        /// </summary>
        private async Task<Passageiro>
            ObterPassageiroAtual()
        {
            var user =
                await _userHelper
                    .GetUserAsync(User);

            if (user == null)
            {
                return null;
            }

            return await _passageiroRepository
                .GetByUserIdAsync(user.Id);
        }

        /// <summary>
        /// Verifica se o utilizador atual pode editar
        /// o passageiro indicado.
        /// </summary>
        private bool PodeEditar(
            Passageiro passageiro)
        {
            var userId =
                _userHelper.GetUserId(User);

            return passageiro.UserId == userId
                   || User.IsInRole("Admin")
                   || User.IsInRole("Funcionario");
        }
    }
}
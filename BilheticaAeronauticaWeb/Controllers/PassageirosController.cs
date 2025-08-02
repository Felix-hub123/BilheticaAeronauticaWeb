using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    /// <summary>
    /// Controller responsável pela gestão dos passageiros.
    /// Inclui operações de CRUD, perfil do usuário e histórico de bilhetes.
    /// Aplica regras de autorização conforme roles e propriedade dos dados.
    /// </summary>
    [Authorize(Roles = "Passageiro")]
    public class PassageirosController : Controller
    {
       
        private readonly IPassageiroRepository _passageiroRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;
        private readonly IBlobHelper _blobHelper; 
        private readonly IBilheteRepository _bilheteRepository;
        public PassageirosController(
            IPassageiroRepository passageiroRepository,
             IUserHelper userHelper,
             IConverterHelper converterHelper,
             IBlobHelper blobHelper,
             IBilheteRepository bilheteRepository
            )
        {
            _passageiroRepository = passageiroRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
            _bilheteRepository = bilheteRepository;
        }



        /// <summary>
        /// Lista todos os passageiros (apenas Admin e Funcionário).
        /// </summary>
        /// <returns>View com lista ordenada por nome dos passageiros.</returns>
        // GET: Passageiros
        [Authorize(Roles = "Admin,Funcionario")]
        public IActionResult Index()
        {
            return View(_passageiroRepository.GetAll().OrderBy(p => p.Nome));
        }



        /// <summary>
        /// Exibe detalhes do passageiro específico.
        /// Só pode ser visto pelo próprio passageiro, Admin ou Funcionário.
        /// </summary>
        /// <param name="id">ID do passageiro a consultar.</param>
        /// <returns>View com detalhes ou página de not found / forbidden 
        // GET: Passageiros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return View("NotFound");
            var passageiro = await _passageiroRepository.GetByIdAsync(id.Value);
            if (passageiro == null)
                return View("NotFound");
            var userId = _userHelper.GetUserId(User);
            if (passageiro.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Funcionario"))
                return Forbid();
            return View(passageiro);
        }



        /// <summary>
        /// Exibe formulário para criação de novo passageiro.
        /// Redireciona para perfil se já existir passageiro associado ao user.
        /// </summary>
        /// <returns>View do formulário ou redirecionamento para perfil.</returns>
        // GET: Passageiros/Create
        public IActionResult Create()
        {
            var userId = _userHelper.GetUserId(User);
            var existente = _passageiroRepository.GetAll().FirstOrDefault(p => p.UserId == userId);
            if (existente != null)
                return RedirectToAction(nameof(Perfil));
            return View();
        }



        /// <summary>
        /// Cria um novo passageiro com dados submetidos, incluindo upload de imagem.
        /// </summary>
        /// <param name="model">ViewModel com dados do passageiro.</param>
        /// <returns>Redireciona para perfil em sucesso, ou mostra formulário com erros.</returns>
        /// 
        // POST: Passageiros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PassageiroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userId = _userHelper.GetUserId(User);
            Guid imageId = Guid.Empty;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
            }
            model.ImageId = imageId;

            var passageiro = _converterHelper.ToPassageiro(model, userId, true);
            await _passageiroRepository.CreateAsync(passageiro);
            TempData["SuccessMessage"] = "Conta criada com sucesso!";
            return RedirectToAction(nameof(Perfil));
        }



        /// <summary>
        /// Cria um passageiro via API recebido no corpo da requisição.
        /// Associa o passageiro ao utilizador logado.
        /// </summary>
        /// <param name="model">Modelo Passageiro enviado na request JSON.</param>
        /// <returns>JSON com ID e nome do passageiro criado.</returns>
        /// 
        [HttpPost]
        public async Task<IActionResult> CreateFromReserva([FromBody] Passageiro model)
        {
            var user = await _userHelper.GetUserAsync(User);
            model.UserId = user.Id;
            await _passageiroRepository.CreateAsync(model);
          
            return Json(new { id = model.Id, nome = model.Nome });
        }



        /// <summary>
        /// Mostra o perfil do passageiro logado, redireciona para criação se não existir.
        /// </summary>
        /// <returns>View do perfil com dados do passageiro.</returns>
        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> Perfil()
        {
            var passageiro = await ObterPassageiroAtual();
            if (passageiro == null)
                return RedirectToAction("Create");

            var model = _converterHelper.ToPassageirosViewModel(passageiro);
            return View(model);
        }



        /// <summary>
        /// Atualiza os dados do perfil do passageiro, incluindo upload de nova imagem.
        /// </summary>
        /// <param name="model">ViewModel com dados para atualização.</param>
        /// <returns>Redireciona para perfil em sucesso ou retorna à view com erros.</returns>
        [HttpPost]
        [Authorize(Roles = "Passageiro")]
        [ValidateAntiForgeryToken]
       
        public async Task<IActionResult> Perfil(PassageiroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var passageiro = await _passageiroRepository.GetByIdAsync(model.Id);
            if (passageiro == null || !PodeEditar(passageiro))
                return Forbid();

            // Lógica explícita da imagem
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                passageiro.ImageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
            }

            // Atualizar outros dados
            AtualizaPassageiro(passageiro, model);

            await _passageiroRepository.UpdateAsync(passageiro);
            TempData["SuccessMessage"] = "Perfil atualizado!";
            return RedirectToAction(nameof(Perfil));
        }


        /// <summary>
        /// Exibe formulário para editar passageiro específico.
        /// Permite edição apenas para o próprio, Admin ou Funcionário.
        /// </summary>
        /// <param name="id">ID do passageiro para editar.</param>
        /// <returns>View do formulário ou páginas de erro.</returns>
        // EDIT (Admin ou Funcionário, ou próprio passageiro)
        [Authorize(Roles = "Passageiro,Admin,Funcionario")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return View("NotFound");

            var passageiro = await _passageiroRepository.GetByIdAsync(id.Value);
            if (passageiro == null)
                return View("NotFound");

            if (!PodeEditar(passageiro))
                return Forbid();

            var model = _converterHelper.ToPassageirosViewModel(passageiro);
            return View(model);
        }


        /// <summary>
        /// Atualiza passageiro após edição com validação e upload opcional de imagem.
        /// </summary>
        /// <param name="model">ViewModel com dados atualizados.</param>
        /// <returns>Redireciona para perfil ou retorna ao formulário com erros.</returns>

        // POST: Passageiros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PassageiroViewModel model)
        {


            if (!ModelState.IsValid)
                return View(model);

            var passageiro = await _passageiroRepository.GetByIdAsync(model.Id);
            if (passageiro == null)
                return View("NotFound");
            if (!PodeEditar(passageiro))
                return Forbid();

            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                passageiro.ImageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
            }

            AtualizaPassageiro(passageiro, model);

            await _passageiroRepository.UpdateAsync(passageiro);
            TempData["SuccessMessage"] = "Perfil atualizado!";
            return RedirectToAction(nameof(Perfil));

        }



        /// <summary>
        /// Atualiza os dados básicos do passageiro a partir do ViewModel.
        /// </summary>
        /// <param name="entidade">Entidade Passageiro a atualizar.</param>
        /// <param name="model">ViewModel com dados atualizados.</param>
        private void AtualizaPassageiro(Passageiro entidade, PassageiroViewModel model)
        {
            entidade.Nome = model.Nome;
            entidade.Apelido = model.Apelido;
            entidade.DocumentoIdentificacao = model.DocumentoIdentificacao;
            entidade.NumeroDocumento = model.NumeroDocumento;
            entidade.DataNascimento = model.DataNascimento;
            // entidade.ImageId já tratado acima na lógica de imagem
        }


        /// <summary>
        /// Obtém o passageiro associado ao utilizador autenticado atual.
        /// </summary>
        /// <returns>Entidade Passageiro ou null se não existir.</returns>
        private async Task<Passageiro> ObterPassageiroAtual()
        {
            var user = await _userHelper.GetUserAsync(User);
            return await _passageiroRepository.GetByUserIdAsync(user.Id);
        }

        private bool PodeEditar(Passageiro passagem)
        {
            var userId = _userHelper.GetUserId(User);
            return passagem.UserId == userId
                || User.IsInRole("Admin")
                || User.IsInRole("Funcionario");
        }


        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> Historico()
        {
            var user = await _userHelper.GetUserAsync(User);
            var bilhetes = await _bilheteRepository.GetBilhetesByUserAsync(user.Id);
            return View(bilhetes); 
        }


        // Só Admins/fun podem apagar passageiros
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return View("NotFound");
            var passageiro = await _passageiroRepository.GetByIdAsync(id.Value);
            if (passageiro == null)
                return View("NotFound");

            await _passageiroRepository.DeleteAsync(passageiro);
            TempData["SuccessMessage"] = "Passageiro removido.";
            return RedirectToAction(nameof(Index));
        }

        // POST: Passageiros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var passageiro = await _passageiroRepository.GetByIdAsync(id);
            await _passageiroRepository.DeleteAsync(passageiro);
            return RedirectToAction(nameof(Index));
        }

        private bool PassageiroExists(int id)
        {
            return _passageiroRepository.ExistsAsync(id).Result;
        }
    }
}

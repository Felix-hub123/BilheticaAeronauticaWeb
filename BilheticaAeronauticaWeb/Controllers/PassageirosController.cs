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

        // GET: Passageiros
        [Authorize(Roles = "Admin,Funcionario")]
        public IActionResult Index()
        {
            return View(_passageiroRepository.GetAll().OrderBy(p => p.Nome));
        }




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

        // GET: Passageiros/Create
        public IActionResult Create()
        {
            var userId = _userHelper.GetUserId(User);
            var existente = _passageiroRepository.GetAll().FirstOrDefault(p => p.UserId == userId);
            if (existente != null)
                return RedirectToAction(nameof(Perfil));
            return View();
        }

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

        [HttpPost]
        public async Task<IActionResult> CreateFromReserva([FromBody] Passageiro model)
        {
            var user = await _userHelper.GetUserAsync(User);
            model.UserId = user.Id;
            await _passageiroRepository.CreateAsync(model);
          
            return Json(new { id = model.Id, nome = model.Nome });
        }

        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> Perfil()
        {
            var passageiro = await ObterPassageiroAtual();
            if (passageiro == null)
                return RedirectToAction("Create");

            var model = _converterHelper.ToPassageirosViewModel(passageiro);
            return View(model);
        }

      


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




        private void AtualizaPassageiro(Passageiro entidade, PassageiroViewModel model)
        {
            entidade.Nome = model.Nome;
            entidade.Apelido = model.Apelido;
            entidade.DocumentoIdentificacao = model.DocumentoIdentificacao;
            entidade.NumeroDocumento = model.NumeroDocumento;
            entidade.DataNascimento = model.DataNascimento;
            // entidade.ImageId já tratado acima na lógica de imagem
        }

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

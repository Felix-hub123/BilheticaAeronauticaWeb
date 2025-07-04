using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class BilheteController : Controller
    {
        private readonly IBilheteRepository _bilheteRepository;
        private readonly IPassageiroRepository _passageiroRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly ILugarRepository _lugarRepository;
        private readonly IVooService _vooService;
        private readonly IUserHelper _userHelper;
        private readonly IBilheteService _bilheteService;
        public BilheteController(
            IBilheteRepository bilheteRepository,
            IVooRepository vooRepository,
            IPassageiroRepository passageiroRepository,
            IConverterHelper converterHelper,
            IUserHelper userHelper,
            IBilheteService bilheteService,
            ILugarRepository lugarRepository,
            IVooService vooService)
        {
            _bilheteService = bilheteService;
            _bilheteRepository = bilheteRepository;
            _passageiroRepository = passageiroRepository;
            _converterHelper = converterHelper;
            _lugarRepository = lugarRepository;
            _vooService = vooService;
            _userHelper = userHelper;
        }

       
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Carrinho()
        {
            var user = await _userHelper.GetUserAsync(User);
            var reservas = await _bilheteRepository.GetBilheteTempsByUserAsync(user.Id);
            return View(reservas);
        }

       
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> AdicionarReserva()
        {
            var user = await _userHelper.GetUserAsync(User);


            var passageiro = await _passageiroRepository.GetByUserIdAsync(user.Id);
            if (passageiro == null)
            {
               
                ModelState.AddModelError("", "Não existe passageiro associado a este utilizador.");
                return View("Erro");
            }

            var model = new BilheteViewModel
            {
                NomeCliente = user.FullName ?? user.UserName,
                PassageiroId = passageiro.Id, 
                Voos = await _bilheteService.GetVoosSelectListAsync(),
                Lugares = new List<SelectListItem>()
            };
            return View(model);

        }


        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> ComprarSelecionado(int idBilheteSelecionado)
        {
            var user = await _userHelper.GetUserAsync(User);
           
            var sucesso = await _bilheteRepository.ConfirmBilheteTempAsync(user.Id, idBilheteSelecionado);
            if (sucesso)
                return RedirectToAction("Historico");
            TempData["ErrorMessage"] = "Não foi possível confirmar o bilhete. Verifique o seu carrinho.";
            return RedirectToAction("Carrinho");
        }


        [HttpPost]
        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> AdicionarReserva(BilheteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Voos = await _bilheteService.GetVoosSelectListAsync();
                model.Lugares = await _bilheteService.GetLugaresSelectListAsync(model.VooId);
                return View(model);
            }

            var user = await _userHelper.GetUserAsync(User);
            var lugar = await _bilheteService.GetLugarByIdAsync(model.LugarId);
            var voo = await _bilheteService.GetVooByIdAsync(model.VooId);

            model.Valor = _bilheteService.CalcularPrecoBilhete(lugar, voo, model.BagagemExtra, model.Refeicao);

            var bilheteTemp = _converterHelper.ToBilheteTemp(model, user.Id);

            var result = await _bilheteRepository.AddBilheteTempAsync(bilheteTemp, user.Id);
            if (result)
                return RedirectToAction("Carrinho");

            ModelState.AddModelError("", "Já existe uma reserva para este lugar neste voo.");
            model.Voos = await _bilheteService.GetVoosSelectListAsync();
            model.Lugares = await _bilheteService.GetLugaresSelectListAsync(model.VooId);
            return View(model);
        }


        [HttpGet]
        public async Task<JsonResult> LugaresDisponiveis(int vooId)
        {
            var lugares = await _bilheteService.GetLugaresSelectListAsync(vooId);
            return Json(lugares);
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> RemoverReserva(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
                
            await _bilheteRepository.DeleteBilheteTempAsync(id.Value);
            return RedirectToAction("Carrinho");
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Comprar()
        {
            var user = await _userHelper.GetUserAsync(User);
            var sucesso = await _bilheteRepository.ConfirmBilheteAsync(user.Id);
            if (sucesso)
                return RedirectToAction("Historico");
            TempData["ErrorMessage"] = "Não foi possível confirmar os bilhetes. Verifique o seu carrinho.";
            return RedirectToAction("Carrinho");
        }

  
        [Authorize(Roles = "Cliente,Admin,Funcionario")]
        public async Task<IActionResult> Historico()
        {
            var user = await _userHelper.GetUserAsync(User);
            var bilhetes = await _bilheteRepository.GetBilhetesByUserAsync(user.Id);
            var viewModels = bilhetes.Select(b => _converterHelper.ToBilheteViewModel(b)).ToList();

            return View(viewModels);
        }

   
        public async Task<IActionResult> Detalhes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
               
            var bilhete = await _bilheteRepository.GetBilheteAsync(id.Value);
            if (bilhete == null)
            {
                return NotFound();
            }

            return View(bilhete);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Editar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

        
            var bilhete = await _bilheteRepository.GetBilheteAsync(id.Value);
            if (bilhete == null)
                return NotFound();

 
            var model = _converterHelper.ToBilheteViewModel(bilhete);
            model.Voos = await _bilheteService.GetVoosSelectListAsync();
            model.Lugares = await _bilheteService.GetLugaresSelectListAsync(model.VooId);


            return View(model);
        }

      
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Anular(int? id)
        {
            if (id == null)
                return NotFound();
            await _bilheteRepository.SoftDeleteBilheteAsync(id.Value);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<JsonResult> CalcularPreco(int vooId, int lugarId, bool bagagemExtra, bool refeicao)
        {
            var voo = await _vooService.ObterVooPorIdAsync(vooId);
            var lugar = await _bilheteService.GetLugarByIdAsync(lugarId);
            decimal preco = _bilheteService.CalcularPrecoBilhete(lugar, voo, bagagemExtra, refeicao);
            return Json(preco);
        }


    }
}

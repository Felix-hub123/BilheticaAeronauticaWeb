using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using QuestPDF.Fluent;
using Rotativa.AspNetCore;
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
            IVooService vooService
)
        {
            _bilheteService = bilheteService;
            _bilheteRepository = bilheteRepository;
            _passageiroRepository = passageiroRepository;
            _converterHelper = converterHelper;
            _lugarRepository = lugarRepository;
            _vooService = vooService;
            _userHelper = userHelper;
        }


        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Index()
        {
            var bilhetes = await _bilheteRepository.GetAllBilhetesAsync();
            return View(bilhetes);
        }

        [Authorize(Roles = "Passageiro,Admin,Funcionario")]
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


            var user = await _userHelper.GetUserAsync(User);
            var passageiro = await _passageiroRepository.GetByUserIdAsync(user.Id);
            if (bilhete.PassageiroId != passageiro.Id && !User.IsInRole("Admin") && !User.IsInRole("Funcionario"))
            {
                ViewBag.ErrorMessage = "Não tem permissão para ver este bilhete.";
                return View("Erro");
            }

            return View(bilhete);
        }


        [HttpGet("Futuros")]
        public async Task<IActionResult> GetVoosFuturos()
        {
            var user = await _userHelper.GetUserAsync(User);
           
            var bilhetesFuturos = await _bilheteRepository.GetBilhetesFuturosByUserAsync(user.Id);

            var lista = bilhetesFuturos.Select(b => new
            {
                BilheteId = b.Id,
                VooId = b.Voo.Id,
                NumeroVoo = b.Voo.Numero,
                Origem = b.Voo.Origem.Nome,
                Destino = b.Voo.Destino.Nome,
                DataPartida = b.Voo.DataHoraPartida,
                Lugar = b.Lugar.Codigo,
                Estado = b.WasDeleted ? "Anulado" : "Ativo"
            }).ToList();

            return Ok(lista);
        }


        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> Carrinho()
        {
            var user = await _userHelper.GetUserAsync(User);
            var reservas = await _bilheteRepository.GetBilheteTempsByUserAsync(user.Id);
            return View(reservas);
        }


    
        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> AdicionarReserva()
        {
            var user = await _userHelper.GetUserAsync(User);
            var passageiro = await _passageiroRepository.GetByUserIdAsync(user.Id);

            if (passageiro == null)
            {
                TempData["ErrorMessage"] = "Não existe passageiro associado a este utilizador.";
                return RedirectToAction("Index", "Home");
            }

            var voos = await _bilheteService.GetVoosSelectListAsync();
            if (voos == null || !voos.Any())
            {
                TempData["ErrorMessage"] = "Não existem voos disponíveis para reserva neste momento.";
                return RedirectToAction("Index", "Home");
            }

            var model = new BilheteViewModel
            {
                NomeCliente = user.FullName ?? user.UserName,
                PassageiroId = passageiro.Id,
                Voos = voos,
                Lugares = new List<SelectListItem>()
            };
            return View(model);
        }




        [HttpPost]
        [Authorize(Roles = "Passageiro")]
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Editar(BilheteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PreencherSelectLists(model);
                return View(model);
            }

            var bilhete = await _bilheteRepository.GetBilheteAsync(model.Id);
            if (bilhete == null)
                return NotFound();


            bilhete = _converterHelper.UpdateBilheteFromViewModel(bilhete, model);

            await _bilheteRepository.UpdateBilheteAsync(bilhete);
            return RedirectToAction("Index");
        }



        [HttpPost]
        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> AdicionarReserva(BilheteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var erros = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                ViewBag.ErrosValidacao = erros;

                await PreencherSelectLists(model);
                return View(model);
            }

            var user = await _userHelper.GetUserAsync(User);
            var lugar = await _bilheteService.GetLugarByIdAsync(model.LugarId);
            var voo = await _bilheteService.GetVooByIdAsync(model.VooId);

            var lugarOcupado = !await _bilheteService.LugarDisponivelAsync(model.VooId, model.LugarId);
            if (lugarOcupado)
            {
                ModelState.AddModelError("", "Este lugar já foi reservado por outro utilizador. Por favor escolha outro lugar.");
                await PreencherSelectLists(model);
                return View(model);
            }

            model.Valor = _bilheteService.CalcularPrecoBilhete(lugar, voo, model.BagagemExtra, model.Refeicao);

            var bilheteTemp = _converterHelper.ToBilheteTemp(model, user.Id);
            var result = await _bilheteRepository.AddBilheteTempAsync(bilheteTemp, user.Id);

            if (result)
                return RedirectToAction("Carrinho");

            ModelState.AddModelError("", "Já existe uma reserva para este lugar neste voo.");
            await PreencherSelectLists(model);
            return View(model);
        }






        [HttpGet]
        public async Task<JsonResult> LugaresDisponiveis(int vooId)
        {
            var lugares = await _bilheteService.GetLugaresSelectListAsync(vooId);
            return Json(lugares);
        }

        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> RemoverReserva(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            await _bilheteRepository.DeleteBilheteTempAsync(id.Value);
            return RedirectToAction("Carrinho");
        }



        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> Comprar()
        {
            var user = await _userHelper.GetUserAsync(User);
            var sucesso = await _bilheteRepository.ConfirmBilheteAsync(user.Id);
            if (sucesso)
                return RedirectToAction("Historico");
            TempData["ErrorMessage"] = "Não foi possível confirmar os bilhetes. Verifique o seu carrinho.";
            return RedirectToAction("Carrinho");
        }


        [Authorize(Roles = "Passageiro,Admin,Funcionario")]
        public async Task<IActionResult> Historico()
        {
            var user = await _userHelper.GetUserAsync(User);
            var bilhetes = await _bilheteRepository.GetBilhetesByUserAsync(user.Id);
            var now = DateTime.Now;

            var model = new HistoricoViewModel
            {
                Futuros = bilhetes
                    .Where(b => b.Voo.DataHoraPartida >= now)
                    .Select(b => _converterHelper.ToBilheteViewModel(b))
                    .ToList(),
                Passados = bilhetes
                    .Where(b => b.Voo.DataHoraPartida < now)
                    .Select(b => _converterHelper.ToBilheteViewModel(b))
                    .ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> DownloadPdf(int id)
        {
            var bilhete = await _bilheteRepository.GetBilheteAsync(id);
            if (bilhete == null)
                return NotFound();

            var viewModel = _converterHelper.ToBilheteViewModel(bilhete);

            var documento = new BilhetePdfDocument(viewModel);
            var pdfBytes = documento.GeneratePdf();

            return File(pdfBytes, "application/pdf", $"Bilhete_{viewModel.Id}.pdf");
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


        [Authorize(Roles = "Admin")]
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

        private async Task PreencherSelectLists(BilheteViewModel model)
        {
            model.Voos = await _bilheteService.GetVoosSelectListAsync();
            model.Lugares = await _bilheteService.GetLugaresSelectListAsync(model.VooId);
        }

        [Authorize(Roles = "Passageiro,Admin")]
        public async Task<IActionResult> Cancelar(int? id)
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



            var user = await _userHelper.GetUserAsync(User);
            var isOwner = bilhete.Passageiro?.UserId == user.Id;
            var isAdmin = User.IsInRole("Admin");
            if (!isOwner && !isAdmin)
                return Forbid();

            bilhete.WasDeleted = true;
            await _bilheteRepository.UpdateBilheteAsync(bilhete);

            TempData["SuccessMessage"] = "Bilhete anulado com sucesso.";
            return RedirectToAction("Historico");
        }

        [Authorize(Roles = "Passageiro")]
        [HttpGet]
        public async Task<IActionResult> Pagamento(int idBilhete)
        {
            var bilhete = await _bilheteRepository.GetBilheteAsync(idBilhete);
            if (bilhete == null)
                return NotFound();

            var model = new PagamentoViewModel
            {
                BilheteId = idBilhete,
                Valor = bilhete.Valor
            };
            return View(model);
        }

        [Authorize(Roles = "Passageiro")]
        [HttpPost]
        public async Task<IActionResult> Pagamento(PagamentoViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            
            bool pagamentoSucesso = model.NumeroCartao.StartsWith("4"); 

            if (!pagamentoSucesso)
            {
                ModelState.AddModelError("", "Pagamento recusado. Verifique os dados do cartão.");
                return View(model);
            }

           
            await _bilheteRepository.ConfirmarPagamentoEBilheteAsync(model.BilheteId);

            
            var bilhete = await _bilheteRepository.GetBilheteAsync(model.BilheteId);
            var passageiro = await _passageiroRepository.GetByIdAsync(bilhete.PassageiroId);

            TempData["SuccessMessage"] = "Pagamento efetuado e bilhete enviado para o seu email!";
            return RedirectToAction("Historico");


        }





    }
}

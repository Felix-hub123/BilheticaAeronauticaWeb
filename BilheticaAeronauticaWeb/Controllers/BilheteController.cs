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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{

    /// <summary>
    /// Controlador responsável pela gestão dos bilhetes.
    /// Implementa funcionalidades de consulta, histórico, reservas temporárias,
    /// compra, pagamento, cancelamento e edição de bilhetes.
    /// Aplica regras de permissão conforme roles e proprietário do bilhete.
    /// </summary>
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


        /// <summary>
        /// Lista todos os bilhetes (acesso restrito a Admin e Funcionário).
        /// </summary>

        [Authorize(Roles = "Admin,Funcionario")]
        public async Task<IActionResult> Index()
        {
            var bilhetes = await _bilheteRepository.GetAllBilhetesAsync();
            return View(bilhetes);
        }

        /// <summary>
        /// Exibe os detalhes de um aeroporto específico.
        /// </summary>
        /// <param name="id">ID do aeroporto a consultar.</param>
        /// <returns>Retorna a View com os detalhes do aeroporto se encontrado, 
        /// ou retorna uma página de erro personalizada se não encontrado.</returns>

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

        /// <summary>
        /// Retorna JSON com os voos futuros reservados pelo cliente atual.
        /// </summary>
        /// <returns>Lista JSON de voos futuros para o cliente autenticado.</returns>
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

        /// <summary>
        /// Apresenta o carrinho de reservas temporárias do utilizador autenticado.
        /// </summary>
        /// <returns>View com reservas temporárias do cliente.</returns>
        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> Carrinho()
        {
            var user = await _userHelper.GetUserAsync(User);
            var reservas = await _bilheteRepository.GetBilheteTempsByUserAsync(user.Id);
            return View(reservas);
        }


        /// <summary>
        /// Exibe o formulário para adicionar uma nova reserva.
        /// Disponível apenas para Passageiros autenticados.
        /// </summary>
        /// <returns>View com formulário e listas de voos disponíveis.</returns>
        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> AdicionarReserva()
        {
            var user =
                await _userHelper.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var passageiro =
                await _passageiroRepository
                    .GetByUserIdAsync(user.Id);

            /*
             * Se a conta existe mas ainda não possui
             * perfil Passageiro, não mandamos para Home.
             *
             * Mandamos diretamente para criação/completação
             * do perfil.
             */
            if (passageiro == null)
            {
                TempData["InfoMessage"] =
                    "Complete primeiro os seus dados de passageiro.";

                return RedirectToAction(
                    "Create",
                    "Passageiros");
            }

            var voos =
                await _bilheteService
                    .GetVoosSelectListAsync();

            if (voos == null ||
                !voos.Any())
            {
                TempData["ErrorMessage"] =
                    "Não existem voos disponíveis para reserva neste momento.";

                return RedirectToAction(
                    "Index",
                    "Passageiro");
            }

            var model =
                new BilheteViewModel
                {
                    NomeCliente =
                        user.FullName ??
                        user.UserName,

                    PassageiroId =
                        passageiro.Id,

                    Voos =
                        voos,

                    Lugares =
                        new List<SelectListItem>()
                };

            return View(model);
        }



        /// <summary>
        /// Edita um bilhete existente (apenas para Admin).
        /// </summary>
        /// <param name="model">Dados atualizados do bilhete</param>
        /// <returns>Redirect à listagem ou view com erros</returns>
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


        /// <summary>
        /// Adiciona uma reserva ao carrinho (Bilhete temporário).
        /// Valida datas e disponibilidade antes de reservar.
        /// </summary>
        /// <param name="model">Dados da reserva pretendida</param>
        /// <returns>Redirect ao carrinho ou view com erros</returns>
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

            
            if (voo == null || voo.DataHoraPartida <= DateTime.Now)
            {
                ModelState.AddModelError("", "Não é possível reservar lugar em voos que já partiram.");
                await PreencherSelectLists(model);
                return View(model);
            }

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





        /// <summary>
        /// Obtém os lugares disponíveis para determinado voo.
        /// </summary>
        /// <param name="vooId">ID do voo</param>
        /// <returns>Lista JSON de lugares disponíveis</returns>
        [HttpGet]
        public async Task<JsonResult> LugaresDisponiveis(int vooId)
        {
            var lugares = await _bilheteService.GetLugaresSelectListAsync(vooId);
            return Json(lugares);
        }


        /// <summary>
        /// Remove uma reserva temporária do carrinho.
        /// </summary>
        /// <param name="id">ID da reserva temporária</param>
        /// <returns>Redirect ao carrinho</returns>
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


        /// <summary>
        /// Finaliza a compra de todos os bilhetes presentes no carrinho.
        /// </summary>
        /// <returns>Redirect ao histórico ou carrinho com erro</returns>
        [Authorize(Roles = "Passageiro")]
        public async Task<IActionResult> Comprar()
        {
            var user = await _userHelper.GetUserAsync(User);
            var bilhetes = await _bilheteRepository.GetBilheteTempsByUserAsync(user.Id);

           
            if (bilhetes.Any(b => b.Voo.DataHoraPartida <= DateTime.Now))
            {
                TempData["ErrorMessage"] = "Um ou mais bilhetes no carrinho referem voos que já partiram. Remova-os para prosseguir.";
                return RedirectToAction("Carrinho");
            }

            var sucesso = await _bilheteRepository.ConfirmBilheteAsync(user.Id);
            if (sucesso)
                return RedirectToAction("Historico");
            TempData["ErrorMessage"] = "Não foi possível confirmar os bilhetes. Verifique o seu carrinho.";
            return RedirectToAction("Carrinho");
        }



        /// <summary>
        /// Exibe o historial completo de bilhetes do utilizador.
        /// </summary>
        /// <returns>View com histórico de viagens divididas entre futuros e passados.</returns>
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



        /// <summary>
        /// Permite download do bilhete em formato PDF.
        /// </summary>
        /// <param name="id">ID do bilhete</param>
        /// <returns>Arquivo PDF do bilhete para download</returns>
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


        /// <summary>
        /// Exibe formulário para edição de bilhete (apenas Admin).
        /// </summary>
        /// <param name="id">ID do bilhete</param>
        /// <returns>View para edição ou erro.</returns>
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

        /// <summary>
        /// Anula um bilhete através de soft delete (apenas Admin).
        /// </summary>
        /// <param name="id">ID do bilhete</param>
        /// <returns>Redirect à listagem</returns>
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Anular(int? id)
        {
            if (id == null)
                return NotFound();
            await _bilheteRepository.SoftDeleteBilheteAsync(id.Value);
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Calcula o preço do bilhete com base no voo, lugar e extras.
        /// </summary>
        /// <param name="vooId">ID do voo</param>
        /// <param name="lugarId">ID do lugar</param>
        /// <param name="bagagemExtra">Indica se há bagagem extra</param>
        /// <param name="refeicao">Indica se há refeição</param>
        /// <returns>Preço calculado em decimal JSON</returns>
        [HttpGet]
        public async Task<JsonResult> CalcularPreco(int vooId, int lugarId, bool bagagemExtra, bool refeicao)
        {
            var voo = await _vooService.ObterVooPorIdAsync(vooId);
            var lugar = await _bilheteService.GetLugarByIdAsync(lugarId);
            decimal preco = _bilheteService.CalcularPrecoBilhete(lugar, voo, bagagemExtra, refeicao);
            return Json(preco);
        }


        /// <summary>
        /// Preenche as listas de voos e lugares para utilização nas Views.
        /// </summary>
        /// <param name="model">ViewModel para preencher as coleções</param>
        /// <returns>Task assíncrona</returns>
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




        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ComprarSelecionado(int idBilheteSelecionado, string numeroTelemovel)
        {
            var user = await _userHelper.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "Usuário não autenticado.";
                return RedirectToAction("Carrinho");
            }

            // Validação do número de telemóvel (9 dígitos)
            if (string.IsNullOrWhiteSpace(numeroTelemovel) || !Regex.IsMatch(numeroTelemovel, @"^\d{9}$"))
            {
                TempData["Error"] = "Número de telemóvel inválido. Insira um número com 9 dígitos.";
                return RedirectToAction("Carrinho");
            }

            // Confirmar bilhete temporário selecionado
            bool confirmado = await _bilheteRepository.ConfirmBilheteTempAsync(user.Id, idBilheteSelecionado);
            if (!confirmado)
            {
                TempData["Error"] = "Bilhete inválido ou não encontrado.";
                return RedirectToAction("Carrinho");
            }

            // Simular espera do pagamento MB WAY, 3s de delay para parecer real
            await Task.Delay(3000);

            // Simula pagamento aprovado
            bool pago = true;
            if (!pago)
            {
                TempData["Error"] = "Falha no pagamento. Por favor, tente novamente.";
                return RedirectToAction("Carrinho");
            }

            TempData["Mensagem"] = "Pagamento com sucesso! Bilhete confirmado.";
            return RedirectToAction("CompraSucesso");
        }







        [HttpGet]
        public IActionResult CompraSucesso()
        {
            ViewBag.Mensagem = TempData["Mensagem"];
            return View();
        }

       



    }
}

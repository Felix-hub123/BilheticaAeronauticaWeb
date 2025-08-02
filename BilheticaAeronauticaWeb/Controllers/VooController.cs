using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    /// Controlador responsável pela gestão de voos.
    /// Implementa CRUD, pesquisa e validações específicas de negócio (datas, origem/destino distintas, etc).
    /// Aplica regras de acesso conforme roles (Funcionário e Admin para modificações).
    /// </summary>
    public class VooController : Controller
    {
        private readonly IVooService _vooService;
        private readonly IAeroportoRepository _aeroportoRepository;
        private readonly IAviaoRepository _aviaoRepository;
        private readonly IVooRepository _vooRepository;

        public VooController(
            IVooService vooService,
            IAeroportoRepository aeroportoRepository,
            IAviaoRepository aviaoRepository,
            IVooRepository vooRepository)
        {
            _vooService = vooService;
            _aeroportoRepository = aeroportoRepository;
            _aviaoRepository = aviaoRepository;
            _vooRepository = vooRepository;
        }



        /// <summary>
        /// Lista todos os voos disponíveis para consulta pública, ordenados por data/hora de partida.
        /// </summary>
        /// <returns>View contendo os voos disponíveis.</returns>
        /// 
        // GET: VooController
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            var voos = await _vooService.ObterVoosDisponiveisAsync();
            return View(voos.OrderBy(v => v.DataHoraPartida));
        }


        /// <summary>
        /// Mostra os detalhes do voo identificado pelo ID.
        /// Retorna NotFound caso o voo não exista ou o ID não seja informado.
        /// </summary>
        /// <param name="id">ID do voo</param>
        /// <returns>View com detalhes do voo ou NotFound.</returns>
        /// 
        // GET: VooController/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var voo = await _vooService.ObterVooPorIdAsync(id.Value);
            if (voo == null)
                return NotFound();

            return View(voo);
        }

        /// <summary>
        /// Exibe o formulário para criação de um novo voo.
        /// Apenas acessível para roles Funcionario e Admin.
        /// </summary>
        /// <returns>View com formulário para criar voo.</returns>
        /// 
        // GET: VooController/Create
        [HttpGet]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Create()
        {
            await PreencherDropDowns();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Create(VooViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PreencherDropDowns();
                return View(model);
            }
            if (model.OrigemId == model.DestinoId)
            {
                ModelState.AddModelError("", "Origem e destino não podem ser iguais.");
                await PreencherDropDowns();
                return View(model);
            }
            if (model.DataHoraChegada <= model.DataHoraPartida)
            {
                ModelState.AddModelError("", "A chegada deve ser posterior à partida.");
                await PreencherDropDowns();
                return View(model);
            }
            try
            {
                var aeroportoDestino = await _aeroportoRepository.GetByIdAsync(model.DestinoId);
                decimal taxaDestino = aeroportoDestino?.TaxaAeroportoPadrao ?? 0m;
                var voo = new Voo
                {
                    OrigemId = model.OrigemId,
                    DestinoId = model.DestinoId,
                    AviaoId = model.AviaoId,
                    DataHoraPartida = model.DataHoraPartida,
                    DataHoraChegada = model.DataHoraChegada,
                    PrecoBase = model.PrecoBase,
       
                };

                await _vooService.CriarVooAsync(voo, model.PrecoBase);

                TempData["Success"] = "Voo criado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Erro ao criar voo: {ex.Message}";
                await PreencherDropDowns();
                return View(model);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Edit(VooViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PreencherDropDowns();
                return View(model);
            }

            if (model.OrigemId == model.DestinoId)
            {
                ModelState.AddModelError("", "Origem e destino não podem ser iguais.");
                await PreencherDropDowns();
                return View(model);
            }

            var voo = await _vooService.ObterVooPorIdAsync(model.Id);
            if (voo == null)
                return NotFound();

            var (sucesso, mensagem) = await _vooService.AtualizarVooComRegrasAsync(voo, model);

            if (!sucesso)
            {
                ModelState.AddModelError("", mensagem);
                await PreencherDropDowns();
                return View(model);
            }

            TempData["Success"] = mensagem;
            return RedirectToAction(nameof(Index));
        }



        /// <summary>
        /// Elimina um voo identificado pelo ID.
        /// Aplica tratamento de exceções e retorna feedback via TempData.
        /// </summary>
        /// <param name="id">ID do voo a eliminar.</param>
        /// <returns>Redirect para lista de voos.</returns>
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var voo = await _vooRepository.GetVooWithIncludesAsync(id.Value); 

            if (voo == null)
                return NotFound();

            return View(voo);
        }



        /// <summary>
        /// Exibe formulário para pesquisar voos conforme origem, destino e data.
        /// Carrega listas de aeroportos para seleção.
        /// </summary>
        /// <returns>View com filtros e resultados (inicialmente vazios).</returns>

        [HttpGet]
        public async Task<IActionResult> Pesquisa()
        {
            var aeroportos = await _aeroportoRepository.GetAll().ToListAsync();
            var model = new PesquisaVoosViewModel
            {
                Aeroportos = aeroportos.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Nome }).ToList(),
                Resultados = new List<Voo>()
            };
            return View(model);
        }

        /// <summary>
        /// Processa pesquisa de voos segundo critérios especificados.
        /// Retorna lista dos voos que cumprem os critérios.
        /// </summary>
        /// <param name="model">ViewModel com os filtros aplicados.</param>
        /// <returns>View com lista de resultados.</returns>
        [HttpPost]
        public async Task<IActionResult> Pesquisa(PesquisaVoosViewModel model)
        {
            var aeroportos = await _aeroportoRepository.GetAll().ToListAsync();
            model.Aeroportos = aeroportos.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Nome }).ToList();

          
            model.Resultados = await _vooService.PesquisarVoosAsync(model.DataPartida, model.OrigemId, model.DestinoId);
            return View(model);
        }







        /// <summary>
        /// Confirma eliminação do voo via POST.
        /// Tratar com validação e erro silencioso retornando para lista.
        /// </summary>
        /// <param name="id">ID do voo a eliminar.</param>
        /// <returns>Redirect para lista.</returns>
        // POST: VooController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var voo = await _vooRepository.GetByIdAsync(id);

            if (voo == null)
            {
                return NotFound();
            }

            // Busca diretamente bilhetes associados ao voo
            var bilhetes = await _vooRepository.GetBilhetesByVooIdAsync(id);

            if (bilhetes != null && bilhetes.Any())
            {
                ViewBag.ErrorMessage = "Não é possível eliminar este voo porque existem bilhetes vendidos associados a ele.";
                return View("Delete", voo);
            }

            await _vooRepository.DeleteAsync(voo);

            return RedirectToAction(nameof(Index));
        }





        /// <summary>
        /// Método auxiliar para preencher ViewBags com listas de aeroportos (origem/destino) e aviões.
        /// Apresenta mensagens de erro caso não existam registros necessários para criação/edição.
        /// </summary>
        /// <returns>Task para execução assíncrona.</returns>
        private async Task PreencherDropDowns()
        {
            var aeroportos = await _aeroportoRepository.GetAll().ToListAsync();
            var avioes = await _aviaoRepository.GetAll().ToListAsync();

            if (!aeroportos.Any())
            {
                ViewBag.ErrorMessage = "Nenhum aeroporto disponível. Cadastre aeroportos antes de criar um voo.";
            }

            if (!avioes.Any())
            {
                ViewBag.ErrorMessage = (ViewBag.ErrorMessage ?? "") + " Nenhum avião disponível. Cadastre aviões antes de criar um voo.";
            }

            ViewBag.OrigemId = aeroportos.Select(a => new { Value = a.Id, Text = a.Nome }).ToList();
            ViewBag.DestinoId = aeroportos.Select(a => new { Value = a.Id, Text = a.Nome }).ToList();
            ViewBag.AviaoId = avioes.Select(a => new { Value = a.Id, Text = a.Modelo }).ToList();
        }
    }
}



using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using BilheticaAeronauticaWeb.Services;
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
    /// Controlador responsável pela gestão de voos.
    /// </summary>
    public class VooController : Controller
    {
        private readonly IVooService _vooService;
        private readonly IAeroportoRepository _aeroportoRepository;
        private readonly IAviaoRepository _aviaoRepository;
        private readonly IVooRepository _vooRepository;
        private readonly IImageHelper _imageHelper;

        public VooController(
            IVooService vooService,
            IAeroportoRepository aeroportoRepository,
            IAviaoRepository aviaoRepository,
            IVooRepository vooRepository,
            IImageHelper imageHelper)
        {
            _vooService = vooService;
            _aeroportoRepository = aeroportoRepository;
            _aviaoRepository = aviaoRepository;
            _vooRepository = vooRepository;
            _imageHelper = imageHelper;
        }

        // =========================================================
        // INDEX
        // =========================================================

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var voos = await _vooService.ObterVoosDisponiveisAsync();

            var model = voos
                .OrderBy(v => v.DataHoraPartida)
                .Select(v => new VooIndexViewModel
                {
                    Id = v.Id,

                    Numero = v.Numero,

                    OrigemNome = v.Origem?.Nome ?? string.Empty,

                    DestinoNome = v.Destino?.Nome ?? string.Empty,

                    OrigemImageUrl = v.Origem != null
                        ? _imageHelper.GetImageUrl(
                            v.Origem.ImageId,
                            "aeroportos")
                        : string.Empty,

                    DestinoImageUrl = v.Destino != null
                        ? _imageHelper.GetImageUrl(
                            v.Destino.ImageId,
                            "aeroportos")
                        : string.Empty,

                    AviaoModelo = v.Aviao?.Modelo ?? string.Empty,

                    DataHoraPartida = v.DataHoraPartida,

                    DataHoraChegada = v.DataHoraChegada
                })
                .ToList();

            return View(model);
        }

        // =========================================================
        // DETAILS
        // =========================================================

        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voo =
                await _vooService
                    .ObterVooPorIdAsync(id.Value);

            if (voo == null)
            {
                return NotFound();
            }

            return View(voo);
        }

        // =========================================================
        // CREATE - GET
        // =========================================================

        [HttpGet]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Create()
        {
            await PreencherDropDowns();

            return View();
        }

        // =========================================================
        // CREATE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Create(
            VooViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PreencherDropDowns();

                return View(model);
            }

            if (model.OrigemId == model.DestinoId)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Origem e destino não podem ser iguais.");

                await PreencherDropDowns();

                return View(model);
            }

            if (model.DataHoraChegada <=
                model.DataHoraPartida)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "A chegada deve ser posterior à partida.");

                await PreencherDropDowns();

                return View(model);
            }

            try
            {
                /*
                 * Os campos datetime-local do browser chegam
                 * ao ASP.NET como DateTimeKind.Unspecified.
                 *
                 * PostgreSQL timestamptz exige UTC.
                 *
                 * Por isso consideramos as horas introduzidas
                 * como hora de Portugal e convertemos para UTC.
                 */

                var partidaUtc =
                    ConverterHoraPortugalParaUtc(
                        model.DataHoraPartida);

                var chegadaUtc =
                    ConverterHoraPortugalParaUtc(
                        model.DataHoraChegada);

                var aeroportoDestino =
                    await _aeroportoRepository
                        .GetByIdAsync(
                            model.DestinoId);

                decimal taxaDestino =
                    aeroportoDestino?
                        .TaxaAeroportoPadrao
                    ?? 0m;

                var voo = new Voo
                {
                    OrigemId =
                        model.OrigemId,

                    DestinoId =
                        model.DestinoId,

                    AviaoId =
                        model.AviaoId,

                    DataHoraPartida =
                        partidaUtc,

                    DataHoraChegada =
                        chegadaUtc,

                    PrecoBase =
                        model.PrecoBase
                };

                await _vooService
                    .CriarVooAsync(
                        voo,
                        model.PrecoBase);

                TempData["Success"] =
                    "Voo criado com sucesso!";

                return RedirectToAction(
                    nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage =
                    $"Erro ao criar voo: {ex.Message}";

                await PreencherDropDowns();

                return View(model);
            }
        }

        // =========================================================
        // EDIT - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Edit(
            VooViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PreencherDropDowns();

                return View(model);
            }

            if (model.OrigemId == model.DestinoId)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Origem e destino não podem ser iguais.");

                await PreencherDropDowns();

                return View(model);
            }

            if (model.DataHoraChegada <=
                model.DataHoraPartida)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "A chegada deve ser posterior à partida.");

                await PreencherDropDowns();

                return View(model);
            }

            /*
             * Converter também no Edit.
             *
             * Caso contrário, o mesmo erro do PostgreSQL
             * aconteceria ao atualizar um voo.
             */
            model.DataHoraPartida =
                ConverterHoraPortugalParaUtc(
                    model.DataHoraPartida);

            model.DataHoraChegada =
                ConverterHoraPortugalParaUtc(
                    model.DataHoraChegada);

            var voo =
                await _vooService
                    .ObterVooPorIdAsync(
                        model.Id);

            if (voo == null)
            {
                return NotFound();
            }

            var (sucesso, mensagem) =
                await _vooService
                    .AtualizarVooComRegrasAsync(
                        voo,
                        model);

            if (!sucesso)
            {
                ModelState.AddModelError(
                    string.Empty,
                    mensagem);

                await PreencherDropDowns();

                return View(model);
            }

            TempData["Success"] =
                mensagem;

            return RedirectToAction(
                nameof(Index));
        }

        // =========================================================
        // DELETE - GET
        // =========================================================

        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Delete(
            int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voo =
                await _vooRepository
                    .GetVooWithIncludesAsync(
                        id.Value);

            if (voo == null)
            {
                return NotFound();
            }

            return View(voo);
        }

        // =========================================================
        // DELETE - POST
        // =========================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<ActionResult> DeleteConfirmed(
            int id)
        {
            var voo =
                await _vooRepository
                    .GetByIdAsync(id);

            if (voo == null)
            {
                return NotFound();
            }

            var bilhetes =
                await _vooRepository
                    .GetBilhetesByVooIdAsync(id);

            if (bilhetes != null &&
                bilhetes.Any())
            {
                ViewBag.ErrorMessage =
                    "Não é possível eliminar este voo porque existem bilhetes vendidos associados a ele.";

                return View(
                    "Delete",
                    voo);
            }

            await _vooRepository
                .DeleteAsync(voo);

            TempData["Success"] =
                "Voo eliminado com sucesso.";

            return RedirectToAction(
                nameof(Index));
        }

        // =========================================================
        // PESQUISA - GET
        // =========================================================

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Pesquisa()
        {
            var aeroportos =
                await _aeroportoRepository
                    .GetAll()
                    .ToListAsync();

            var model =
                new PesquisaVoosViewModel
                {
                    Aeroportos =
                        aeroportos
                            .Select(
                                a =>
                                    new SelectListItem
                                    {
                                        Value =
                                            a.Id.ToString(),

                                        Text =
                                            a.Nome
                                    })
                            .ToList(),

                    Resultados =
                        new List<Voo>()
                };

            return View(model);
        }

        // =========================================================
        // PESQUISA - POST
        // =========================================================

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Pesquisa(
            PesquisaVoosViewModel model)
        {
            var aeroportos =
                await _aeroportoRepository
                    .GetAll()
                    .ToListAsync();

            model.Aeroportos =
                aeroportos
                    .Select(
                        a =>
                            new SelectListItem
                            {
                                Value =
                                    a.Id.ToString(),

                                Text =
                                    a.Nome
                            })
                    .ToList();

            model.Resultados =
                await _vooService
                    .PesquisarVoosAsync(
                        model.DataPartida,
                        model.OrigemId,
                        model.DestinoId);

            return View(model);
        }

        // =========================================================
        // DROPDOWNS
        // =========================================================

        private async Task PreencherDropDowns()
        {
            var aeroportos =
                await _aeroportoRepository
                    .GetAll()
                    .ToListAsync();

            var avioes =
                await _aviaoRepository
                    .GetAll()
                    .ToListAsync();

            if (!aeroportos.Any())
            {
                ViewBag.ErrorMessage =
                    "Nenhum aeroporto disponível. Cadastre aeroportos antes de criar um voo.";
            }

            if (!avioes.Any())
            {
                ViewBag.ErrorMessage =
                    (ViewBag.ErrorMessage ?? "") +
                    " Nenhum avião disponível. Cadastre aviões antes de criar um voo.";
            }

            ViewBag.OrigemId =
                aeroportos
                    .Select(
                        a => new
                        {
                            Value = a.Id,
                            Text = a.Nome
                        })
                    .ToList();

            ViewBag.DestinoId =
                aeroportos
                    .Select(
                        a => new
                        {
                            Value = a.Id,
                            Text = a.Nome
                        })
                    .ToList();

            ViewBag.AviaoId =
                avioes
                    .Select(
                        a => new
                        {
                            Value = a.Id,
                            Text = a.Modelo
                        })
                    .ToList();
        }

        // =========================================================
        // DATAS / UTC
        // =========================================================

        /// <summary>
        /// Converte uma data/hora introduzida pelo utilizador
        /// em Portugal para UTC antes de guardar na base de dados.
        ///
        /// Funciona tanto no Windows como no Linux/Render.
        /// </summary>
        private DateTime ConverterHoraPortugalParaUtc(
            DateTime dataHora)
        {
            if (dataHora.Kind == DateTimeKind.Utc)
            {
                return dataHora;
            }

            /*
             * datetime-local não contém informação
             * sobre o fuso horário.
             */
            var dataSemFuso =
                DateTime.SpecifyKind(
                    dataHora,
                    DateTimeKind.Unspecified);

            /*
             * Visual Studio / Windows:
             * GMT Standard Time
             *
             * Render / Linux:
             * Europe/Lisbon
             */
            var timeZoneId =
                OperatingSystem.IsWindows()
                    ? "GMT Standard Time"
                    : "Europe/Lisbon";

            var portugalTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(
                    timeZoneId);

            return TimeZoneInfo.ConvertTimeToUtc(
                dataSemFuso,
                portugalTimeZone);
        }
    }
}



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

        // GET: VooController
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            var voos = await _vooService.ObterVoosDisponiveisAsync();
            return View(voos.OrderBy(v => v.DataHoraPartida));
        }



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


        // GET: Voo/Edit/5
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var voo = await _vooService.ObterVooPorIdAsync(id.Value);
            if (voo == null)
                return NotFound();

            var model = new VooViewModel
            {
                Id = voo.Id,
                OrigemId = voo.OrigemId,
                DestinoId = voo.DestinoId,
                AviaoId = voo.AviaoId,
                DataHoraPartida = voo.DataHoraPartida,
                DataHoraChegada = voo.DataHoraChegada,
                PrecoBase = voo.PrecoBase,
            };

            await PreencherDropDowns(); 

            return View(model); 
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

            voo.OrigemId = model.OrigemId;
            voo.DestinoId = model.DestinoId;
            voo.AviaoId = model.AviaoId;
            voo.DataHoraPartida = model.DataHoraPartida;
            voo.DataHoraChegada = model.DataHoraChegada;
            voo.PrecoBase = model.PrecoBase;

            await _vooService.EditarVooAsync(voo);

            TempData["Success"] = "Voo atualizado com sucesso!";
            return RedirectToAction(nameof(Index));
        }




        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            try
            {
                await _vooService.EliminarVooAsync(id.Value);
                TempData["Success"] = "Voo eliminado com sucesso!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Erro ao eliminar voo: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

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


        [HttpPost]
        public async Task<IActionResult> Pesquisa(PesquisaVoosViewModel model)
        {
            var aeroportos = await _aeroportoRepository.GetAll().ToListAsync();
            model.Aeroportos = aeroportos.Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Nome }).ToList();

          
            model.Resultados = await _vooService.PesquisarVoosAsync(model.DataPartida, model.OrigemId, model.DestinoId);
            return View(model);
        }








        // POST: VooController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _vooService.EliminarVooAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                
                return RedirectToAction(nameof(Index));
            }
        }


      

     
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



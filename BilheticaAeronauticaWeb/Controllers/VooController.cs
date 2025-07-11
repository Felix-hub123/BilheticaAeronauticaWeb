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

        public VooController(
            IVooService vooService,
            IAeroportoRepository aeroportoRepository,
            IAviaoRepository aviaoRepository)
        {
            _vooService = vooService;
            _aeroportoRepository = aeroportoRepository;
            _aviaoRepository = aviaoRepository;
        }

        // GET: VooController
        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            var voos = await _vooService.ObterVoosDisponiveisAsync();
            return View(voos.OrderBy(v => v.Id));
        }

        // GET: VooController/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var voo = await _vooService.ObterVooPorIdAsync(id.Value);
            if (voo == null)
            {
                return NotFound();
            }
            return View(voo);
        }

        // GET: VooController/Create
        [Authorize(Roles = "Funcionario,Admin")]
        public async  Task<ActionResult> Create()
        {
            await PreencherDropDowns();
            return View();
        }

        private async Task PreencherDropDowns()
        {
            var aeroportos = await _aeroportoRepository.GetAll().ToListAsync();
            var avioes = await _aviaoRepository.GetAll().ToListAsync();
            ViewBag.OrigemId = new SelectList(aeroportos, "Id", "Nome");
            ViewBag.DestinoId = new SelectList(aeroportos, "Id", "Nome");
            ViewBag.AviaoId = new SelectList(avioes, "Id", "Modelo");

        }

        // POST: VooController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<ActionResult> Create(Voo voo, decimal precoBaseLugar)
        {
            if (!ModelState.IsValid)
            {
                await PreencherDropDowns();
                return View(voo);
            }

            try
            {
                await _vooService.CriarVooAsync(voo, precoBaseLugar);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Erro ao criar voo.");
                await PreencherDropDowns();
                return View(voo);
            }
        }

        // GET: VooController/Edit/5
        [HttpGet]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var voo = await _vooService.ObterVooPorIdAsync(id.Value);
                if (voo == null)
                {
                    return NotFound();

                }

                await PreencherDropDowns();
                return View(voo);
            }
            catch (Exception)
            {
                return View("Error");
            }

        }

        // POST: VooController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<ActionResult> Edit(int id, Voo voo)
        {
            if (id != voo.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                await PreencherDropDowns();
                return View(voo);
            }

            try
            {
                await _vooService.EditarVooAsync(voo);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Erro ao editar voo.");
                await PreencherDropDowns();
                return View(voo);
            }
        }

        // GET: VooController/Delete/5
        [Authorize(Roles = "Funcionario,Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            try
            {
                await _vooService.EliminarVooAsync(id.Value);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                
                return RedirectToAction(nameof(Index));
            }
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
    }
}


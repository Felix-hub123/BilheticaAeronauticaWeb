using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class VooController : Controller
    {
        private readonly IVooRepository _vooRepository;
        private readonly IAeroportoRepository _aeroportoRepository;
        private readonly IAviaoRepository _aviaoRepository;

        public VooController(
            IVooRepository vooRepository,
            IAeroportoRepository aeroportoRepository,
            IAviaoRepository aviaoRepository)
        {
            _vooRepository = vooRepository;
            _aeroportoRepository = aeroportoRepository;
            _aviaoRepository = aviaoRepository;
        }

        // GET: VooController
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(_vooRepository.GetAll().OrderBy(p => p.Id));
        }

        // GET: VooController/Details/5
        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var voo = await _vooRepository.GetByIdAsync(id.Value);
            if(voo == null)
            {
                return NotFound();
            }
            return View(voo);
        }

        // GET: VooController/Create
        [Authorize(Roles = "FuncionarioOrAdmin")]
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
        [Authorize(Roles = "FuncionarioOrAdmin")]
        public async Task<ActionResult> Create(Voo voo)
        {
            if (!ModelState.IsValid)
            {
                await PreencherDropDowns();
                return View(voo);
            }

            await _vooRepository.CreateAsync(voo);
            return RedirectToAction(nameof(Index));
        }

        // GET: VooController/Edit/5
        [HttpGet]
        [Authorize(Roles = "FuncionarioOrAdmin")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voo = await _vooRepository.GetByIdAsync(id.Value);
            if (voo == null)
            {
                return NotFound();
            }

             await PreencherDropDowns(); 
            return View(voo);

        }

        // POST: VooController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "FuncionarioOrAdmin")]
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
                await _vooRepository.UpdateAsync(voo);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                await PreencherDropDowns();
                return View(voo);
            }
        }

        // GET: VooController/Delete/5
        [Authorize(Roles = "FuncionarioOrAdmin")]
        public async  Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var voo = await  _vooRepository.GetByIdAsync(id.Value);
            ;
            if (voo == null)
            {
                return NotFound();
            }

            return View(voo);
        }

        // POST: VooController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "FuncionarioOrAdmin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var voo = await _vooRepository.GetByIdAsync(id);
                if (voo == null)
                    return NotFound();

                await _vooRepository.DeleteAsync(voo);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }
    }
}

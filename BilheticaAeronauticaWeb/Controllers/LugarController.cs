using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class LugarController : Controller
    {
        private readonly ILugarRepository _lugarRepository;
        private readonly IAviaoRepository _aviaoRepository;
        private readonly IVooRepository _vooRepository;
        private readonly IConverterHelper _converterHelper;

        public LugarController(ILugarRepository lugarRepository,
            IAviaoRepository aviaoRepository,
            IVooRepository vooRepository,
            IConverterHelper converterHelper
            )
        {
            _lugarRepository = lugarRepository;
            _aviaoRepository = aviaoRepository;
            _vooRepository = vooRepository;
            _converterHelper = converterHelper;
        }
        // GET: LugarController
        public ActionResult Index()
        {
            return View(_lugarRepository.GetAll().OrderBy(l => l.Codigo));
        }

        // GET: LugarController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if( id == null)
            {
                return new NotFoundViewResult("Lugar Not Found");
            }

            var lugar = await _lugarRepository.GetByIdAsync(id.Value);
            if( lugar == null || lugar.WasDeleted )
            {
                return new NotFoundViewResult("Lugar Not Found");
            }
            return View(lugar);
        }

        // GET: LugarController/Create
        public ActionResult Create()
        {
            CarregarDropdowns();
            return View();

        }

        private void CarregarDropdowns()
        {
            ViewBag.Aviaos = _aviaoRepository.GetAll()
              .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = a.Modelo })
              .ToList();

            var voos = _vooRepository.GetAll()
            .Include(v => v.Origem)
            .Include(v => v.Destino)
            .ToList()
            .Select(v => _converterHelper.ToVooViewModel(v))
            .ToList();

            ViewBag.Voos = voos.Select(vm => new SelectListItem
            {
                Value = vm.Id.ToString(),
                Text = vm.DisplayName
            }).ToList();

        }

        // POST: LugarController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> Create(Lugar lugar)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _lugarRepository.CreateAsync(lugar);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao criar o lugar.");
            }

            CarregarDropdowns();
            return View(lugar);
        }

        // GET: LugarController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                 return new NotFoundViewResult("LugarNotFound");
            }

               
            var lugar = await _lugarRepository.GetByIdAsync(id.Value);
            if (lugar == null || lugar.WasDeleted)
            {
                 return new NotFoundViewResult("LugarNotFound");
            }
               

            CarregarDropdowns();
            return View(lugar);

        }

        // POST: LugarController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, Lugar lugar)
        {
            if (id != lugar.Id)
            {
                 return new NotFoundViewResult("LugarNotFound");
            }
               
            try
            {
                if (ModelState.IsValid)
                {
                    await _lugarRepository.UpdateAsync(lugar);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao editar o lugar.");
            }

            CarregarDropdowns();
            return View(lugar);
        }

      

        // POST: LugarController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> Delete(int id)
        {
            try
            {
                var lugar = await _lugarRepository.GetByIdAsync(id);
                if (lugar != null)
                {
                    lugar.WasDeleted = true; // Soft delete
                    await _lugarRepository.UpdateAsync(lugar);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao eliminar o lugar.");
                var lugar = await _lugarRepository.GetByIdAsync(id);
                return View(lugar);
            }
        }
    }
    
}

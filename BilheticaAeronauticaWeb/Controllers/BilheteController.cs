using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class BilheteController : Controller
    {
        private readonly IBilheteRepository _bilheteRepository;
        private readonly IVooRepository _vooRepository;
        private readonly IPassageiroRepository _passageiroRepository;
        private readonly IConverterHelper _converterHelper;
        private readonly ILugarRepository _lugarRepository;
        private readonly IUserHelper _userHelper;

        public BilheteController(
            IBilheteRepository bilheteRepository,
            IVooRepository vooRepository,
            IPassageiroRepository passageiroRepository,
            IConverterHelper converterHelper,
            IUserHelper userHelper,
            ILugarRepository lugarRepository)
        {
            _bilheteRepository = bilheteRepository;
            _vooRepository = vooRepository;
            _passageiroRepository = passageiroRepository;
            _converterHelper = converterHelper;
            _lugarRepository = lugarRepository;
            _userHelper = userHelper;
        }

        // GET: BilheteController

        public   ActionResult Index()
        {
            return View (  _bilheteRepository.GetAll().OrderBy(b => b.DataCompra));
        }

        // GET: BilheteController/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("BilheteNotFound");
            }
            var bilhete = await _bilheteRepository.GetByIdAsync(id.Value);
            if (bilhete == null)
            {
                return new NotFoundViewResult("BilheteNotFound");
            }
            return View(bilhete);
        }

        // GET: BilheteController/Create
        public async Task <ActionResult> Create()
        {
            var model = new BilheteViewModel();
            await CarregarDropdownsAsync(model);
            return View(model);

        }

        private async Task CarregarDropdownsAsync(BilheteViewModel model)
        {
            // Voos
            model.Voos = _vooRepository.GetAll()
                .ToList()
                .Select(v => _converterHelper.ToVooViewModel(v))
                .Select(vm => new SelectListItem
                {
                    Value = vm.Id.ToString(),
                    Text = vm.DisplayName
                }).ToList();

           
            var passageiros = await _userHelper.GetUsersByRoleAsync("Passageiro"); // Assuming this returns a collection of 'User'
            model.Passageiros = passageiros.Select(p => new SelectListItem
            {
                Value = p.Id,
                Text = p.Nome // or p.UserName, depending on your entity
            }).ToList();

            // Available seats
            model.Lugares = _lugarRepository.GetAll()
                .Where(l => l.Disponivel && !l.WasDeleted)
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Codigo
                }).ToList();
        }
        

        [HttpPost]        [ValidateAntiForgeryToken]        public async Task<ActionResult> Create(BilheteViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var bilhete = _converterHelper.ToBilhete(model, true);
                    await _bilheteRepository.CreateAsync(bilhete);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao criar o bilhete. Tente novamente.");
            }

            await CarregarDropdownsAsync(model);
            return View(model);
        }


        // GET: BilheteController/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new NotFoundViewResult("BilheteNotFound");
            }

            var bilhete = await _bilheteRepository.GetByIdAsync(id.Value);
            if (bilhete == null)
            {
                return new NotFoundViewResult("BilheteNotFound");
            }

            var model = _converterHelper.ToBilheteViewModel(bilhete);
            await CarregarDropdownsAsync(model);
            return View(model);
        }

        // POST: BilheteController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Edit(int id, BilheteViewModel model)
        {
            if (id != model.Id)
            {
                return new NotFoundViewResult("BilheteNotFound");
            }
             

            try
            {
                if (ModelState.IsValid)
                {
                    var bilhete = _converterHelper.ToBilhete(model, false);
                    await _bilheteRepository.UpdateAsync(bilhete);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao editar o bilhete. Tente novamente.");
            }

            await CarregarDropdownsAsync(model);
            return View(model);
        }

        // GET: BilheteController/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                 return new NotFoundViewResult("BilheteNotFound");
            }
               

            var bilhete = await _bilheteRepository.GetByIdAsync(id.Value);
            if (bilhete == null)
            {
                 return new NotFoundViewResult("BilheteNotFound");
            }
               

            return View(bilhete);
        }

        // POST: BilheteController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var bilhete = await _bilheteRepository.GetByIdAsync(id);
                if (bilhete != null)
                {
                    bilhete.WasDeleted = true; // Soft delete
                    await _bilheteRepository.UpdateAsync(bilhete);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao eliminar o bilhete.");
                var bilhete = await _bilheteRepository.GetByIdAsync(id);
                return View(bilhete);
            }
        }
    }
}

using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
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
   
    public class PassageirosController : Controller
    {
       
        private readonly IPassageiroRepository _passageiroRepository;
        private readonly IUserHelper _userHelper;
        private readonly IConverterHelper _converterHelper;

        public PassageirosController(
            IPassageiroRepository passageiroRepository,
             IUserHelper userHelper,
             IConverterHelper converterHelper
            )
        {
            _passageiroRepository = passageiroRepository;
            _userHelper = userHelper;
            _converterHelper = converterHelper;
        }

        // GET: Passageiros
        public IActionResult Index()
        {
            return View(_passageiroRepository.GetAll().OrderBy(p => p.Nome));
        }

        // GET: Passageiros/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var passageiro = await _passageiroRepository.GetByIdAsync(id.Value);
            if (passageiro == null)
            {
                return NotFound();
            }

            return View(passageiro);
        }

        // GET: Passageiros/Create
        public IActionResult Create()
        {
            
            ViewData["UserId"] = new SelectList(_passageiroRepository.GetAll(), "Id", "Id");
            return View();
        }

        // POST: Passageiros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PassageiroViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userHelper.GetUserId(User);
                var passageiro = _converterHelper.ToPassageiro(model, userId, true);
                await _passageiroRepository.CreateAsync(passageiro);
                return RedirectToAction(nameof(Index));
            }
             return View(model);
        }

        // GET: Passageiros/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var passageiro = await _passageiroRepository.GetByIdAsync(id.Value);
            if (passageiro == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToPassageirosViewModel(passageiro);
            return View(model);
        }

        // POST: Passageiros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PassageiroViewModel model)
        {
          

            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _userHelper.GetUserId(User);
                    var passageiro = _converterHelper.ToPassageiro(model, userId, false);
                    await _passageiroRepository.UpdateAsync(passageiro);
                    return RedirectToAction(nameof(Index));

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PassageiroExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
               
            }
          
            return View(model);
        }

       
        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var passageiro = await _passageiroRepository.GetByIdAsync(id.Value);
            if (passageiro == null)
            {
                return NotFound();
            }

            await _passageiroRepository.DeleteAsync(passageiro); 
            return RedirectToAction(nameof(Index));
        }

        // POST: Passageiros/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var passageiro = await _passageiroRepository.GetByIdAsync(id);
            await _passageiroRepository.DeleteAsync(passageiro);
            return RedirectToAction(nameof(Index));
        }

        private bool PassageiroExists(int id)
        {
            return _passageiroRepository.ExistsAsync(id).Result;
        }
    }
}

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
    [Authorize(Roles = "Cliente")]
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
        [Authorize(Roles = "Admin,Funcionario")]
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

            var userId = _userHelper.GetUserId(User);
            if (passageiro.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Funcionario"))
                return Forbid();

            return View(passageiro);
        }

        // GET: Passageiros/Create
        public IActionResult Create()
        {

            var userId = _userHelper.GetUserId(User);
            if (_passageiroRepository.GetAll().Any(p => p.UserId == userId))
                return RedirectToAction("Edit", new { id = _passageiroRepository.GetAll().First(p => p.UserId == userId).Id });
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

        [HttpPost]
        public async Task<IActionResult> CreateFromReserva([FromBody] Passageiro model)
        {
            var user = await _userHelper.GetUserAsync(User);
            model.UserId = user.Id;
            await _passageiroRepository.CreateAsync(model);
            // Retorna o novo passageiro para o JavaScript adicionar ao dropdown
            return Json(new { id = model.Id, nome = model.Nome });
        }

        [Authorize(Roles = "Cliente")]
        public async Task<IActionResult> Perfil()
        {
            var user = await _userHelper.GetUserAsync(User);
            var passageiro = await _passageiroRepository.GetByUserIdAsync(user.Id);

            if (passageiro == null)
            {
                // Se não existir, redireciona para criar perfil
                return RedirectToAction("Create");
            }

            var model = new PassageiroViewModel
            {
                Id = passageiro.Id,
                Nome = passageiro.Nome,
                Apelido = passageiro.Apelido,
           
            };
            return View(model); // Views/Passageiros/Perfil.cshtml
        }


        [HttpPost]
        [Authorize(Roles = "Cliente")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Perfil(PassageiroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserAsync(User);
            var passageiro = await _passageiroRepository.GetByIdAsync(model.Id);

            if (passageiro == null || passageiro.UserId != user.Id)
                return Forbid();

            // Atualiza os dados
            passageiro.Nome = model.Nome;
            passageiro.Apelido = model.Apelido;
     

            await _passageiroRepository.UpdateAsync(passageiro);

            TempData["SuccessMessage"] = "Perfil atualizado com sucesso!";
            return RedirectToAction("Perfil");
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

            var userId = _userHelper.GetUserId(User);
            if (passageiro.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Funcionario"))
                return Forbid();

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

                    // Só o dono do perfil ou um admin/funcionário pode editar
                    if (passageiro.UserId != userId && !User.IsInRole("Admin") && !User.IsInRole("Funcionario"))
                        return Forbid();

                    await _passageiroRepository.UpdateAsync(passageiro);
                    return RedirectToAction(nameof(Details), new { id = passageiro.Id });

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


        // Só admins/funcionários podem apagar passageiros
        [Authorize(Roles = "Admin,Funcionario")]
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
        [Authorize(Roles = "Admin,Funcionario")]
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

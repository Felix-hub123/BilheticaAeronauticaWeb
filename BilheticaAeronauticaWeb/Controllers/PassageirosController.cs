using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class PassageirosController : Controller
    {
        private readonly IPassageiroRepository _passageiroRepository;
        private readonly UserHelper _userHelper;

        public PassageirosController(IPassageiroRepository passageiroRepository, UserHelper userHelper)
        {
            _passageiroRepository = passageiroRepository;
            _userHelper = userHelper;

        }

        // GET: Passageiros
        public  IActionResult Index()
        {
            return View(_passageiroRepository.GetAll().OrderBy(p=> p.Nome));
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
            return View();
        }

        // POST: Passageiros/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Passageiro passageiro, string email, string password)
        {
            if (ModelState.IsValid)
            {
                // 1. Verifica se já existe um utilizador com este email
                var user = await _userHelper.GetUserByEmailAsync(email);
                if (user == null)
                {
                    user = new User
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true // ou false se quiseres confirmação por email
                    };
                    var result = await _userHelper.AddUserAsync(user, password);
                    if (result.Succeeded)
                    {
                        await _userHelper.AddUserToRoleAsync(user, "Cliente");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                            ModelState.AddModelError(string.Empty, error.Description);
                        return View(passageiro);
                    }
                }

                // 2. Associa o UserId ao Passageiro
                passageiro.UserId = user.Id;
                passageiro.DataRegisto = DateTime.UtcNow;
                passageiro.WasDeleted = false;

                await _passageiroRepository.CreateAsync(passageiro);
                return RedirectToAction(nameof(Index));
            }
            return View(passageiro);
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
            return View(passageiro);
        }

        // POST: Passageiros/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,Passageiro passageiro)
        {
            if (id != passageiro.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   await _passageiroRepository.UpdateAsync(passageiro);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _passageiroRepository.ExistsAsync(passageiro.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(passageiro);
        }

        // GET: Passageiros/Delete/5
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

            return View(passageiro);
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

     
    }
}

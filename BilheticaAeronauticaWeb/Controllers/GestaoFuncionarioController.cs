using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class GestaoFuncionarioController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IConverterHelper _converterHelper;

        public GestaoFuncionarioController(UserManager<User> userManager,
            IConverterHelper converterHelper)
        {
            _userManager = userManager;
            _converterHelper = converterHelper;
        }
        // GET: GestaoFuncionarioController
        public async  Task<ActionResult> Index()
        {
            var funcionarios = await _userManager.GetUsersInRoleAsync("Funcionario");
            var model = funcionarios.Select(f => _converterHelper.ToFuncionarioViewModel(f)).ToList();

            return View(model);
        }

        // GET: GestaoFuncionarioController/Details/5
        public async  Task<ActionResult> Details(string id)
        {
            var funcionario = await _userManager.FindByIdAsync(id);
            if (funcionario == null)
                return NotFound();

            var model = _converterHelper.ToFuncionarioViewModel(funcionario);
            return View(model);
        }

        // GET: GestaoFuncionarioController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GestaoFuncionarioController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> Create(FuncionarioViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _converterHelper.ToFuncionario(model, isNew: true);
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Funcionario");
                        return RedirectToAction(nameof(Index));
                    }
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao criar o funcionário. Tente novamente.");
            }
            return View(model);
        }

        // GET: GestaoFuncionarioController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var funcionario = await _userManager.FindByIdAsync(id);
            if (funcionario == null)
            {
                return new NotFoundViewResult("Funcionario Not Found");
            }
              
            var model = _converterHelper.ToFuncionarioViewModel(funcionario);
            return View(model);
        }

        // POST: GestaoFuncionarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(FuncionarioViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var funcionario = await _userManager.FindByIdAsync(model.Id);
                    if (funcionario == null)
                    {
                        return new NotFoundViewResult("FuncionarioNotFound");
                    }

                    _converterHelper.UpdateFuncionarioFromViewModel(funcionario, model);
                    var result = await _userManager.UpdateAsync(funcionario);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(Index));
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao editar o funcionário.");
                return View(model);
            }
        }

        // GET: GestaoFuncionarioController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("FuncionarioNotFound");
                }

                var funcionario = await _userManager.FindByIdAsync(id);
                if (funcionario == null)
                {
                    return new NotFoundViewResult("FuncionarioNotFound");
                }

                var model = _converterHelper.ToFuncionarioViewModel(funcionario);
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao carregar o funcionário para eliminação.");
                return View("Error");
            }
        }

        // POST: GestaoFuncionarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var funcionario = await _userManager.FindByIdAsync(id);
                if (funcionario != null)
                {
                    await _userManager.DeleteAsync(funcionario);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao eliminar o funcionário.");
                var funcionario = await _userManager.FindByIdAsync(id);
                var model = funcionario != null ? _converterHelper.ToFuncionarioViewModel(funcionario) : null;
                return View("Delete", model);
            }
        }
    }
}

using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class GestaoAdministradorController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IConverterHelper _converterHelper;
        private readonly IBlobHelper _blobHelper;

        public GestaoAdministradorController(
            UserManager<User> userManager,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper)
        {
            _userManager = userManager;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
        }
        // GET: GestaoAdministradorController
        public async Task<ActionResult> Index()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var model = admins.Select(a => _converterHelper.ToUserViewModel(a)).ToList();
            return View(model);
        }

        // GET: GestaoAdministradorController/Details/5
        public async Task<ActionResult> Details(string id)
        {
            var admin = await _userManager.FindByIdAsync(id);
            if (admin == null)
                return NotFound();

            var model = _converterHelper.ToUserViewModel(admin);
            return View(model);
        }

        // GET: GestaoAdministradorController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GestaoAdministradorController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async  Task<ActionResult> Create(UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = _converterHelper.ToFuncionario(model, isNew: true);

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        user.ImageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                    }

                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "Admin");
                        return RedirectToAction(nameof(Index));
                    }
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao criar o administrador. Tente novamente.");
            }
            return View(model);
        }

        // GET: GestaoAdministradorController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            var admin = await _userManager.FindByIdAsync(id);
            if (admin == null)
            {
                return new NotFoundViewResult("AdministradorNotFound");
            }

            var model = _converterHelper.ToUserViewModel(admin);
            return View(model);
        }

        // POST: GestaoAdministradorController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  async Task<ActionResult> Edit(UserViewModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var admin = await _userManager.FindByIdAsync(model.Id);
                    if (admin == null)
                    {
                        return new NotFoundViewResult("AdministradorNotFound");
                    }

                    _converterHelper.UpdateFuncionarioFromViewModel(admin, model);

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        admin.ImageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                    }
                    var result = await _userManager.UpdateAsync(admin);
                    if (result.Succeeded)
                        return RedirectToAction(nameof(Index));
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao editar o administrador.");
                return View(model);
            }
        }

        // GET: GestaoAdministradorController/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                if (id == null)
                {
                    return new NotFoundViewResult("AdministradorNotFound");
                }

                var admin = await _userManager.FindByIdAsync(id);
                if (admin == null)
                {
                    return new NotFoundViewResult("AdministradorNotFound");
                }

                var model = _converterHelper.ToUserViewModel(admin);
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao carregar o administrador para eliminação.");
                return View("Error");
            }
        }

        // POST: GestaoAdministradorController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var admin = await _userManager.FindByIdAsync(id);
                if (admin != null)
                {
                    await _userManager.DeleteAsync(admin);
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao eliminar o administrador.");
                var admin = await _userManager.FindByIdAsync(id);
                var model = admin != null ? _converterHelper.ToUserViewModel(admin) : null;
                return View("Delete", model);
            }
        }
    }
}
    


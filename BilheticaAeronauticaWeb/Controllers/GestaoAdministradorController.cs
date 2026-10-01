using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    /// <summary>
    /// Controlador para gestão dos administradores da plataforma.
    /// Permite listar, criar, editar e eliminar utilizadores com role "Admin".
    /// Usa UserManager para gestão segura das contas e IImageHelper para imagens.
    /// </summary>
    public class GestaoAdministradorController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IConverterHelper _converterHelper;
        private readonly IImageHelper _imageHelper;

        public GestaoAdministradorController(
            UserManager<User> userManager,
            IConverterHelper converterHelper,
            IImageHelper imageHelper)
        {
            _userManager = userManager;
            _converterHelper = converterHelper;
            _imageHelper = imageHelper;
        }

        /// <summary>
        /// Lista todos os administradores registados.
        /// </summary>
        public async Task<ActionResult> Index()
        {
            var admins = await _userManager.GetUsersInRoleAsync("Admin");
            var model = admins
                .Select(a => _converterHelper.ToUserViewModel(a))
                .ToList();

            return View(model);
        }

        /// <summary>
        /// Mostra os detalhes de um administrador específico.
        /// </summary>
        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return NotFound();
            }

            var admin = await _userManager.FindByIdAsync(id);

            if (admin == null)
            {
                return NotFound();
            }

            var model = _converterHelper.ToUserViewModel(admin);
            return View(model);
        }

        /// <summary>
        /// Exibe o formulário para criação de um novo administrador.
        /// </summary>
        public ActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Recebe os dados do formulário para criar um novo administrador.
        /// Faz upload da imagem e atribui o role "Admin".
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(UserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var user = _converterHelper.ToFuncionario(model, isNew: true);

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    user.ImageId = await _imageHelper.UploadImageAsync(
                        model.ImageFile,
                        "users");
                }

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Ocorreu um erro ao criar o administrador. Tente novamente.");
            }

            return View(model);
        }

        /// <summary>
        /// Exibe o formulário para editar um administrador existente.
        /// </summary>
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
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

        /// <summary>
        /// Recebe os dados para atualizar um administrador existente.
        /// Faz upload da nova imagem se fornecida.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                var admin = await _userManager.FindByIdAsync(model.Id);

                if (admin == null)
                {
                    return new NotFoundViewResult("AdministradorNotFound");
                }

                _converterHelper.UpdateFuncionarioFromViewModel(admin, model);

                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    if (admin.ImageId != Guid.Empty)
                    {
                        await _imageHelper.DeleteImageAsync(
                            admin.ImageId,
                            "users");
                    }

                    admin.ImageId = await _imageHelper.UploadImageAsync(
                        model.ImageFile,
                        "users");
                }

                var result = await _userManager.UpdateAsync(admin);

                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Ocorreu um erro ao editar o administrador.");

                return View(model);
            }
        }

        /// <summary>
        /// Exibe a confirmação para eliminar um administrador.
        /// </summary>
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
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
                ModelState.AddModelError(
                    string.Empty,
                    "Ocorreu um erro ao carregar o administrador para eliminação.");

                return View("Error");
            }
        }

        /// <summary>
        /// Remove o administrador da base de dados.
        /// Remove também a imagem associada, se existir.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var admin = await _userManager.FindByIdAsync(id);

                if (admin != null)
                {
                    if (admin.ImageId != Guid.Empty)
                    {
                        await _imageHelper.DeleteImageAsync(
                            admin.ImageId,
                            "users");
                    }

                    await _userManager.DeleteAsync(admin);
                }

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Ocorreu um erro ao eliminar o administrador.");

                var admin = await _userManager.FindByIdAsync(id);
                var model = admin != null
                    ? _converterHelper.ToUserViewModel(admin)
                    : null;

                return View("Delete", model);
            }
        }
    }
}



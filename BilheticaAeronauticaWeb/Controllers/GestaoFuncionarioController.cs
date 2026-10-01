using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    /// <summary>
    /// Controlador responsável pela gestão dos utilizadores
    /// com perfil de funcionário.
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class GestaoFuncionarioController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IConverterHelper _converterHelper;
        private readonly IUserHelper _userHelper;
        private readonly IEMailHelper _mailHelper;
        private readonly IImageHelper _imageHelper;

        public GestaoFuncionarioController(
            UserManager<User> userManager,
            IConverterHelper converterHelper,
            IUserHelper userHelper,
            IEMailHelper mailHelper,
            IImageHelper imageHelper)
        {
            _userManager = userManager;
            _converterHelper = converterHelper;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _imageHelper = imageHelper;
        }

        // =========================================================
        // INDEX
        // =========================================================

        public async Task<ActionResult> Index()
        {
            var funcionarios =
                await _userManager
                    .GetUsersInRoleAsync("Funcionario");

            var model = funcionarios
                .Select(f =>
                {
                    var vm =
                        _converterHelper
                            .ToUserViewModel(f);

                    vm.ImageUrl =
                        _imageHelper.GetImageUrl(
                            f.ImageId,
                            "users",
                            "noimage");

                    return vm;
                })
                .ToList();

            return View(model);
        }

        // =========================================================
        // DETAILS
        // =========================================================

        public async Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var funcionario =
                await _userManager
                    .FindByIdAsync(id);

            if (funcionario == null)
            {
                return new NotFoundViewResult(
                    "FuncionarioNotFound");
            }

            var model =
                _converterHelper
                    .ToUserViewModel(funcionario);

            model.ImageUrl =
                _imageHelper.GetImageUrl(
                    funcionario.ImageId,
                    "users",
                    "noimage");

            return View(model);
        }

        // =========================================================
        // CREATE
        // =========================================================

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            RegisterFuncionarioViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser =
                await _userHelper
                    .GetUserByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    "Email",
                    "Já existe um utilizador com este email.");

                return View(model);
            }

            var user = new User
            {
                Nome = model.Nome,
                Apelido = model.Apelido,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true,
                PasswordInicialDefinida = false
            };

            var tempPassword =
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 10)
                + "Aa1!";

            var result =
                await _userHelper
                    .AddUserAsync(
                        user,
                        tempPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            await _userHelper
                .AddUserToRoleAsync(
                    user,
                    "Funcionario");

            var token =
                await _userHelper
                    .GeneratePasswordResetTokenAsync(user);

            var encodedToken =
                System.Net.WebUtility
                    .UrlEncode(token);

            var resetLink =
                Url.Action(
                    "ResetPassword",
                    "Account",
                    new
                    {
                        userId = user.Id,
                        token = encodedToken,
                        email = user.Email
                    },
                    protocol: HttpContext.Request.Scheme);

            var emailBody = $@"
                <h2>Conta Criada</h2>
                Olá {user.Nome},<br/>
                Clique no link abaixo para definir a sua password:<br/>
                <a href='{resetLink}'>Definir Password</a>";

            var emailResponse =
                await _mailHelper.SendEmailAsync(
                    user.Email,
                    "Definir Password - AeroTicket",
                    emailBody);

            if (emailResponse.IsSuccess)
            {
                TempData["ShowSuccessModal"] = true;

                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(
                string.Empty,
                "Funcionário criado, mas falhou o envio de e-mail.");

            return View(model);
        }

        // =========================================================
        // EDIT
        // =========================================================

        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest();
            }

            var funcionario =
                await _userManager
                    .FindByIdAsync(id);

            if (funcionario == null)
            {
                return new NotFoundViewResult(
                    "FuncionarioNotFound");
            }

            var model =
                _converterHelper
                    .ToUserViewModel(funcionario);

            model.ImageUrl =
                _imageHelper.GetImageUrl(
                    funcionario.ImageId,
                    "users",
                    "noimage");

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(
            UserViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    model.ImageUrl =
                        _imageHelper.GetImageUrl(
                            model.ImageId,
                            "users",
                            "noimage");

                    return View(model);
                }

                var funcionario =
                    await _userManager
                        .FindByIdAsync(model.Id);

                if (funcionario == null)
                {
                    return new NotFoundViewResult(
                        "FuncionarioNotFound");
                }

                _converterHelper
                    .UpdateFuncionarioFromViewModel(
                        funcionario,
                        model);

                if (model.ImageFile != null &&
                    model.ImageFile.Length > 0)
                {
                    if (funcionario.ImageId != Guid.Empty)
                    {
                        await _imageHelper
                            .DeleteImageAsync(
                                funcionario.ImageId,
                                "users");
                    }

                    funcionario.ImageId =
                        await _imageHelper
                            .UploadImageAsync(
                                model.ImageFile,
                                "users");
                }

                var result =
                    await _userManager
                        .UpdateAsync(funcionario);

                if (result.Succeeded)
                {
                    return RedirectToAction(
                        nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                model.ImageUrl =
                    _imageHelper.GetImageUrl(
                        funcionario.ImageId,
                        "users",
                        "noimage");

                return View(model);
            }
            catch
            {
                model.ImageUrl =
                    _imageHelper.GetImageUrl(
                        model.ImageId,
                        "users",
                        "noimage");

                ModelState.AddModelError(
                    string.Empty,
                    "Ocorreu um erro ao editar o funcionário.");

                return View(model);
            }
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    return new NotFoundViewResult(
                        "FuncionarioNotFound");
                }

                var funcionario =
                    await _userManager
                        .FindByIdAsync(id);

                if (funcionario == null)
                {
                    return new NotFoundViewResult(
                        "FuncionarioNotFound");
                }

                var model =
                    _converterHelper
                        .ToUserViewModel(funcionario);

                model.ImageUrl =
                    _imageHelper.GetImageUrl(
                        funcionario.ImageId,
                        "users",
                        "noimage");

                return View(model);
            }
            catch
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Ocorreu um erro ao carregar o funcionário para eliminação.");

                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(
            string id)
        {
            try
            {
                var funcionario =
                    await _userManager
                        .FindByIdAsync(id);

                if (funcionario == null)
                {
                    return new NotFoundViewResult(
                        "FuncionarioNotFound");
                }

                var imageId =
                    funcionario.ImageId;

                var result =
                    await _userManager
                        .DeleteAsync(funcionario);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(
                            string.Empty,
                            error.Description);
                    }

                    var model =
                        _converterHelper
                            .ToUserViewModel(funcionario);

                    model.ImageUrl =
                        _imageHelper.GetImageUrl(
                            funcionario.ImageId,
                            "users",
                            "noimage");

                    return View(
                        "Delete",
                        model);
                }

                if (imageId != Guid.Empty)
                {
                    await _imageHelper
                        .DeleteImageAsync(
                            imageId,
                            "users");
                }

                return RedirectToAction(
                    nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(
                    string.Empty,
                    $"Ocorreu um erro ao eliminar o funcionário: {ex.Message}");

                var funcionario =
                    await _userManager
                        .FindByIdAsync(id);

                if (funcionario == null)
                {
                    return View(
                        "Delete",
                        null);
                }

                var model =
                    _converterHelper
                        .ToUserViewModel(funcionario);

                model.ImageUrl =
                    _imageHelper.GetImageUrl(
                        funcionario.ImageId,
                        "users",
                        "noimage");

                return View(
                    "Delete",
                    model);
            }
        }
    }
}
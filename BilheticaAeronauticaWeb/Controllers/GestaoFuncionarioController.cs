using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SuperShop.Helpers;
using SuperShop.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{

    /// <summary>
    /// Controlador responsável pela gestão dos utilizadores com perfil de funcionário.
    /// Permite listar, criar, editar e eliminar funcionários.
    /// Aplica upload de imagem e usa UserManager para gestão segura de utilizadores.
    /// </summary>
    /// 
    [Authorize(Roles = "Admin")]
    public class GestaoFuncionarioController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IConverterHelper _converterHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IUserHelper _userHelper;
        private readonly IEMailHelper _mailHelper;

        public GestaoFuncionarioController(
            UserManager<User> userManager,
            IConverterHelper converterHelper,
            IBlobHelper blobHelper,
            IUserHelper userHelper,
            IEMailHelper mailHelper)
        {
            _userManager = userManager;
            _converterHelper = converterHelper;
            _blobHelper = blobHelper;
            _userHelper = userHelper;
            _mailHelper = mailHelper;
        }


        /// <summary>
        /// Lista todos os funcionários registados no sistema.
        /// </summary>
        /// <returns>View contendo a lista de funcionários.</returns>
        // GET: GestaoFuncionarioController
        public async  Task<ActionResult> Index()
        {
            var funcionarios = await _userManager.GetUsersInRoleAsync("Funcionario");
            var model = funcionarios.Select(f => _converterHelper.ToUserViewModel(f)).ToList();

            return View(model);
        }


        /// <summary>
        /// Mostra detalhes de um funcionário específico.
        /// </summary>
        /// <param name="id">ID do funcionário a consultar.</param>
        /// <returns>View com detalhes do funcionário ou NotFound se não existir.</returns>
        // GET: GestaoFuncionarioController/Details/5
        public async  Task<ActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }
               

            var funcionario = await _userManager.FindByIdAsync(id);
            if (funcionario == null)
            {
                return new NotFoundViewResult("Funcionario Not Found");
            }


            var model = _converterHelper.ToUserViewModel(funcionario);
            return View(model);
        }




        /// <summary>
        /// Exibe o formulário para criar um novo funcionário.
        /// </summary>
        /// <returns>View para criação de funcionário.</returns>
        // GET: GestaoFuncionarioController/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RegisterFuncionarioViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userHelper.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Já existe um utilizador com este email.");
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

         
            var tempPassword = Guid.NewGuid().ToString("N").Substring(0, 10) + "Aa1!";

            var result = await _userHelper.AddUserAsync(user, tempPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            await _userHelper.AddUserToRoleAsync(user, "Funcionario");

            var token = await _userHelper.GeneratePasswordResetTokenAsync(user);
            var encodedToken = System.Net.WebUtility.UrlEncode(token);

            var resetLink = Url.Action("ResetPassword", "Account", new
            {
                userId = user.Id,
                token = encodedToken,
                email = user.Email
            }, protocol: HttpContext.Request.Scheme);

            var emailBody = $@"
             <h2>Conta Criada</h2>
                 Olá {user.Nome},<br/>
                  Clique no link abaixo para definir a sua password:<br/>
                  <a href='{resetLink}'>Definir Password</a>";

            var emailResponse = await _mailHelper.SendEmailAsync(user.Email, "Definir Password - AeroTicket", emailBody);

            if (emailResponse.IsSuccess)
            {
                TempData["ShowSuccessModal"] = true;
              
            }

            ModelState.AddModelError("", "Funcionário criado, mas falhou o envio de e-mail.");
            return View(model);
        }



        /// <summary>
        /// Exibe o formulário de edição para um funcionário existente.
        /// Retorna NotFound caso o funcionário não seja encontrado.
        /// </summary>
        /// <param name="id">ID do funcionário a editar.</param>
        /// <returns>View com os dados atuais do funcionário para edição.</returns>

        // GET: GestaoFuncionarioController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var funcionario = await _userManager.FindByIdAsync(id);
            if (funcionario == null)
            {
                return new NotFoundViewResult("Funcionario Not Found");
            }

            var model = _converterHelper.ToUserViewModel(funcionario);
            return View(model);
        }




        /// <summary>
        /// Recebe os dados do formulário para atualizar um funcionário.
        /// Atualiza imagem se nova for enviada.
        /// Exibe mensagens de erro em caso de falha.
        /// </summary>
        /// <param name="model">Dados do funcionário atualizados.</param>
        /// <returns>
        /// Redirect para Index em caso de sucesso;
        /// view do formulário com erros se falhar.
        /// </returns>
        // POST: GestaoFuncionarioController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UserViewModel model)
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

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        funcionario.ImageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                    }

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



        /// <summary>
        /// Exibe a confirmação para eliminar um funcionário.
        /// Em caso de exceção, exibe página de erro genérica.
        /// </summary>
        /// <param name="id">ID do funcionário a eliminar.</param>
        /// <returns>View de confirmação da eliminação.</returns>

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

                var model = _converterHelper.ToUserViewModel(funcionario);
                return View(model);
            }
            catch
            {
                ModelState.AddModelError("", "Ocorreu um erro ao carregar o funcionário para eliminação.");
                return View("Error");
            }
        }



        /// <summary>
        /// Processo de eliminação do funcionário após confirmação.
        /// Trata exceções e apresenta mensagens de erro caso necessário.
        /// </summary>
        /// <param name="id">ID do funcionário a eliminar.</param>
        /// <returns>Redirect para Index em caso de sucesso; view com erros em caso de falha.</returns>
        // POST: GestaoFuncionarioController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var funcionario = await _userManager.FindByIdAsync(id);
                if (funcionario == null)
                {
                    return new NotFoundViewResult("FuncionarioNotFound");
                }

                var result = await _userManager.DeleteAsync(funcionario);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    var model = _converterHelper.ToUserViewModel(funcionario);
                    return View("Delete", model);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ocorreu um erro ao eliminar o funcionário: {ex.Message}");
                var funcionario = await _userManager.FindByIdAsync(id);
                var model = funcionario != null ? _converterHelper.ToUserViewModel(funcionario) : null;
                return View("Delete", model);
            }
        }



    }
}

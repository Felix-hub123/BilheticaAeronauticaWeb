using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Model;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SuperShop.Helpers;
using SuperShop.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SuperShop.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IEMailHelper _mailHelper;
        private readonly IConfiguration _configuration;
        private readonly IImageHelper _imageHelper;
        private readonly IPassageiroRepository _passageiroRepository;

        public AccountController(
            IUserHelper userHelper,
            IEMailHelper mailHelper,
            IConfiguration configuration,
            IImageHelper imageHelper,
            IPassageiroRepository passageiroRepository)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _imageHelper = imageHelper;
            _passageiroRepository = passageiroRepository;
        }

        // =========================================================
        // LOGIN
        // =========================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity != null &&
                User.Identity.IsAuthenticated)
            {
                return RedirectToDashboard();
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result =
                await _userHelper.LoginAsync(model);

            if (result.Succeeded)
            {
                /*
                 * Fazemos uma nova requisição para que o cookie
                 * de autenticação e as roles já estejam disponíveis.
                 */
                return RedirectToAction(
                    nameof(RedirectByRole));
            }

            ModelState.AddModelError(
                string.Empty,
                "Email ou password incorretos.");

            return View(model);
        }

        // =========================================================
        // REDIRECIONAMENTO POR ROLE
        // =========================================================

        [Authorize]
        public IActionResult RedirectByRole()
        {
            return RedirectToDashboard();
        }

        /// <summary>
        /// Envia cada utilizador diretamente para
        /// a dashboard correspondente à sua role.
        /// </summary>
        private IActionResult RedirectToDashboard()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(
                    "Index",
                    "Admin");
            }

            if (User.IsInRole("Funcionario"))
            {
                return RedirectToAction(
                    "Index",
                    "Funcionarios");
            }

            if (User.IsInRole("Passageiro"))
            {
                return RedirectToAction(
                    "Index",
                    "Passageiro");
            }

            /*
             * Fallback apenas para um utilizador autenticado
             * que não tenha nenhuma das roles esperadas.
             */
            return RedirectToAction(
                "Index",
                "Home");
        }

        // =========================================================
        // LOGOUT
        // =========================================================
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();

            Response.Headers["Cache-Control"] =
                "no-cache, no-store, must-revalidate";

            Response.Headers["Pragma"] =
                "no-cache";

            Response.Headers["Expires"] =
                "0";

            return RedirectToAction(
                "Index",
                "Home");
        }

        // =========================================================
        // REGISTER
        // =========================================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity != null &&
                User.Identity.IsAuthenticated)
            {
                return RedirectToDashboard();
            }

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterNewUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userExists =
                await _userHelper
                    .GetUserByEmailAsync(
                        model.Username);

            if (userExists != null)
            {
                ModelState.AddModelError(
                    "Username",
                    "Email já está registado.");

                return View(model);
            }

            var user = new User
            {
                Nome = model.Nome,
                Apelido = model.Apelido,
                Email = model.Username,
                UserName = model.Username,
                PhoneNumber = model.PhoneNumber
            };

            var result =
                await _userHelper
                    .AddUserAsync(
                        user,
                        model.Password);

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

            // Novo utilizador público é Passageiro.
            await _userHelper
                .AddUserToRoleAsync(
                    user,
                    "Passageiro");

            // Criar Passageiro associado à conta.
            var passageiro = new Passageiro
            {
                Nome = user.Nome,
                Apelido = user.Apelido,

                DataNascimento = DateTime.SpecifyKind(
          model.DataNascimento,
          DateTimeKind.Utc),

                DocumentoIdentificacao =
          model.DocumentoIdentificacao,

                NumeroDocumento =
          model.NumeroDocumento,

                UserId = user.Id,

                DataRegisto = DateTime.UtcNow,

                WasDeleted = false
            };

            await _passageiroRepository
                .AddAsync(passageiro);

            // Confirmação de email.
            var token =
                await _userHelper
                    .GenerateEmailConfirmationTokenAsync(
                        user);

            var encodedToken =
                System.Net.WebUtility
                    .UrlEncode(token);

            var tokenLink =
                Url.Action(
                    "ConfirmEmail",
                    "Account",
                    new
                    {
                        userId = user.Id,
                        token = encodedToken
                    },
                    protocol:
                        HttpContext.Request.Scheme);

            var response =
                await _mailHelper.SendEmailAsync(
                    model.Username,
                    "Confirmação de Email",
                    $@"
                        <h1>Confirmação de Email</h1>
                        Para ativar a sua conta,
                        clique aqui:
                        <a href='{tokenLink}'>
                            Confirmar Email
                        </a>");

            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] =
                    "Utilizador criado com sucesso! Verifique o seu email para confirmar a conta.";

                return RedirectToAction(nameof(Login));
            }

            Console.WriteLine(
                $"ERRO AO ENVIAR EMAIL: {response.Message}");

            ModelState.AddModelError(
                string.Empty,
                "Não foi possível enviar o email de confirmação.");

            return View(model);
        }

        // =========================================================
        // EDIT PROFILE
        // =========================================================

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user =
                await _userHelper
                    .GetUserByEmailAsync(
                        User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            var model =
                new EditProfileViewModel
                {
                    Nome = user.Nome,
                    Apelido = user.Apelido,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    ImageId = user.ImageId
                };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(
            EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user =
                await _userHelper
                    .GetUserByEmailAsync(
                        User.Identity.Name);

            if (user == null)
            {
                return NotFound();
            }

            user.Nome =
                model.Nome;

            user.Apelido =
                model.Apelido;

            user.PhoneNumber =
                model.PhoneNumber;

            if (model.ImageFile != null &&
                model.ImageFile.Length > 0)
            {
                if (user.ImageId != Guid.Empty)
                {
                    await _imageHelper
                        .DeleteImageAsync(
                            user.ImageId,
                            "users");
                }

                user.ImageId =
                    await _imageHelper
                        .UploadImageAsync(
                            model.ImageFile,
                            "users");
            }

            var result =
                await _userHelper
                    .UpdateUserAsync(user);

            if (result.Succeeded)
            {
                ViewBag.Message =
                    "Perfil atualizado com sucesso.";

                model.ImageId =
                    user.ImageId;

                return View(model);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View(model);
        }

        // =========================================================
        // CONFIRM EMAIL
        // =========================================================

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(
            string userId,
            string token)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(token))
            {
                return NotFound();
            }

            var user =
                await _userHelper
                    .GetUserByIdAsync(userId);

            if (user == null)
            {
                ViewBag.Message =
                    "Utilizador não encontrado.";

                return View();
            }

            var decodedToken =
                System.Net.WebUtility
                    .UrlDecode(token);

            var result =
                await _userHelper
                    .ConfirmEmailAsync(
                        user,
                        decodedToken);

            if (result.Succeeded)
            {
                await _userHelper.LogoutAsync();

                TempData["SuccessMessage"] =
                    "Email confirmado com sucesso. Pode agora iniciar sessão.";

                return RedirectToAction(
                    nameof(Login));
            }

            ViewBag.Message =
                "Erro ao confirmar o email. O token pode ser inválido ou expirado.";

            return View();
        }

        // =========================================================
        // RECOVER PASSWORD
        // =========================================================

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecoverPassword(
            RecoverPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user =
                await _userHelper
                    .GetUserByEmailAsync(
                        model.Email);

            if (user != null)
            {
                var token =
                    await _userHelper
                        .GeneratePasswordResetTokenAsync(
                            user);

                var encodedToken =
                    System.Net.WebUtility
                        .UrlEncode(token);

                var link =
                    Url.Action(
                        "ResetPassword",
                        "Account",
                        new
                        {
                            userId = user.Id,
                            token = encodedToken,
                            email = user.Email
                        },
                        protocol:
                            HttpContext.Request.Scheme);

                var emailResponse =
                    await _mailHelper.SendEmailAsync(
                        model.Email,
                        "Recuperação de Password",
                        $@"
                            Para redefinir a sua password,
                            clique aqui:
                            <a href='{link}'>
                                Redefinir Password
                            </a>");

                if (!emailResponse.IsSuccess)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Falha ao enviar o email. Por favor, tente novamente mais tarde.");

                    return View(model);
                }
            }

            TempData["SuccessMessage"] =
                "As instruções foram enviadas caso o email exista no sistema.";

            return RedirectToAction(
                nameof(RecoverPassword));
        }

        // =========================================================
        // RESET PASSWORD
        // =========================================================

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(
            string userId,
            string token,
            string email)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(token) ||
                string.IsNullOrEmpty(email))
            {
                return RedirectToAction(
                    nameof(Login));
            }

            var model =
                new ResetPasswordViewModel
                {
                    UserId = userId,
                    Token = token,
                    Email = email
                };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(
            ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user =
                await _userHelper
                    .GetUserByEmailAsync(
                        model.Email);

            if (user == null ||
                user.Id != model.UserId)
            {
                return View(
                    "ResetPasswordConfirmation");
            }

            var decodedToken =
                System.Net.WebUtility
                    .UrlDecode(model.Token);

            var result =
                await _userHelper
                    .ResetPasswordAsync(
                        user,
                        decodedToken,
                        model.NewPassword);

            if (result.Succeeded)
            {
                user.PasswordInicialDefinida = true;

                await _userHelper
                    .UpdateUserAsync(user);

                return View(
                    "ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View(model);
        }

        // =========================================================
        // CHANGE PASSWORD
        // =========================================================

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(
            ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user =
                await _userHelper
                    .GetUserByEmailAsync(
                        User.Identity.Name);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Utilizador não encontrado.");

                return View(model);
            }

            var result =
                await _userHelper
                    .ChangePasswordAsync(
                        user,
                        model.OldPassword,
                        model.NewPassword);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] =
                    "Password alterada com sucesso.";

                return RedirectToAction(
                    nameof(RedirectByRole));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(
                    string.Empty,
                    error.Description);
            }

            return View(model);
        }

        // =========================================================
        // JWT API TOKEN
        // =========================================================

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateToken(
            [FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user =
                await _userHelper
                    .GetUserByEmailAsync(
                        model.Email);

            if (user == null)
            {
                return BadRequest();
            }

            var result =
                await _userHelper
                    .ValidatePasswordAsync(
                        user,
                        model.Password);

            if (!result.Succeeded)
            {
                return BadRequest();
            }

            var claims =
                new[]
                {
                    new Claim(
                        JwtRegisteredClaimNames.Sub,
                        user.Email),

                    new Claim(
                        JwtRegisteredClaimNames.Jti,
                        Guid.NewGuid().ToString())
                };

            var key =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        _configuration["Tokens:Key"]));

            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256);

            var token =
                new JwtSecurityToken(
                    _configuration["Tokens:Issuer"],
                    _configuration["Tokens:Audience"],
                    claims,
                    expires:
                        DateTime.UtcNow.AddDays(15),
                    signingCredentials:
                        credentials);

            var results =
                new
                {
                    token =
                        new JwtSecurityTokenHandler()
                            .WriteToken(token),

                    expiration =
                        token.ValidTo
                };

            return Created(
                string.Empty,
                results);
        }

        // =========================================================
        // NOT AUTHORIZED
        // =========================================================

        [AllowAnonymous]
        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}

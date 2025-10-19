using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Data.Entities;
using BilheticaAeronauticaWeb.Helper;
using BilheticaAeronauticaWeb.Model;
using BilheticaAeronauticaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly IBlobHelper _blobHelper;
        private readonly IPassageiroRepository _passageiroRepository;

        public AccountController(
            IUserHelper userHelper,
            IEMailHelper mailHelper,
            IConfiguration configuration,
            IBlobHelper blobHelper,
            IPassageiroRepository passageiroRepository)
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _blobHelper = blobHelper;
            _passageiroRepository = passageiroRepository;
        }

        // GET: Login
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Login
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _userHelper.LoginAsync(model);

            if (result.Succeeded)
            {
                if (this.Request.Query.Keys.Contains("ReturnUrl"))
                    return Redirect(this.Request.Query["ReturnUrl"].First());

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Failed to login!");
            return View(model);
        }

        // Logout
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _userHelper.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Register
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var userExists = await _userHelper.GetUserByEmailAsync(model.Username);
            if (userExists != null)
            {
                ModelState.AddModelError("Username", "Email já está registado.");
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

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            // Adicionar user à role Passageiro
            await _userHelper.AddUserToRoleAsync(user, "Passageiro");

            // Criar e salvar a entidade Passageiro associada ao novo utilizador
            var passageiro = new Passageiro
            {
                Nome = user.Nome,
                Apelido = user.Apelido,
                UserId = user.Id,
                DataRegisto = DateTime.UtcNow,
                WasDeleted = false
                // Preencha outros campos obrigatórios se existirem
            };

            await _passageiroRepository.AddAsync(passageiro);

            // Continuar com a geração do token e envio do email
            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = System.Net.WebUtility.UrlEncode(token);

            var tokenLink = Url.Action("ConfirmEmail", "Account", new
            {
                userId = user.Id,
                token = encodedToken
            }, protocol: HttpContext.Request.Scheme);

            var response = await _mailHelper.SendEmailAsync(model.Username, "Confirmação de Email",
               $"<h1>Confirmação de Email</h1> Para ativar sua conta, clique aqui: <a href='{tokenLink}'>Confirmar Email</a>");
            if (response.IsSuccess)
            {
                TempData["SuccessMessage"] = "Utilizador criado com sucesso! Verifique o seu email para confirmar a conta.";
                return RedirectToAction("Register");
            }

            ModelState.AddModelError(string.Empty, "Não foi possível enviar o email de confirmação.");
            return View(model);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
                return NotFound();

            var model = new EditProfileViewModel
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
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
                return NotFound();

            user.Nome = model.Nome;
            user.Apelido = model.Apelido;
            user.PhoneNumber = model.PhoneNumber;

            if (model.ImageFile != null)
            {
                var imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                user.ImageId = imageId;
            }

            var result = await _userHelper.UpdateUserAsync(user);
            if (result.Succeeded)
            {
                ViewBag.Message = "Perfil atualizado com sucesso.";
                model.ImageId = user.ImageId;
                return View(model);
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }




        // GET: ConfirmEmail
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
          
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return NotFound();  
            }

      
            var user = await _userHelper.GetUserByIdAsync(userId);
            if (user == null)
            {
      
                ViewBag.Message = "Utilizador não encontrado.";
                return View(); 
            }

   
            var decodedToken = System.Net.WebUtility.UrlDecode(token);

         
            var result = await _userHelper.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
             
                await _userHelper.LogoutAsync();

                TempData["SuccessMessage"] = "Email confirmado com sucesso. Pode agora iniciar sessão.";

             
                return RedirectToAction("Login", "Account");
            }
            else
            {
               
                ViewBag.Message = "Erro ao confirmar o email. O token pode ser inválido ou expirado.";
                return View();  
            }
        }


        // GET: RecoverPassword
        [AllowAnonymous]
        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(model.Email);

            
            if (user != null)
            {
                var token = await _userHelper.GeneratePasswordResetTokenAsync(user);
                var encodedToken = System.Net.WebUtility.UrlEncode(token);

                var link = Url.Action("ResetPassword", "Account", new
                {
                    userId = user.Id,
                    token = encodedToken,
                    email = user.Email
                }, protocol: HttpContext.Request.Scheme);

                var emailResponse = await _mailHelper.SendEmailAsync(model.Email, "Password Reset",
                    $"To reset your password, click here: <a href='{link}'>Reset Password</a>");

                if (!emailResponse.IsSuccess)
                {
                    ModelState.AddModelError(string.Empty, "Falha ao enviar o e-mail. Por favor, tente novamente mais tarde.");
                    return View(model);
                }
            }

           
            TempData["SuccessMessage"] = "Instruções para recuperar sua senha foram enviadas, caso o e-mail exista em nosso sistema.";
            return RedirectToAction(nameof(RecoverPassword));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string token, string email)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Login");
            }

            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token,
                Email = email
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(model.Email);

            if (user == null || user.Id != model.UserId)
            {
                // Para não expor se usuário existe
                return View("ResetPasswordConfirmation");
            }

            var decodedToken = System.Net.WebUtility.UrlDecode(model.Token);
            var result = await _userHelper.ResetPasswordAsync(user, decodedToken, model.NewPassword);

            if (result.Succeeded)
            {
                user.PasswordInicialDefinida = true;
                await _userHelper.UpdateUserAsync(user);

                return View("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
                return NotFound();

            var model = new EditProfileViewModel
            {
                Nome = user.Nome,
                Apelido = user.Apelido,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                ImageId = user.ImageId // Assumindo que ApplicationUser tem essa propriedade
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
                return NotFound();

            user.Nome = model.Nome;
            user.Apelido = model.Apelido;
            user.PhoneNumber = model.PhoneNumber;

            // Tratar upload de imagem
            if (model.ImageFile != null)
            {
                var imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                user.ImageId = imageId;
            }

            var result = await _userHelper.UpdateUserAsync(user);

            if (result.Succeeded)
            {
                ViewBag.Message = "Perfil atualizado com sucesso.";
                model.ImageId = user.ImageId; // Atualiza para exibir a nova imagem
                return View(model);
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
        }


        // GET: ChangePassword
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: ChangePassword
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(User.Identity.Name);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return View(model);
            }

            var result = await _userHelper.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
            {
                return RedirectToAction("ChangeUser");
            }
            else
            {
                ModelState.AddModelError(string.Empty, result.Errors.FirstOrDefault()?.Description);
            }

            return View(model);
        }

        // POST: CreateToken (JWT Token API)
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
                return BadRequest();

            var result = await _userHelper.ValidatePasswordAsync(user, model.Password);

            if (!result.Succeeded)
                return BadRequest();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Tokens:Issuer"],
                _configuration["Tokens:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(15),
                signingCredentials: credentials);

            var results = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            };

            return Created(string.Empty, results);
        }


        // GET: NotAuthorized
        [AllowAnonymous]
        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}


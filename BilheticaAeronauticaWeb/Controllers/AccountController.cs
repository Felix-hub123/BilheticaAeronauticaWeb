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
using ZXing;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IEMailHelper _mailHelper;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly DataContext _context;
        private readonly IBlobHelper _blobHelper;

        public AccountController(
            IUserHelper userHelper,
            IEMailHelper mailHelper,
            IConfiguration configuration,
            UserManager<User> userManager,
             SignInManager<User> signInManager,
             DataContext context,
             IBlobHelper blobHelper
        )
        {
            _userHelper = userHelper;
            _mailHelper = mailHelper;
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _blobHelper = blobHelper;
        }


        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Credenciais inválidas.");
                return View(model);
            }

            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Tem de confirmar o email antes de fazer login.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                // <<<<<< COPIA este bloco que ali em cima para cá!
                user = await _userHelper.GetUserByEmailAsync(model.Email);

                if (await _userHelper.IsUserInRoleAsync(user, "Admin"))
                    return RedirectToAction("Index", "Admin");
                if (await _userHelper.IsUserInRoleAsync(user, "Funcionario"))
                    return RedirectToAction("Index", "Funcionarios");
                if (await _userHelper.IsUserInRoleAsync(user, "Passageiro"))
                    return RedirectToAction("Index", "Passageiro");

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Credenciais inválidas.");
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterNewUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var existingUser = await _userHelper.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Já existe utilizador com este email.");
                return View(model);
            }

            var user = new User
            {
                Nome = model.Nome,
                Apelido = model.Apelido,
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
            };

            var result = await _userHelper.AddUserAsync(user, model.Password);
            if (result != IdentityResult.Success)
            {
                ModelState.AddModelError(string.Empty, "Não foi possível criar o utilizador.");
                return View(model);
            }

            // ADICIONA O USER À ROLE
            await _userHelper.AddUserToRoleAsync(user, "Passageiro");

            // *** GARANTE A CRIAÇÃO DO PASSAGEIRO NA TABELA Passageiros ***
            if (!_context.Passageiros.Any(p => p.UserId == user.Id))
            {
                _context.Passageiros.Add(new Passageiro
                {
                    Nome = model.Nome,
                    Apelido = model.Apelido,
                    DataRegisto = DateTime.UtcNow,
                    UserId = user.Id
                });
                await _context.SaveChangesAsync();
            }

            // PROCEDE NORMALMENTE COM O PROCESSO DE CONFIRMAÇÃO DE EMAIL
            var token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
            var tokenLink = Url.Action(
                "ConfirmEmail", "Account",
                new { userId = user.Id, token = token },
                protocol: HttpContext.Request.Scheme);
            await _mailHelper.SendEmailAsync(
                model.Email,
                "Confirmação de Email",
                $"Bem-vindo! Confirme o seu email: <a href=\"{tokenLink}\">Confirmar Email</a>");

            ViewBag.Message = "Instruções enviadas para o email. Verifique a caixa de entrada.";
            return View("RegisterConfirmation");
        }




        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
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

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            user.Nome = model.Nome;
            user.Apelido = model.Apelido;
            user.PhoneNumber = model.PhoneNumber;
            if (model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var imageId = await _blobHelper.UploadBlobAsync(model.ImageFile, "users");
                user.ImageId = imageId;
                model.ImageId = imageId; 
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                ViewBag.Message = "Perfil atualizado com sucesso!";
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Erro ao atualizar o perfil.");
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Index", "Home");
            }
              

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
               

            
            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                ViewBag.Message = "Email confirmado com sucesso.";
                return View("ConfirmEmail"); 
            }
            else
            {
                ViewBag.Message = "Erro ao confirmar o email. O token pode ter expirado.";
                return View("ConfirmEmail");
            }

        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword( RecoverPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
            {
                ViewBag.Message = "Se o email existir, foi enviado um link de recuperação para o seu email.";
                return View("ForgotPasswordConfirmation"); 
            }

            var token = await _userHelper.GeneratePasswordResetTokenAsync(user);
            var link = Url.Action(
                "ResetPassword", "Account", 
                new { userId = user.Id, token = token },
                protocol: HttpContext.Request.Scheme);

            await _mailHelper.SendEmailAsync(model.Email, "Recuperação de Password",
                $"Para redefinir a sua password clique: <a href=\"{link}\">Reset Password</a>");

            ViewBag.Message = "Se o email existir, foi enviado um link de recuperação para o seu email.";
            return View("ForgotPasswordConfirmation"); 
        }


        [HttpGet]
        public  IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account"); 
            }

            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Token = token
            };
            return View(model);
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userHelper.GetUserByEmailAsync(model.Email);
            if (user == null)
                return View("ResetPasswordConfirmation");

            var result = await _userHelper.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (result.Succeeded)
            {
                // Mostra página amigável
                return View("ResetPasswordConfirmation");
                // Alternativamente: return RedirectToAction("Login");
            }
            // Se erro, mostra as mensagens de erro ao utilizador
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CreateToken([FromBody] LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userHelper.GetUserByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userHelper.ValidatePasswordAsync(user, model.Password);

                    if (result.Succeeded)
                    {
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

                        return this.Created(string.Empty, results);
                    }
                }
            }

            return BadRequest();
        }




    }
}

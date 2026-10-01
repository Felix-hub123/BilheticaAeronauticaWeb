using BilheticaAeronauticaWeb.Data;
using BilheticaAeronauticaWeb.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    /// <summary>
    /// Dashboard principal do utilizador com role Passageiro.
    /// </summary>
    [Authorize(Roles = "Passageiro")]
    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public class PassageiroController : Controller
    {
        private readonly IPassageiroRepository _passageiroRepository;
        private readonly IUserHelper _userHelper;

        public PassageiroController(
            IPassageiroRepository passageiroRepository,
            IUserHelper userHelper)
        {
            _passageiroRepository = passageiroRepository;
            _userHelper = userHelper;
        }

        /// <summary>
        /// Dashboard do Passageiro.
        /// Verifica primeiro se existe uma entidade Passageiro
        /// associada ao utilizador autenticado.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            var user =
                await _userHelper.GetUserAsync(User);

            if (user == null)
            {
                return RedirectToAction(
                    "Login",
                    "Account");
            }

            var passageiro =
                await _passageiroRepository
                    .GetByUserIdAsync(user.Id);

            /*
             * Contas antigas podem ter:
             *
             * AspNetUsers ✅
             * Role Passageiro ✅
             * Passageiros ❌
             *
             * Nesse caso obrigamos o utilizador
             * a completar/criar o seu perfil.
             */
            if (passageiro == null)
            {
                TempData["InfoMessage"] =
                    "Complete os seus dados de passageiro para continuar.";

                return RedirectToAction(
                    "Create",
                    "Passageiros");
            }

            return View();
        }
    }
}

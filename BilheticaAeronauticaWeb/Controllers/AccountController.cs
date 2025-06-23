using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login(string returnUrl = null)
        {
            return Redirect($"/Identity/Account/Login?ReturnUrl={returnUrl}");
        }
    }
}

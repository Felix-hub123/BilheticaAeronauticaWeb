using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult NotAuthorized()
        {
            return View();
        }
    }
}

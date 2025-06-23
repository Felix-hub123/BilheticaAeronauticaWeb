using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class ClienteController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers
{
    [Authorize(Roles = "Passageiro")]
    public class PassageiroController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

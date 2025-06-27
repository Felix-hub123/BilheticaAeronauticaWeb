using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers
{
    [Authorize(Roles = "Funcionario,Admin")]
    public class FuncionariosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

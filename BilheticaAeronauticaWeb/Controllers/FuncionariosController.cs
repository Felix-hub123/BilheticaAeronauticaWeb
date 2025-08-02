using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers
{/// <summary>
 /// Controlador responsável pela gestão de funcionários.
 /// Acesso exclusivo para utilizadores com roles "Funcionario"
 /// conforme matriz de permissões definida no projeto.
 /// </summary>
    [Authorize(Roles = "Funcionario")]
    public class FuncionariosController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

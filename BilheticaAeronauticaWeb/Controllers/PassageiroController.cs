using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers
{


    /// <summary>
    /// Controlador responsável pelas funcionalidades acessíveis ao utilizador com role Passageiro.
    /// Garante que apenas utilizadores autenticados com o papel Passageiro têm acesso às suas ações.
    /// </summary>
    [Authorize(Roles = "Passageiro")]
    public class PassageiroController : Controller
    {
        /// <summary>
        /// Página principal do painel do passageiro.
        /// Aqui o passageiro poderá consultar as suas informações pessoais, histórico de voos, reservas, etc.
        /// </summary>
        /// <returns>View do painel do passageiro.</returns>
        [Authorize(Roles = "Passageiro")]
        public IActionResult Index()
        {
            return View();
        }
    }
}

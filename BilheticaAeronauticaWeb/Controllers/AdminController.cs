using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers
{

    /// <summary>
    /// Controlador responsável pelo painel de administração.
    /// Apenas acessível a utilizadores com o papel (role) 'Admin',
    /// cumprindo a matriz de permissões e regras de negócio do projeto (ver PDF - página da matriz).
    ///
    /// Aqui serão centralizadas as funcionalidades exclusivas do administrador:
    /// - Gestão de funcionários (criação de contas)
    /// - Gestão de cidades/aeroportos e aparelhos (CRUD)
    /// - Gestão de clientes
    /// - Outras funcionalidades reservadas ao administrador global da plataforma.
    ///
    /// NOTA: O atributo [Authorize(Roles = "Admin")] restringe o acesso a este controlador a utilizadores autenticados com o papel "Admin",
    /// conforme exigido nos requisitos funcionais do projeto (PDF).
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

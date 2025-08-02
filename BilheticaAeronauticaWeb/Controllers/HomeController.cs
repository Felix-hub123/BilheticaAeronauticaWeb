using BilheticaAeronauticaWeb.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    /// <summary>
    /// Controlador principal responsável pelas ações públicas
    /// da aplicação como home page, privacidade, tratamento de erros
    /// e redirecionamento para painéis consoante role do utilizador autenticado.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVooRepository _vooRepository;

        /// <summary>
        /// Construtor com injeção de dependências para logging e acesso a dados de voos.
        /// </summary>
        /// <param name="logger">Logger do aplicativo.</param>
        /// <param name="vooRepository">Repositório para acesso aos voos.</param>
        public HomeController(ILogger<HomeController> logger, IVooRepository vooRepository)
        {
            _logger = logger;
            _vooRepository = vooRepository;
        }


        /// <summary>
        /// Exibe a página inicial (home) da aplicação.
        /// Obtém lista de voos disponíveis, conta total, e pré-carrega os 5 primeiros.
        /// </summary>
        /// <returns>View da home com dados dos voos.</returns>
        public async Task<IActionResult> Index()
        {
            var voos = await _vooRepository.GetAllVoosAsync();
            ViewBag.NumeroVoos = voos.Count;
            ViewBag.ListaVoos = voos.Take(5).ToList();


            return View();
        }


        /// <summary>
        /// Exibe a página da política de privacidade.
        /// </summary>
        /// <returns>View da política de privacidade.</returns>
        public IActionResult Privacy()
        {
            return View();
        }


        /// <summary>
        /// Método para exibir página de erro genérica com traceId.
        /// Usado para tratamento de erros globais.
        /// </summary>
        /// <returns>View de erro.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new { RequestId = HttpContext.TraceIdentifier });
        }


        /// <summary>
        /// Página customizada para erro HTTP 404 (Página não encontrada).
        /// </summary>
        /// <returns>View de página não encontrada.</returns>
        [Route("error/404")]
        public IActionResult Error404()
        {
            return View();
        }


        /// <summary>
        /// Página para apresentação dos detalhes de ofertas/promos.
        /// </summary>
        /// <returns>View estática (ex.: oferta especial).</returns>
        public IActionResult VerDetalhes()
        {
            return View();
        }


        /// <summary>
        /// Página estática relacionada a promoções atuais ou oportunidades.
        /// </summary>
        /// <returns>View da promoção.</returns>
        public IActionResult AproveitarPromocao()
        {
            return View();
        }


        /// <summary>
        /// Página informativa sobre o programa de fidelidade da companhia.
        /// </summary>
        /// <returns>View sobre fidelidade.</returns>
        public IActionResult SaberMaisFidelidade()
        {
            return View();
        }


        /// <summary>
        /// Redireciona o utilizador autenticado para o painel correto conforme o seu role.
        /// Admin → Admin Controller
        /// Funcionário → Funcionarios Controller
        /// Passageiro → Passageiro Controller
        /// Não autenticado ou outros → Home
        /// </summary>
        /// <returns>Redirect para o painel apropriado ou home.</returns>
        public IActionResult Painel()
        {
            if (!User.Identity.IsAuthenticated)
                  return RedirectToAction("Index");
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");
            else if (User.IsInRole("Funcionario"))
                return RedirectToAction("Index", "Funcionario");
            else if (User.IsInRole("Passageiro"))
                return RedirectToAction("Index", "Passageiro"); 

                 return RedirectToAction("Index", "Home");
        }

    }
}

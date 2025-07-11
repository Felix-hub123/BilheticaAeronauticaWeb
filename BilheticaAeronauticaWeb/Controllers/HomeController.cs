using BilheticaAeronauticaWeb.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IVooRepository _vooRepository;
    

        public HomeController(ILogger<HomeController> logger, IVooRepository vooRepository)
        {
            _logger = logger;
            _vooRepository = vooRepository;
        }

        public async Task<IActionResult> Index()
        {
            var voos = await _vooRepository.GetAllVoosAsync();
            ViewBag.NumeroVoos = voos.Count;
            ViewBag.ListaVoos = voos.Take(5).ToList();


            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new { RequestId = HttpContext.TraceIdentifier });
        }

        [Route("error/404")]

        public IActionResult Error404()
        {
            return View();
        }
    }
}

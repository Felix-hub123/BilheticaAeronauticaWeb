using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace BilheticaAeronauticaWeb.Controllers
{

    /// <summary>
    /// Controlador responsável por tratar erros HTTP e exceções globais.
    /// Implementa páginas customizadas para códigos de erro 404, 403 e erros genéricos.
    /// Garante uma experiência de utilizador robusta sem exposição de mensagens técnicas.
    /// </summary>
    public class ErrorController : Controller
    {

        /// <summary>
        /// Trata os códigos HTTP de erro e encaminha para a view adequada.
        /// Suporta códigos 404 (não encontrado), 403 (proibido) e erro genérico para outros casos.
        /// </summary>
        /// <param name="statusCode">Código de estado HTTP do erro</param>
        /// <returns>View customizada adequada ao código de erro</returns>
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Desculpe, a página que procuraste não foi encontrada.";
                    return View("NotFound");
                case 403:
                    ViewBag.ErrorMessage = "Não tens permissão para aceder a esta página.";
                    return View("Forbidden");
              
                default:
                    ViewBag.ErrorMessage = "Ocorreu um erro inesperado.";
                    return View("Error");
            }
        }

        /// <summary>
        /// Trata exceções não capturadas pela aplicação e erros do servidor.
        /// Apresenta uma view genérica de erro com mensagens amigáveis e sem expor detalhes técnicos.
        /// </summary>
        /// <returns>View de erro genérico</returns>
        [Route("Error")]
        public IActionResult Error()
        {
            var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
          
            ViewBag.ErrorMessage = "Ocorreu um erro inesperado no sistema.";
            ViewBag.Path = exceptionDetails?.Path;

            return View();
        }
    }
}

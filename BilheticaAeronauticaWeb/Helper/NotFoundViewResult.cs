using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BilheticaAeronauticaWeb.Helper
{
    /// <summary>
    /// Resultado de uma View com status HTTP 404 (Not Found).
    /// Permite retornar uma View com código HTTP 404 definido.
    /// </summary>
    public class NotFoundViewResult : ViewResult
    {
        /// <summary>
        /// Inicializa uma nova instância de <see cref="NotFoundViewResult"/> com o nome da view a ser exibida.
        /// </summary>
        /// <param name="viewName">Nome da view a renderizar para a resposta 404.</param>
        public NotFoundViewResult(string viewName)
        {
            ViewName = viewName;
            StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
    
   
}

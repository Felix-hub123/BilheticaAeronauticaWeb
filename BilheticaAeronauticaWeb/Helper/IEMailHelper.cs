using BilheticaAeronauticaWeb.Helper;
using System.Threading.Tasks;

namespace SuperShop.Helpers
{
    /// <summary>
    /// Interface que define os métodos para envio de emails assincronamente.
    /// </summary>
    public interface IEMailHelper
    {
        /// <summary>
        /// Envia um email assíncrono para o destinatário especificado,
        /// com o assunto e conteúdo em formato HTML.
        /// </summary>
        /// <param name="email">Endereço de email do destinatário.</param>
        /// <param name="subject">Assunto do email.</param>
        /// <param name="htmlMessage">Conteúdo HTML do corpo do email.</param>
        /// <returns>
        /// Uma tarefa que representa a operação assíncrona, contendo um objeto <see cref="Response"/> 
        /// com o estado de sucesso/falha e informação adicional.
        /// </returns>
        Task<Response> SendEmailAsync(string email, string subject, string htmlMessage);
    }
}

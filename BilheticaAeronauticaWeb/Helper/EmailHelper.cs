using BilheticaAeronauticaWeb.Helper;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace SuperShop.Helpers
{

    /// <summary>
    /// Helper para envio de email via SMTP usando a biblioteca MailKit.
    /// </summary>
    public class EMailHelper : IEMailHelper
    {

        /// <summary>
        /// Inicializa uma nova instância do <see cref="EMailHelper"/> com as configurações necessárias para envio de email.
        /// </summary>
        /// <param name="configuration">Interface para obter configurações da aplicação.</param>
        private readonly IConfiguration _configuration;

        public EMailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        /// <summary>
        /// Envia um email assíncrono para o destinatário especificado com o assunto e corpo em HTML.
        /// </summary>
        /// <param name="email">Endereço de email do destinatário.</param>
        /// <param name="subject">Assunto do email.</param>
        /// <param name="htmlMessage">Conteúdo HTML do corpo do email.</param>
        /// <returns>
        /// Um objeto <see cref="Response"/> indicando sucesso ou falha, e mensagem detalhada em caso de erro.
        /// </returns>
        public async Task<Response> SendEmailAsync(string email, string subject, string htmlMessage) 
        {
            var nameFrom = _configuration["Mail:NameFrom"];
            var from = _configuration["Mail:From"];
            var smtp = _configuration["Mail:Smtp"];
            var port = _configuration["Mail:Port"];
            var password = _configuration["Mail:Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(nameFrom ?? from, from));
            message.To.Add(new MailboxAddress(email, email));
            message.Subject = subject;

            var bodybuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage,
            };
            message.Body = bodybuilder.ToMessageBody();

            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                    await client.ConnectAsync(smtp, int.Parse(port), SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(from, password);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.ToString()
                };
            }

            return new Response
            {
                IsSuccess = true
            };
        }
    }
}

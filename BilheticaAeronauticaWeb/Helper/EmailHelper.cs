using BilheticaAeronauticaWeb.Helper;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace SuperShop.Helpers
{
    /// <summary>
    /// Helper para envio de emails através de SMTP utilizando MailKit.
    /// </summary>
    public class EMailHelper : IEMailHelper
    {
        private readonly IConfiguration _configuration;

        public EMailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Envia um email HTML para o destinatário indicado.
        /// </summary>
        public async Task<Response> SendEmailAsync(
            string email,
            string subject,
            string htmlMessage)
        {
            try
            {
                var nameFrom =
                    _configuration["Mail:NameFrom"];

                var from =
                    _configuration["Mail:From"];

                var username =
                    _configuration["Mail:Username"];

                var smtp =
                    _configuration["Mail:Smtp"];

                var portString =
                    _configuration["Mail:Port"];

                var password =
                    _configuration["Mail:Password"];

                // =====================================================
                // VALIDAR CONFIGURAÇÕES
                // =====================================================

                if (string.IsNullOrWhiteSpace(from))
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Mail:From não está configurado."
                    };
                }

                if (string.IsNullOrWhiteSpace(smtp))
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Mail:Smtp não está configurado."
                    };
                }

                if (string.IsNullOrWhiteSpace(portString) ||
                    !int.TryParse(portString, out var port))
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Mail:Port não está configurado corretamente."
                    };
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    return new Response
                    {
                        IsSuccess = false,
                        Message = "Mail:Password não está configurado."
                    };
                }

                /*
                 * Caso não exista Mail:Username,
                 * utiliza o próprio endereço From.
                 */
                if (string.IsNullOrWhiteSpace(username))
                {
                    username = from;
                }

                // =====================================================
                // CRIAR MENSAGEM
                // =====================================================

                var message = new MimeMessage();

                message.From.Add(
                    new MailboxAddress(
                        nameFrom ?? from,
                        from));

                message.To.Add(
                    MailboxAddress.Parse(email));

                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                };

                message.Body =
                    bodyBuilder.ToMessageBody();

                // =====================================================
                // SMTP
                // =====================================================

                using var client =
                    new MailKit.Net.Smtp.SmtpClient();

                /*
                 * Porta 465 normalmente utiliza SSL diretamente.
                 * Porta 587 normalmente utiliza STARTTLS.
                 */
                var socketOptions =
                    port == 465
                        ? SecureSocketOptions.SslOnConnect
                        : SecureSocketOptions.StartTls;

                await client.ConnectAsync(
                    smtp,
                    port,
                    socketOptions);

                await client.AuthenticateAsync(
                    username,
                    password);

                await client.SendAsync(message);

                await client.DisconnectAsync(true);

                return new Response
                {
                    IsSuccess = true,
                    Message = "Email enviado com sucesso."
                };
            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.ToString()
                };
            }
        }
    }
}
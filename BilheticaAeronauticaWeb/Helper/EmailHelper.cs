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
            var nameFrom = _configuration["Mail:NameFrom"];
            var from = _configuration["Mail:From"];
            var smtp = _configuration["Mail:Smtp"];
            var portString = _configuration["Mail:Port"];
            var password = _configuration["Mail:Password"];

            // =========================================================
            // VALIDAR CONFIGURAÇÕES
            // =========================================================

            if (string.IsNullOrWhiteSpace(nameFrom))
            {
                nameFrom = "AeroTicket";
            }

            if (string.IsNullOrWhiteSpace(from))
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "A configuração Mail:From não foi encontrada."
                };
            }

            if (string.IsNullOrWhiteSpace(smtp))
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "A configuração Mail:Smtp não foi encontrada."
                };
            }

            if (!int.TryParse(portString, out int port))
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "A configuração Mail:Port é inválida."
                };
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = "A configuração Mail:Password não foi encontrada."
                };
            }

            // =========================================================
            // CRIAR EMAIL
            // =========================================================

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    nameFrom,
                    from));

            message.To.Add(
                new MailboxAddress(
                    email,
                    email));

            message.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            message.Body = bodyBuilder.ToMessageBody();

            // =========================================================
            // ENVIAR EMAIL
            // =========================================================

            const int maxTentativas = 2;

            for (int tentativa = 1;
                 tentativa <= maxTentativas;
                 tentativa++)
            {
                try
                {
                    using var client =
                        new MailKit.Net.Smtp.SmtpClient();

                    // 30 segundos
                    client.Timeout = 30000;

                    await client.ConnectAsync(
                        smtp,
                        port,
                        SecureSocketOptions.StartTls);

                    await client.AuthenticateAsync(
                        from,
                        password);

                    await client.SendAsync(message);

                    await client.DisconnectAsync(true);

                    return new Response
                    {
                        IsSuccess = true,
                        Message = "Email enviado com sucesso."
                    };
                }
                catch (TimeoutException ex)
                {
                    Console.WriteLine(
                        $"Tentativa SMTP {tentativa}/{maxTentativas} falhou por timeout: {ex.Message}");

                    if (tentativa == maxTentativas)
                    {
                        return new Response
                        {
                            IsSuccess = false,
                            Message =
                                $"O servidor de email não respondeu após {maxTentativas} tentativas. {ex.Message}"
                        };
                    }

                    // Esperar 2 segundos antes da segunda tentativa.
                    await Task.Delay(2000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"ERRO SMTP: {ex}");

                    return new Response
                    {
                        IsSuccess = false,
                        Message = ex.ToString()
                    };
                }
            }

            return new Response
            {
                IsSuccess = false,
                Message = "Não foi possível enviar o email."
            };
        }
    }
}
using BilheticaAeronauticaWeb.Helper;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace SuperShop.Helpers
{
   

    public class EMailHelper : IEMailHelper
    {
        private readonly IConfiguration _configuration;

        public EMailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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

using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Helper
{
    public class EmailSender : IEmailSender
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailSender(IOptions<EmailSettings> options)
        {
            _apiKey = options.Value.SendGridApiKey;
            _fromEmail = options.Value.FromEmail;
            _fromName = options.Value.FromName;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SendGrid.SendGridClient(_apiKey);
            var msg = new SendGrid.Helpers.Mail.SendGridMessage()
            {
                From = new SendGrid.Helpers.Mail.EmailAddress(_fromEmail, _fromName),
                Subject = subject,
                HtmlContent = htmlMessage
            };
            msg.AddTo(new SendGrid.Helpers.Mail.EmailAddress(email));
            var response = await client.SendEmailAsync(msg);
            // Opcional:-verifique resposta
            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
            {
                // opcional: lançar uma exceção ou log
            }

        }
    }

}

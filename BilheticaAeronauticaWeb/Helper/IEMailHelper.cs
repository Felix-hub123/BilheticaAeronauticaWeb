

using BilheticaAeronauticaWeb.Helper;
using System.Threading.Tasks;

namespace SuperShop.Helpers
{
    public interface IEMailHelper
    {
        Task<Response> SendEmailAsync(string email, string subject, string htmlMessage);
    }
}

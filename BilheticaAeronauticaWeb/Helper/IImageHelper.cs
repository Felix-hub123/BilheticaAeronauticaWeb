using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BilheticaAeronauticaWeb.Helper
{
    public interface IImageHelper
    {
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}

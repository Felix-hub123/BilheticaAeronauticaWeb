using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Helper
{
    /// <summary>
    /// Helper para realizar o upload de imagens enviadas via formulário para a pasta específica no servidor.
    /// </summary>
    public class ImageHelper : IImageHelper
    {
        /// <summary>
        /// Faz o upload de uma imagem recebida via formulário HTTP para uma pasta dentro de wwwroot/images.
        /// </summary>
        /// <param name="imageFile">Ficheiro de imagem submetido pelo utilizador.</param>
        /// <param name="folder">Nome da subpasta dentro da pasta 'images' onde gravar o ficheiro.</param>
        /// <returns>
        /// O caminho relativo (URL virtual) para acessar a imagem gravada, no formato "~/images/{folder}/{nomeArquivo}".
        /// </returns>
        public async  Task<string> UploadImageAsync(IFormFile imageFile, string folder)
        {
            string guid = Guid.NewGuid().ToString();
            string file = $"{guid}.jpg";


            string path = Path.Combine(
                Directory.GetCurrentDirectory(),
              $"wwwroot\\images\\{folder}",
              file);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return   $"~/images/{folder}/{file}";
        }
    }
}

using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Helper
{
    /// <summary>
    /// Interface responsável pela gestão de imagens da aplicação.
    /// </summary>
    public interface IImageHelper
    {
        /// <summary>
        /// Faz upload de uma imagem.
        /// Em desenvolvimento guarda localmente.
        /// Em produção guarda no Supabase Storage.
        /// </summary>
        Task<Guid> UploadImageAsync(
            IFormFile imageFile,
            string folder);

        /// <summary>
        /// Elimina uma imagem.
        /// </summary>
        Task DeleteImageAsync(
            Guid imageId,
            string folder);

        /// <summary>
        /// Devolve o URL correto da imagem.
        /// </summary>
        string GetImageUrl(
            Guid imageId,
            string folder,
            string placeholderName = "noimage");
    }
}
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BilheticaAeronauticaWeb.Helper
{ /// <summary>
  /// Interface que define métodos para operações relacionadas com imagens,
  /// como o upload assíncrono de ficheiros para uma determinada pasta ou armazenamento.
  /// </summary>
    public interface IImageHelper
    {
        /// <summary>
        /// Faz o upload de uma imagem recebida via formulário HTTP para a pasta/contêiner especificada,
        /// retornando a URI ou nome do ficheiro que foi gravado.
        /// </summary>
        /// <param name="imageFile">Ficheiro de imagem enviado pelo utilizador.</param>
        /// <param name="folder">Nome da pasta ou container onde a imagem será armazenada.</param>
        /// <returns>
        /// Uma <see cref="string"/> contendo o caminho, URI ou identificador da imagem armazenada.
        /// </returns>
        Task<string> UploadImageAsync(IFormFile imageFile, string folder);
    }
}

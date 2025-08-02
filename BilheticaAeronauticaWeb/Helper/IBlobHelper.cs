using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Helper
{
    /// <summary>
    /// Interface que define métodos para upload de ficheiros para um armazenamento Blob,
    /// abstraindo a origem dos dados (formulário, bytes ou caminho físico).
    /// </summary>
    public interface IBlobHelper
    {
        /// <summary>
        /// Realiza o upload de um ficheiro recebido via formulário HTTP para o container especificado.
        /// </summary>
        /// <param name="file">Ficheiro enviado pelo utilizador através do formulário.</param>
        /// <param name="containerName">Nome do container para onde o ficheiro será enviado.</param>
        /// <returns>
        /// Um <see cref="Guid"/> que identifica unicamente o ficheiro no armazenamento Blob.
        /// </returns>
        Task<Guid> UploadBlobAsync(IFormFile file, string containerName);


        /// <summary>
        /// Realiza o upload de um ficheiro a partir de uma matriz de bytes para o container especificado.
        /// </summary>
        /// <param name="file">Conteúdo binário do ficheiro.</param>
        /// <param name="containerName">Nome do container para onde o ficheiro será enviado.</param>
        /// <returns>
        /// Um <see cref="Guid"/> que identifica unicamente o ficheiro no armazenamento Blob.
        /// </returns>
        Task<Guid> UploadBlobAsync(byte[] file, string containerName);


        /// <summary>
        /// Realiza o upload de um ficheiro a partir de um caminho físico no disco para o container especificado.
        /// </summary>
        /// <param name="image">Caminho completo do ficheiro físico.</param>
        /// <param name="containerName">Nome do container para onde o ficheiro será enviado.</param>
        /// <returns>
        /// Um <see cref="Guid"/> que identifica unicamente o ficheiro no armazenamento Blob.
        /// </returns>
        Task<Guid> UploadBlobAsync(string image, string containerName);
    }
}

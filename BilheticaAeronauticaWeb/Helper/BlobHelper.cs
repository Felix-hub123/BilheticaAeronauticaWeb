using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Helper
{
    public class BlobHelper : IBlobHelper
    {
        /// <summary>
        /// Helper para facilitar operações de upload de ficheiros para Azure Blob Storage
        /// utilizando a biblioteca legacy Microsoft.WindowsAzure.Storage.
        /// </summary>
        private readonly CloudBlobClient _blobClient;

        /// <summary>
        /// Inicializa a instância do BlobHelper com a configuração da connection string para o Azure Storage.
        /// </summary>
        /// <param name="configuration">Interface de configuração para obter a connection string do blob storage.</param>
        public BlobHelper(IConfiguration configuration)
        {
            string keys = configuration["Blob:ConnectionString"];
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            _blobClient = storageAccount.CreateCloudBlobClient();

        }

        /// <summary>
        /// Faz upload de um ficheiro recebido via formulário HTTP para o container especificado.
        /// </summary>
        /// <param name="file">Ficheiro enviado pelo utilizador.</param>
        /// <param name="containerName">Nome do container onde o ficheiro será armazenado.</param>
        /// <returns>GUID gerado que identifica unicamente o blob criado.</returns>
        public async Task<Guid> UploadBlobAsync(IFormFile file, string containerName)
        {
            Stream stream = file.OpenReadStream();
            return await UploadStreamAsync(stream, containerName);
        }


        /// <summary>
        /// Faz upload de um ficheiro a partir de um array de bytes para o container especificado.
        /// </summary>
        /// <param name="file">Conteúdo do ficheiro em bytes.</param>
        /// <param name="containerName">Nome do container onde o ficheiro será armazenado.</param>
        /// <returns>GUID gerado que identifica unicamente o blob criado.</returns>
        public async Task<Guid> UploadBlobAsync(byte[] file, string containerName)
        {
            MemoryStream stream = new MemoryStream(file);
            return await UploadStreamAsync(stream, containerName);
        }


        /// <summary>
        /// Faz upload de um ficheiro a partir do caminho físico no disco para o container especificado.
        /// </summary>
        /// <param name="image">Caminho completo do ficheiro a enviar.</param>
        /// <param name="containerName">Nome do container onde o ficheiro será armazenado.</param>
        /// <returns>GUID gerado que identifica unicamente o blob criado.</returns>

        public async Task<Guid> UploadBlobAsync(string image, string containerName)
        {
            Stream stream = File.OpenRead(image);
            return await UploadStreamAsync(stream, containerName);
        }


        /// <summary>
        /// Método auxiliar privado que faz o upload do stream para o container de armazenamento Blob.
        /// </summary>
        /// <param name="stream">Stream com os dados a enviar.</param>
        /// <param name="containerName">Nome do container destino.</param>
        /// <returns>GUID gerado que identifica unicamente o blob criado.</returns>
        private async Task<Guid> UploadStreamAsync(Stream stream, string containerName)
        {
            Guid name = Guid.NewGuid();
            CloudBlobContainer container = _blobClient.GetContainerReference(containerName);
            CloudBlockBlob blockBlob = container.GetBlockBlobReference($"{name}");
            await blockBlob.UploadFromStreamAsync(stream);
            return name;
        }
    }
    
}

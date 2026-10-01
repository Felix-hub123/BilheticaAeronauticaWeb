using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Supabase.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BilheticaAeronauticaWeb.Helper
{
    /// <summary>
    /// Helper responsável pela gestão das imagens.
    ///
    /// Desenvolvimento:
    /// guarda as imagens localmente em wwwroot/images.
    ///
    /// Produção:
    /// guarda as imagens no Supabase Storage.
    /// </summary>
    public class ImageHelper : IImageHelper
    {
        private readonly IWebHostEnvironment _env;

        private readonly string _supabaseUrl =
            "https://bsfdnspnkqxgxscnkczs.supabase.co";

        /*
         * Substituir pela Publishable Key do Supabase.
         *
         * Mais tarde podemos mover isto para
         * variáveis de ambiente do Render.
         */
        private readonly string _supabaseKey =
            "sb_publishable_hf1ONHZoyoUvZabKGPNNhw_Sg4irpJ3";

        private readonly string _bucketName =
            "bilhetica";

        public ImageHelper(
            IWebHostEnvironment env)
        {
            _env = env;
        }

        // =========================================================
        // UPLOAD
        // =========================================================

        /// <summary>
        /// Faz upload da imagem e devolve o Guid
        /// que será guardado na base de dados.
        /// </summary>
        public async Task<Guid> UploadImageAsync(
            IFormFile imageFile,
            string folder)
        {
            if (imageFile == null ||
                imageFile.Length == 0)
            {
                return Guid.Empty;
            }

            ValidarImagem(imageFile);

            var imageId =
                Guid.NewGuid();

            // =====================================================
            // DESENVOLVIMENTO
            // =====================================================

            if (_env.IsDevelopment())
            {
                var extension =
                    ObterExtensao(imageFile);

                var path =
                    Path.Combine(
                        _env.WebRootPath,
                        "images",
                        folder);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                var fileName =
                    $"{imageId}{extension}";

                var fullPath =
                    Path.Combine(
                        path,
                        fileName);

                using var stream =
                    new FileStream(
                        fullPath,
                        FileMode.Create);

                await imageFile
                    .CopyToAsync(stream);

                return imageId;
            }

            // =====================================================
            // PRODUÇÃO - SUPABASE
            // =====================================================

            /*
             * Em produção utilizamos sempre uma extensão fixa.
             *
             * Assim, como a BD guarda apenas o Guid,
             * conseguimos sempre reconstruir o URL.
             *
             * O ContentType informa ao Supabase
             * qual é realmente o formato da imagem.
             */
            var filePath =
                $"{folder}/{imageId}.img";

            using var memoryStream =
                new MemoryStream();

            await imageFile
                .CopyToAsync(memoryStream);

            var fileBytes =
                memoryStream.ToArray();

            var storageClient =
                CriarStorageClient();

            var bucket =
                storageClient.From(_bucketName);

            var options =
            new Supabase.Storage.FileOptions
            {
                Upsert = false,
                ContentType =
                    ObterContentType(imageFile)
            };

            await bucket.Upload(
                fileBytes,
                filePath,
                options);

            return imageId;
        }

        // =========================================================
        // DELETE
        // =========================================================

        /// <summary>
        /// Elimina uma imagem do armazenamento.
        /// </summary>
        public async Task DeleteImageAsync(
            Guid imageId,
            string folder)
        {
            if (imageId == Guid.Empty)
            {
                return;
            }

            // =====================================================
            // DESENVOLVIMENTO
            // =====================================================

            if (_env.IsDevelopment())
            {
                var path =
                    Path.Combine(
                        _env.WebRootPath,
                        "images",
                        folder);

                if (!Directory.Exists(path))
                {
                    return;
                }

                var files =
                    Directory.GetFiles(
                        path,
                        $"{imageId}.*");

                foreach (var file in files)
                {
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }

                return;
            }

            // =====================================================
            // PRODUÇÃO - SUPABASE
            // =====================================================

            var storageClient =
                CriarStorageClient();

            var bucket =
                storageClient.From(_bucketName);

            var filePath =
                $"{folder}/{imageId}.img";

            try
            {
                await bucket.Remove(
                    new List<string>
                    {
                        filePath
                    });
            }
            catch
            {
                /*
                 * Não deixamos uma falha ao apagar
                 * a imagem quebrar o restante fluxo.
                 */
            }
        }

        // =========================================================
        // GET IMAGE URL
        // =========================================================

        /// <summary>
        /// Obtém o URL da imagem.
        /// </summary>
        public string GetImageUrl(
            Guid imageId,
            string folder,
            string placeholderName = "noimage")
        {
            if (imageId == Guid.Empty)
            {
                return
                    $"/images/{placeholderName}.png";
            }

            // =====================================================
            // DESENVOLVIMENTO
            // =====================================================

            if (_env.IsDevelopment())
            {
                var path =
                    Path.Combine(
                        _env.WebRootPath,
                        "images",
                        folder);

                if (Directory.Exists(path))
                {
                    var files =
                        Directory.GetFiles(
                            path,
                            $"{imageId}.*");

                    if (files.Length > 0)
                    {
                        var fileName =
                            Path.GetFileName(
                                files[0]);

                        return
                            $"/images/{folder}/{fileName}";
                    }
                }

                return
                    $"/images/{placeholderName}.png";
            }

            // =====================================================
            // PRODUÇÃO - SUPABASE
            // =====================================================

            return
                $"{_supabaseUrl}/storage/v1/object/public/" +
                $"{_bucketName}/{folder}/{imageId}.img";
        }

        // =========================================================
        // STORAGE CLIENT
        // =========================================================

        /// <summary>
        /// Cria o cliente do Supabase Storage.
        /// </summary>
        private Client CriarStorageClient()
        {
            return new Client(
                $"{_supabaseUrl}/storage/v1",
                new Dictionary<string, string>
                {
                    {
                        "Authorization",
                        $"Bearer {_supabaseKey}"
                    },
                    {
                        "apikey",
                        _supabaseKey
                    }
                });
        }

        // =========================================================
        // VALIDATION
        // =========================================================

        /// <summary>
        /// Valida formato e tamanho da imagem.
        /// </summary>
        private void ValidarImagem(
            IFormFile imageFile)
        {
            var extensoesPermitidas =
                new[]
                {
                    ".jpg",
                    ".jpeg",
                    ".png",
                    ".webp"
                };

            var extension =
                Path.GetExtension(
                    imageFile.FileName)
                    .ToLowerInvariant();

            if (!extensoesPermitidas
                .Contains(extension))
            {
                throw new InvalidOperationException(
                    "Formato de imagem inválido. " +
                    "Utilize JPG, JPEG, PNG ou WEBP.");
            }

            /*
             * Limite de 5 MB.
             */
            const long tamanhoMaximo =
                5 * 1024 * 1024;

            if (imageFile.Length >
                tamanhoMaximo)
            {
                throw new InvalidOperationException(
                    "A imagem não pode ultrapassar 5 MB.");
            }
        }

        // =========================================================
        // EXTENSION
        // =========================================================

        private string ObterExtensao(
            IFormFile imageFile)
        {
            var extension =
                Path.GetExtension(
                    imageFile.FileName)
                    .ToLowerInvariant();

            if (extension == ".jpeg")
            {
                return ".jpg";
            }

            return extension;
        }

        // =========================================================
        // CONTENT TYPE
        // =========================================================

        private string ObterContentType(
            IFormFile imageFile)
        {
            var extension =
                Path.GetExtension(
                    imageFile.FileName)
                    .ToLowerInvariant();

            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",

                _ => "application/octet-stream"
            };
        }
    }
}
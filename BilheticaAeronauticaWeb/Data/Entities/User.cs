using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{

    /// <summary>
    /// Representa um utilizador da aplicação, herdando as propriedades de IdentityUser para autenticação e segurança.
    /// Inclui informações pessoais adicionais como nome, apelido, documentos, data de nascimento e morada.
    /// Armazena referência à imagem de perfil no Blob Storage e colecção de bilhetes associados.
    /// </summary>
    public class User : IdentityUser
    {
        /// <summary>
        /// Primeiro nome introduzido pelo utilizador para personalização da conta.
        /// </summary>
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome não pode conter mais de 100 caracteres.")]
        public string Nome { get; set; }

        /// <summary>
        /// Sobrenome ou Apelido registado do utilizador da plataforma.
        /// </summary>
        [Required(ErrorMessage = "O apelido é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O apelido não pode exceder 100 caracteres.")]
        public string Apelido { get; set; }

        /// <summary>
        /// Endereço postal ou residencial completo do titular do utilizador.
        /// </summary>
        [MaxLength(200, ErrorMessage = "A morada fornecida não pode passar dos 200 caracteres.")]
        public string Endereço { get; set; }

        /// <summary>
        /// Campo de transporte ou redundância de password de segurança (usado habitualmente em migrações).
        /// </summary>
        [MaxLength(200)]
        public string Password { get; set; }

        /// <summary>
        /// Identificador único associado ao ficheiro de fotografia do utilizador no Azure Storage.
        /// </summary>
        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        /// <summary>
        /// URL completo do serviço na Cloud que expõe a fotografia de rosto/perfil do utilizador logado.
        /// </summary>
        public string ImageFullPath => ImageId == Guid.Empty
            ? $"/images/users/noimage.png"
            : $"https://bilhetica.blob.core.windows.net/users/{ImageId}.jpg";

        /// <summary>
        /// Junção instantânea em string do Nome e Apelido do utilizador sem mapear na tabela SQL.
        /// </summary>
        [NotMapped]
        public string FullName => $"{Nome} {Apelido}";

        /// <summary>
        /// Lista histórica completa de todas as compras de passagens aéreas efetuadas por este utilizador.
        /// </summary>
        public virtual ICollection<Bilhete> Bilhetes { get; set; } = new List<Bilhete>();

        /// <summary>
        /// Flag de controlo interno que especifica se o utilizador já alterou e definiu a sua senha permanente de acesso.
        /// </summary>
        public bool PasswordInicialDefinida { get; set; } = false;

    }

}

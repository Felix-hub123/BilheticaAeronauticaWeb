
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    /// <summary>
    /// Representa um passageiro do sistema, contendo dados pessoais,
    /// informações de contacto, imagem de perfil, data de registo,
    /// dados de identificação, e associação ao utilizador do sistema.
    /// Suporta soft delete para remoção lógica dos registos.
    /// </summary>
    public class Passageiro : IEntity
    {
        /// <summary>
        /// Identificador único do perfil de passageiro.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Primeiro e segundos nomes do passageiro.
        /// </summary>
        [Required(ErrorMessage = "O nome próprio do passageiro é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; }

        /// <summary>
        /// Propriedade utilitária que devolve a junção direta do Nome com o Apelido.
        /// </summary>
        public string FullName => $"{Nome} {Apelido}";

        /// <summary>
        /// Último nome ou apelidos de família do passageiro.
        /// </summary>
        [Required(ErrorMessage = "O apelido do passageiro é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O apelido não pode exceder 100 caracteres.")]
        public string Apelido { get; set; }

        /// <summary>
        /// Identificador único da fotografia de perfil do passageiro no contentor Azure Blob.
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// URL absoluto público para exibição da imagem de perfil associada ao passageiro.
        /// </summary>
        public string ImageFullPath => ImageId == Guid.Empty
             ? $"/images/users/noimage.png"
             : $"https://bilheticaapp.blob.core.windows.net/users/{ImageId}";

        /// <summary>
        /// Data cronológica em que o utilizador concluiu o registo de passageiro na plataforma.
        /// </summary>
        public DateTime DataRegisto { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Propriedade duplicada não mapeada que retorna o nome combinado do passageiro.
        /// </summary>
        [NotMapped]
        public string NomeCompleto => $"{Nome} {Apelido}";

        /// <summary>
        /// Chave estrangeira que conecta este perfil de passageiro a uma conta de segurança IdentityUser.
        /// </summary>
        [Required]
        public string UserId { get; set; }

        /// <summary>
        /// Instância do utilizador de autenticação ligado à conta deste passageiro.
        /// </summary>
        public User User { get; set; }

        /// <summary>
        /// Flag que indica se a conta do passageiro foi desativada ou eliminada logicamente (Soft Delete).
        /// </summary>
        public bool WasDeleted { get; set; }

        /// <summary>
        /// Tipo de documento legal fornecido (Ex: "Passaporte", "Cartão de Cidadão").
        /// </summary>
        [Required(ErrorMessage = "A especificação do tipo de documento é obrigatória.")]
        public string DocumentoIdentificacao { get; set; }

        /// <summary>
        /// Número de série identificador exclusivo presente no documento oficial indicado.
        /// </summary>
        [Required(ErrorMessage = "O número do documento oficial de identificação é obrigatório.")]
        public string NumeroDocumento { get; set; }

        /// <summary>
        /// Data de nascimento oficial do passageiro (relevante para apurar descontos de idade ou restrições legais).
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }
    }
}


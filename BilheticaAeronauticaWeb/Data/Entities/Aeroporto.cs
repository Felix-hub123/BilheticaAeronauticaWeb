using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    /// <summary>
    /// Representa um aeroporto no sistema, contendo dados de localização,
    /// código IATA, taxa aeroportuária padrão e imagem associada.
    /// </summary>
    public class Aeroporto : IEntity, ISoftDelete
    {
        /// <summary>
        /// Identificador único do aeroporto.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nome oficial do aeroporto.
        /// </summary>
        [Required(ErrorMessage = "O nome do aeroporto é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        /// <summary>
        /// Cidade onde o aeroporto está geograficamente localizado.
        /// </summary>
        [Required(ErrorMessage = "A cidade é obrigatória.")]
        [MaxLength(100, ErrorMessage = "A cidade não pode exceder 100 caracteres.")]
        public string Cidade { get; set; }

        /// <summary>
        /// País onde o aeroporto está localizado.
        /// </summary>
        [Required(ErrorMessage = "O país é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O país não pode exceder 100 caracteres.")]
        public string Pais { get; set; }

        /// <summary>
        /// Código de três letras da Associação Internacional de Transportes Aéreos (IATA).
        /// </summary>
        [Required(ErrorMessage = "O código IATA é obrigatório.")]
        [MaxLength(3, ErrorMessage = "O código IATA deve ter exatamente 3 caracteres.")]
        public string IATA { get; set; }

        /// <summary>
        /// Taxa aeroportuária padrão aplicada a descolagens/aterragens neste aeroporto.
        /// </summary>
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 99999999.99, ErrorMessage = "A taxa deve ser um valor positivo.")]
        public decimal TaxaAeroportoPadrao { get; set; }

        /// <summary>
        /// Identificador único da imagem de perfil do aeroporto no Blob Storage.
        /// </summary>
        public Guid ImageId { get; set; }

        /// <summary>
        /// Caminho completo ou URL para renderização da imagem do aeroporto.
        /// </summary>
        public string ImageFullPath => ImageId == Guid.Empty
             ? $"/images/aeroportos/noimage.png"
             : $"https://bilhetica.blob.core.windows.net/aeroportos/{ImageId}";

        /// <summary>
        /// Flag que indica se o registo foi logicamente eliminado (Soft Delete).
        /// </summary>
        public bool WasDeleted { get; set; }

        /// <summary>
        /// Propriedade lógica calculada que indica se o aeroporto já possui histórico de vínculos em voos.
        /// </summary>
        [NotMapped]
        public bool FoiUsadoEmVoos { get; set; }
    }

}


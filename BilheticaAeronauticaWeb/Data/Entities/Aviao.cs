using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    /// <summary>
    /// Representa um avião/aparelho da frota, com informações de marca, modelo,
    /// capacidade nas classes económica e executiva, imagem associada, estado de disponibilidade,
    /// lista de lugares e suporte a soft delete.
    /// </summary>
    public class Aviao : IEntity
    {
        /// <summary>
        /// Identificador único do avião.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fabricante/Marca da aeronave (Ex: Boeing, Airbus).
        /// </summary>
        [Required(ErrorMessage = "A marca do avião é obrigatória.")]
        [MaxLength(100, ErrorMessage = "A marca não pode exceder 100 caracteres.")]
        public string Marca { get; set; }

        /// <summary>
        /// Modelo comercial da aeronave (Ex: 737-800, A320neo).
        /// </summary>
        [Required(ErrorMessage = "O modelo do avião é obrigatório.")]
        [MaxLength(100, ErrorMessage = "O modelo não pode exceder 100 caracteres.")]
        public string Modelo { get; set; }

        /// <summary>
        /// Quantidade de assentos configurados para a classe económica.
        /// </summary>
        [Required(ErrorMessage = "A quantidade de lugares em classe económica é obrigatória.")]
        [Range(1, 500, ErrorMessage = "Os lugares em classe económica devem estar entre 1 e 500.")]
        public int LugaresEconomica { get; set; }

        /// <summary>
        /// Quantidade de assentos configurados para a classe executiva.
        /// </summary>
        [Required(ErrorMessage = "A quantidade de lugares em classe executiva é obrigatória.")]
        [Range(0, 100, ErrorMessage = "Os lugares em classe executiva devem estar entre 0 e 100.")]
        public int LugaresExecutiva { get; set; }

        /// <summary>
        /// Identificador único da imagem da aeronave no Blob Storage.
        /// </summary>
        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        /// <summary>
        /// Caminho completo ou URL para renderização da imagem da aeronave.
        /// </summary>
        public string ImageFullPath => ImageId == Guid.Empty
            ? $"/images/aviao/noimage.png"
            : $"https://bilhetica.blob.core.windows.net/avioes/{ImageId}";

        /// <summary>
        /// Indica se a aeronave se encontra operacional e disponível para escalas de voo.
        /// </summary>
        public bool Disponivel { get; set; } = true;

        /// <summary>
        /// Capacidade global de passageiros da aeronave (soma das classes).
        /// </summary>
        [NotMapped]
        public int Capacidade => LugaresEconomica + LugaresExecutiva;

        /// <summary>
        /// Coleção de assentos físicos estruturados pertencentes a este avião.
        /// </summary>
        public ICollection<Lugar> Lugares { get; set; } = new List<Lugar>();

        /// <summary>
        /// Flag que indica se o registo foi logicamente eliminado (Soft Delete).
        /// </summary>
        public bool WasDeleted { get; set; }
    }
}

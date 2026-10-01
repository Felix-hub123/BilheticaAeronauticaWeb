using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    /// <summary>
    /// Representa um avião/aparelho da frota.
    /// </summary>
    public class Aviao : IEntity
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A marca do avião é obrigatória.")]
        [MaxLength(
            100,
            ErrorMessage = "A marca não pode exceder 100 caracteres.")]
        public string Marca { get; set; }


        [Required(ErrorMessage = "O modelo do avião é obrigatório.")]
        [MaxLength(
            100,
            ErrorMessage = "O modelo não pode exceder 100 caracteres.")]
        public string Modelo { get; set; }

        [Required(
            ErrorMessage =
                "A quantidade de lugares em classe económica é obrigatória.")]
        [Range(
            1,
            500,
            ErrorMessage =
                "Os lugares em classe económica devem estar entre 1 e 500.")]
        public int LugaresEconomica { get; set; }

        [Required(
            ErrorMessage =
                "A quantidade de lugares em classe executiva é obrigatória.")]
        [Range(
            0,
            100,
            ErrorMessage =
                "Os lugares em classe executiva devem estar entre 0 e 100.")]
        public int LugaresExecutiva { get; set; }

        /// <summary>
        /// Identificador da imagem.
        /// </summary>
        [Display(Name = "Imagem")]
        public Guid ImageId { get; set; }

        /// <summary>
        /// Indica se o avião está disponível.
        /// </summary>
        public bool Disponivel { get; set; } = true;

        /// <summary>
        /// Capacidade total do avião.
        /// </summary>
        [NotMapped]
        public int Capacidade =>
            LugaresEconomica + LugaresExecutiva;

        /// <summary>
        /// Lugares associados ao avião.
        /// </summary>
        public ICollection<Lugar> Lugares { get; set; }
            = new List<Lugar>();

        /// <summary>
        /// Soft delete.
        /// </summary>
        public bool WasDeleted { get; set; }
    }
}
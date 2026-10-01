using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel para a entidade Aeroporto,
    /// incluindo suporte para upload e apresentação da imagem.
    /// </summary>
    public class AeroportosViewModel : Aeroporto
    {
        /// <summary>
        /// Ficheiro de imagem enviado pelo utilizador.
        /// </summary>
        [Display(Name = "Imagem")]
        public IFormFile ImageFile { get; set; }

        /// <summary>
        /// URL final da imagem usada nas Views.
        /// É preenchida pelo controller através do IImageHelper.
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
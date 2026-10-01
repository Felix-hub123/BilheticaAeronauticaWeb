using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel usado nas páginas dos aviões.
    /// </summary>
    public class AvioesViewModel : Aviao
    {
        /// <summary>
        /// Ficheiro recebido através do formulário.
        /// </summary>
        [Display(Name = "Imagem")]
        public IFormFile ImageFile { get; set; }

        /// <summary>
        /// URL final da imagem que será apresentada na View.
        /// Esta propriedade não é guardada na base de dados.
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
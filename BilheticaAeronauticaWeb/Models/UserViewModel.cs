using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel que estende a entidade User,
    /// incluindo suporte para upload e apresentação
    /// da imagem de perfil.
    /// </summary>
    public class UserViewModel : User
    {
        /// <summary>
        /// Ficheiro de imagem enviado pelo formulário.
        /// </summary>
        [Display(Name = "Imagem")]
        public IFormFile ImageFile { get; set; }

        /// <summary>
        /// URL final da imagem utilizada nas Views.
        /// É preenchida pelo controller através do IImageHelper.
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
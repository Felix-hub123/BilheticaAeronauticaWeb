using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel que estende a entidade <see cref="User"/>, incluindo suporte para upload de imagem de perfil.
    /// </summary>
    public class UserViewModel : User
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }



    }
}

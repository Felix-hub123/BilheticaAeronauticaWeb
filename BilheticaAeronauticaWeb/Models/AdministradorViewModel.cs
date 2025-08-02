using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel utilizado para operações relacionadas ao administrador,
    /// estende a entidade <see cref="User"/> e acrescenta suporte para upload de imagem.
    /// </summary>
    public class AdministradorViewModel : User
    {
        /// <summary>
        /// Ficheiro de imagem enviado via formulário, usado para alterar ou definir a foto do administrador.
        /// </summary>
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}

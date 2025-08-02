using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel para a entidade <see cref="Aeroporto"/>, incluindo suporte ao upload de imagem.
    /// </summary>
    public class AeroportosViewModel : Aeroporto
    {

        /// <summary>
        /// Ficheiro de imagem enviado pelo utilizador, usada para definir ou atualizar a imagem do aeroporto.
        /// </summary>
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}

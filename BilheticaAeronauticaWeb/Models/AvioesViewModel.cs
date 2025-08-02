using System.ComponentModel.DataAnnotations;
using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Http;


namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel para representar os dados de um avião, incluindo suporte para upload de imagem.
    /// </summary>
    public class AvioesViewModel : Aviao
    {
        /// <summary>
        /// Ficheiro de imagem enviado via formulário, utilizado para definir ou atualizar a imagem do avião.
        /// </summary>
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}

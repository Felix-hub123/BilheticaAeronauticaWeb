using System.ComponentModel.DataAnnotations;
using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Http;


namespace BilheticaAeronauticaWeb.Models
{
    public class AvioesViewModel : Aviao
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}

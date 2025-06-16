using BilheticaAeronauticaWeb.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    public class AeroportosViewModel : Aeroporto
    {
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
    }
}

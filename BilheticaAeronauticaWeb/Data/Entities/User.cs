using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class User : IdentityUser
    {

        [MaxLength(100)]
        public string Nome { get; set; }

        [MaxLength(100)]
        public string Apelido { get; set; }

        [MaxLength(200)]
        public string Endereço { get; set; }

        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? $"/images/aviao/noimage.png"
            : $"https://bilheticaaeronauticaapp.blob.core.windows.net/Users/{ImageId}";



        [NotMapped]
        public string FullName => $"{Nome} {Apelido}";

    


    }
  
}

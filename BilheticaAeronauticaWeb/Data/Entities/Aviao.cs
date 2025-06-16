using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Aviao : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Marca { get; set; }

        [Required, MaxLength(100)]
        public string Modelo { get; set; }

        [Required, Range(1, 500)]
        public int LugaresEconomica { get; set; }

        [Required, Range(0, 100)]
        public int LugaresExecutiva { get; set; }

        
        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? $"/images/aviao/noimage.png"
            : $"https://bilheticaaeronauticaapp.blob.core.windows.net/avioes/{ImageId}";
             



        public bool Disponivel { get; set; } = true;

       
        [NotMapped]
        public int Capacidade => LugaresEconomica + LugaresExecutiva;

       
      
        public ICollection<Lugar> Lugares { get; set; } = new List<Lugar>();

       
        public bool WasDeleted { get; set; }
    }
}

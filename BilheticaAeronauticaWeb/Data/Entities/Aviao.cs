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

        [Required]
        public Guid ImageId { get; set; }

        public bool Disponivel { get; set; }

        [NotMapped]
        public int Capacidade => LugaresEconomica + LugaresExecutiva;

        [NotMapped]
        public string ImageFullPath => ImageId == Guid.Empty
            ? "/images/noimage.png"
            : $"/images/aeronaves/{ImageId}";

        public ICollection<Voo> Voos { get; set; }
        public ICollection<Lugar> Lugares { get; set; }
        public bool WasDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

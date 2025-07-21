using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Aeroporto : IEntity
    {
        public int Id { get; set; }


        [Required, MaxLength(100)]
        public string Nome { get; set; }


        [Required, MaxLength(100)]
        public string Cidade { get; set; }

        [Required, MaxLength(100)]
        public string Pais { get; set; }

        [Required, MaxLength(3)]
        public string IATA { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TaxaAeroportoPadrao { get; set; }

        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
             ? $"/images/aeroportos/noimage.png"
             : $"https://bilheticaapp.blob.core.windows.net/aeroportos/{ImageId}";


        public bool WasDeleted { get; set; }
    }

}


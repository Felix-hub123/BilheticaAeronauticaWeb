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


        [NotMapped]
        public string BandeiraUrl => $"/images/bandeiras/{Pais.ToLower().Replace(" ", "_")}.png";

        [InverseProperty("Origem")]
        public ICollection<Voo> VoosOrigem { get; set; }

        [InverseProperty("Destino")]
        public ICollection<Voo> VoosDestino { get; set; }

        public bool WasDeleted { get; set; }
    }

}


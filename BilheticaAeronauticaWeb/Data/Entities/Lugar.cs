using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Lugar : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(1)]
        public string Fila { get; set; } 

        [Required]
        public int Numero { get; set; } 

        [Required, MaxLength(20)]
        public string Classe { get; set; } 

        public bool Disponivel { get; set; } = true;

        public int AeronaveId { get; set; }
        public Aviao Aviao { get; set; }

        public ICollection<Bilhete> Bilhetes { get; set; }

        [NotMapped]
        public string CodigoAssento => $"{Fila}{Numero}";

        public bool WasDeleted { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}

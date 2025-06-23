using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Lugar : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public string Codigo { get; set; } // Ex: 12A, 1B, etc.

        public bool Disponivel { get; set; } = true;

      
        public int AviaoId { get; set; }
        public Aviao Aviao { get; set; }

       
        public int? VooId { get; set; }
        public Voo Voo { get; set; }

        // Soft delete
        public bool WasDeleted { get; set; }
    }
}

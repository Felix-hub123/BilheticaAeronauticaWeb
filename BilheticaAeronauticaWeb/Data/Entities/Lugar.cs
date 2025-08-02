using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    /// <summary>
    /// Representa um lugar assento em um avião, com código identificador, preço base, disponibilidade,
    /// associação ao avião e opcionalmente a um voo específico.
    /// Inclui propriedade para soft delete para remoção lógica do registo.
    /// </summary>
    public class Lugar : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public string Codigo { get; set; } 

        public decimal PrecoBase { get; set; } = 100.00M; 

        public bool Disponivel { get; set; } = true;

      
        public int AviaoId { get; set; }
        public Aviao Aviao { get; set; }

       
        public int? VooId { get; set; }
        public Voo Voo { get; set; }

        // Soft delete
        public bool WasDeleted { get; set; }
    }
}

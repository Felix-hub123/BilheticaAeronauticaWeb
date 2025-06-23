using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Voo : IEntity
    {
        public int Id { get; set; }

        // FK para Aeroporto de Origem
        [Required]
        public int OrigemId { get; set; }
        public Aeroporto Origem { get; set; }

        // FK para Aeroporto de Destino
        [Required]
        public int DestinoId { get; set; }
        public Aeroporto Destino { get; set; }

        // FK para Aviao
        [Required]
        public int AviaoId { get; set; }
        public Aviao Aviao { get; set; }

        [Required]
        public DateTime DataHoraPartida { get; set; }

        [Required]
        public DateTime DataHoraChegada { get; set; }

        // Lugares associados ao voo
      //  public ICollection<Lugar> Lugares { get; set; } = new List<Lugar>();

        // Soft delete
        public bool WasDeleted { get; set; }
    }
   
}

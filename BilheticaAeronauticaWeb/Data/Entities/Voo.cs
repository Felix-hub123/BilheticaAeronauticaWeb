using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Voo : IEntity
    {
        public int Id { get; set; }

        public string Numero { get; set; }

      
        [Required]
        public int OrigemId { get; set; }
        public Aeroporto Origem { get; set; }

       
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

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoBase { get; set; }

        public ICollection<Lugar> Lugares { get; set; }



        // Soft delete
        public bool WasDeleted { get; set; }
    }
   
}

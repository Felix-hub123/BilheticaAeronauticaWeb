using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Voo : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public string NumeroVoo { get; set; }

        [Required]
        public DateTime DataHora { get; set; }

        public bool Ativo { get; set; } = true;

        [Required]
        public int OrigemId { get; set; }
        public Aeroporto Origem { get; set; }

        [Required]
        public int DestinoId { get; set; }
        public Aeroporto Destino { get; set; }

        [Required]
        public int AeronaveId { get; set; }
        public Aviao Aviao { get; set; }

        public ICollection<Bilhete> Bilhetes { get; set; }

        [NotMapped]
        public string DataVoo => DataHora.ToShortDateString();

        [NotMapped]
        public string HoraVoo => DataHora.ToString("HH:mm");

        public bool WasDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

}


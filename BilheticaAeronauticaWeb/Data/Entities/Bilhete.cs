using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Bilhete : IEntity
    {
        public int Id { get; set; }


        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal Tarifa { get; set; }

        [Required, MaxLength(20)]
        public string Classe { get; set; }

        public bool PodeAlterar { get; set; } = true;

        [Required]
        public DateTime DataCompra { get; set; } = DateTime.Now;

        [Required]
        public int VooId { get; set; }
        public Voo Voo { get; set; }

        [Required]
        public int LugarId { get; set; }
        public Lugar Lugar { get; set; }

        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        public int? PassageiroId { get; set; }
        public Passageiro Passageiro { get; set; }
        public bool WasDeleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}


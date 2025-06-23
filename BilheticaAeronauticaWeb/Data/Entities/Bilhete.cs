using System;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Bilhete : IEntity
    {
        public int Id { get; set; }

        // FK para Lugar
        [Required]
        public int LugarId { get; set; }
        public Lugar Lugar { get; set; }

        // FK para Passageiro
        [Required]
        public int PassageiroId { get; set; }
        public Passageiro Passageiro { get; set; }

        [Required]
        public DateTime DataCompra { get; set; } = DateTime.UtcNow;

        [Required]
        public decimal Preco { get; set; }

        // Soft delete
        public bool WasDeleted { get; set; }

    }
    
}

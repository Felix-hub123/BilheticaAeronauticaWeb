
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Passageiro : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nome { get; set; }

        public string FullName => $"{Nome} {Apelido}";

        [Required, MaxLength(100)]
        public string Apelido { get; set; }


        public DateTime DataRegisto { get; set; } = DateTime.UtcNow;

        // Histórico de bilhetes comprados pelo cliente

        [NotMapped]
        public string NomeCompleto => $"{Nome} {Apelido}";

        // Ligação ao utilizador autenticado (Identity)
      
        public string UserId { get; set; }
        public User User { get; set; }

        public bool WasDeleted { get; set; }
    }
}


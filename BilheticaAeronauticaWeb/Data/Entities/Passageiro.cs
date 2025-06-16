using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class Passageiro : IEntity
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nome { get; set; }

        [Required, MaxLength(100)]
        public string Apelido { get; set; }

        [Required, MaxLength(50)]
        public string DocumentoIdentificacao { get; set; } 

        [Required, MaxLength(50)]
        public string NumeroDocumento { get; set; }

        public DateTime DataRegisto { get; set; } = DateTime.UtcNow;

        // Histórico de bilhetes comprados pelo cliente

        [NotMapped]
        public string NomeCompleto => $"{Nome} {Apelido}";

        // Ligação ao utilizador autenticado (Identity)
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }

        public bool WasDeleted { get; set; }
    }
}


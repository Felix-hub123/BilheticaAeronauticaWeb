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

        public string DocumentoIdentificacao { get; set; }
        public string NumeroDocumento { get; set; }

        public ICollection<Bilhete> Bilhetes { get; set; }

        [NotMapped]
        public string NomeCompleto => $"{Nome} {Apelido}";

        public bool WasDeleted { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    }
}



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

        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
             ? $"/images/users/noimage.png"
             : $"https://bilheticaapp.blob.core.windows.net/users/{ImageId}";

        public DateTime DataRegisto { get; set; } = DateTime.UtcNow;

     

        [NotMapped]
        public string NomeCompleto => $"{Nome} {Apelido}";

    
      
        public string UserId { get; set; }
        public User User { get; set; }

        public bool WasDeleted { get; set; }

        public string DocumentoIdentificacao { get;  set; }

        public string NumeroDocumento { get;  set; }

        public DateTime? DataNascimento { get;  set; }
    }
}


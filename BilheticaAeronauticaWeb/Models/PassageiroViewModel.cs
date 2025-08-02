using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel usado para representar e validar os dados de um passageiro,
    /// incluindo informações pessoais, documentos de identificação e imagem opcional.
    /// </summary>
    public class PassageiroViewModel
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Nome { get; set; }

        [Required, MaxLength(100)]
        public string Apelido { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "Tipo de Documento")]
        public string DocumentoIdentificacao { get; set; }

        [Required, MaxLength(50)]
        [Display(Name = "Número do Documento")]
        public string NumeroDocumento { get; set; }

        [Display(Name = "Data de Nascimento")]
        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }


        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }
        public Guid ImageId { get;  set; }
    }
}

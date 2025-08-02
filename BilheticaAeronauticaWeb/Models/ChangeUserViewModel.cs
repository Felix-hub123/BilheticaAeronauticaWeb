using System;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    public class ChangeUserViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome")]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O apelido é obrigatório")]
        [Display(Name = "Apelido")]
        [MaxLength(100)]
        public string Apelido { get; set; }

        [Display(Name = "Endereço")]
        [MaxLength(200)]
        public string Endereco { get; set; }  

        [Phone(ErrorMessage = "Número de telefone inválido")]
        [Display(Name = "Telefone")]
        public string PhoneNumber { get; set; }

        // Caso queira permitir alterar a foto do perfil pelo Id
        [Display(Name = "Imagem")]
        public Guid ImageId { get; set; }
    }
}

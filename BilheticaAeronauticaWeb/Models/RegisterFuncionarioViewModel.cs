using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel para capturar os dados necessários no formulário de registo de novos utilizadores,
    /// incluindo email, password e confirmação da password.
    /// </summary>
    public class RegisterFuncionarioViewModel
    {


        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [Display(Name = "Apelido")]
        public string Apelido { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório.")]
        [EmailAddress(ErrorMessage = "Informe um email válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [MaxLength(20, ErrorMessage = "O campo {0} só pode ter {1} caracteres.")]
        [Phone(ErrorMessage = "Informe um número de telefone válido.")]
        [Display(Name = "Telefone")]
        public string PhoneNumber { get; set; }

       public IFormFile ImageFile { get; set; }


    }
}


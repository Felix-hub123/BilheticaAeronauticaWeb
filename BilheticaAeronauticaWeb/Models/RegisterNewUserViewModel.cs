using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SuperShop.Models
{
    /// <summary>
    /// ViewModel utilizado para captar os dados necessários para o registo de um novo utilizador,
    /// incluindo informações pessoais, contacto e credenciais de autenticação.
    /// </summary>
    public class RegisterNewUserViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O apelido é obrigatório")]
        [Display(Name = "Apelido")]
        public string Apelido { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [Display(Name = "Email")]
        public string Username { get; set; }

       
        [Required(ErrorMessage = "A palavra-passe é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Palavra-passe")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar palavra-passe")]
        [Compare("Password", ErrorMessage = "As palavras-passe não coincidem")]
        public string ConfirmPassword { get; set; }

        // Outros campos opcionais:
        [Phone]
        [Display(Name = "Telefone")]
        public string PhoneNumber { get; set; }


        [Display(Name = "Foto de Perfil")]
        public IFormFile ImageFile { get; set; }
    }
}

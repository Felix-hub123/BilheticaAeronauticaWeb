using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel utilizado para capturar os dados do formulário de login do utilizador,
    /// contendo as credenciais necessárias para autenticação.
    /// </summary>
    public class LoginViewModel
    {


        /// <summary>
        /// Password do utilizador para autenticação.
        /// Deve ter no mínimo 6 caracteres.
        /// </summary>

        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }


        /// <summary>
        /// Indica se o utilizador deseja manter a sessão iniciada ("lembrar-me").
        /// </summary>
        [Required]
        [DisplayName("Remember Me?")]
        public required bool RememberMe { get; set; }


        /// <summary>
        /// Endereço de email do utilizador.
        /// Deve estar num formato válido de email.
        /// </summary>
        [Required]

        [Display(Name = "Email")]

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;

namespace SuperShop.Models
{
    /// <summary>
    /// ViewModel utilizado para capturar os dados necessários para a redefinição da password de um utilizador,
    /// incluindo o identificador do utilizador, token de segurança, email, nova password e confirmação da nova password.
    /// </summary>
    public class ResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Palavra-passe")]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "As passwords não coincidem.")]
        [Display(Name = "Confirmar Palavra-passe")]
        public string ConfirmPassword { get; set; }

    }
}

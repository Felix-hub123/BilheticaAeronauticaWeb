using System.ComponentModel.DataAnnotations;

namespace SuperShop.Models
{
    /// <summary>
    /// ViewModel utilizado para capturar o endereço de email do utilizador
    /// que deseja recuperar a password via envio de link de redefinição.
    /// </summary>
    public class RecoverPasswordViewModel
    {

        /// <summary>
        /// Endereço de email do utilizador para envio do link de recuperação.
        /// Campo obrigatório e deve estar num formato válido de email.
        /// </summary>
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Por favor, insira um email válido.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}

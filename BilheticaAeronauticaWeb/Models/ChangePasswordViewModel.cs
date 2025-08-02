using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "A password atual é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Password atual")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "A nova password é obrigatória")]
        [DataType(DataType.Password)]
        [Display(Name = "Nova password")]
        [StringLength(100, ErrorMessage = "A {0} deverá ter pelo menos {2} caracteres.", MinimumLength = 6)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar nova password")]
        [Compare("NewPassword", ErrorMessage = "A nova password e a confirmação não coincidem")]
        public string ConfirmPassword { get; set; }
    }
}

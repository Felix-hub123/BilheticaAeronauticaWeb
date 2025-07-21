using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Models
{
    public class LoginViewModel
    {




        [Required]
        [MinLength(6)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }


        [Required]
        [DisplayName("Remember Me?")]
        public required bool RememberMe { get; set; }


        [Required]

        [Display(Name = "Email")]

        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

    }
}

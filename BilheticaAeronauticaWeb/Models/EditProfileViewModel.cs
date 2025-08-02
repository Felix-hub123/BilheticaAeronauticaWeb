using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace BilheticaAeronauticaWeb.Model
{
    /// <summary>
    /// ViewModel utilizado para editar o perfil do utilizador.
    /// Contém dados pessoais e dados para atualização da imagem de perfil.
    /// </summary>
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O apelido é obrigatório.")]
        [Display(Name = "Apelido")]
        public string Apelido { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Número de telefone inválido.")]
        [Display(Name = "Telemóvel")]
        public string PhoneNumber { get; set; }


        public Guid ImageId { get; set; }
        public IFormFile ImageFile { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
             ? $"/images/users/noimage.png"
             : $"https://bilhetica.blob.core.windows.net/users/{ImageId}";
    }
}


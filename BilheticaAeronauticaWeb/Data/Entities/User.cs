using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{

    /// <summary>
    /// Representa um utilizador da aplicação, herdando as propriedades de IdentityUser para autenticação e segurança.
    /// Inclui informações pessoais adicionais como nome, apelido, documentos, data de nascimento e morada.
    /// Armazena referência à imagem de perfil no Blob Storage e colecção de bilhetes associados.
    /// </summary>
    public class User : IdentityUser
    {
        

        [MaxLength(100)]
        public string Nome { get; set; }

        [MaxLength(100)]
        public string Apelido { get; set; }

        [MaxLength(200)]
        public string Endereço { get; set; }

        [MaxLength(200)]
        public string Password { get; set; }


        [Display(Name = "Image")]
        public Guid ImageId { get; set; }

        public string ImageFullPath => ImageId == Guid.Empty
            ? $"/images/users/noimage.png"
            : $"https://bilhetica.blob.core.windows.net/users/{ImageId}.jpg";



        [NotMapped]
        public string FullName => $"{Nome} {Apelido}";

        public virtual ICollection<Bilhete> Bilhetes { get; set; }

        public bool PasswordInicialDefinida { get; set; } = false;





    }

}

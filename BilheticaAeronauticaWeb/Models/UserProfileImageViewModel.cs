using System;

namespace BilheticaAeronauticaWeb.Models
{
    /// <summary>
    /// ViewModel que representa as informações essenciais para exibir a imagem e os dados do perfil do utilizador,
    /// incluindo estado de autenticação, nome completo, identificador da imagem, modo de exibição e caminho completo da imagem.
    /// </summary>
    public class UserProfileImageViewModel
    {
        public bool IsAuthenticated { get; set; } 
        public string FullName { get; set; }      
        public string DisplayMode { get; set; }     
        public string ImageFullPath { get; set; }



    }
}

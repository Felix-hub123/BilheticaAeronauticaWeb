using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BilheticaAeronauticaWeb.Data.Entities
{
    public class User : IdentityUser
    {
        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(200)]
        public string Address { get; set; }

        public string ProfilePhotoUrl { get; set; }

       
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

    


    }
  
}

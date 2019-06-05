using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SirmiumCommercial.Models.ViewModels
{
    public class CreateModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        [Required]
        public string CompanyName { get; set; }

        public DateTime RegistrationDate { get; set; } 

        public string Status { get; set; }

    }
    public class LoginModel
    {
        [Required]
        [UIHint("username")]
        public string UserName { get; set; }
        [Required]
        [UIHint("password")]
        public string Password { get; set; }
    }
}

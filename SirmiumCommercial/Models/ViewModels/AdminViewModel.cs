using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models;

namespace SirmiumCommercial.Models.ViewModels
{
    public class AdminViewModel
    {
        public string AdminId { get; set; }
        public AppUser User { get; set; }
        public IEnumerable<IdentityRole> Roles { get; set; }
        public IdentityRole Role { get; set; }
    }

    public class InviteByEmailViewModel
    {
        public string adminId { get; set; }
        public string email1 { get; set; }
        public string email2 { get; set; }
        public string email3 { get; set; }
        public string email4 { get; set; }
        public string email5 { get; set; }
    }

    public class CreateNewUser
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string RepeatPassword { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        [Required]
        public string CompanyName { get; set; }

        public string Status { get; set; }

        public string UserRoleId { get; set; }
        public IQueryable<IdentityRole> Roles { get; set; }
    }
}

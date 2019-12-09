using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models;

namespace SirmiumCommercial.Models.ViewModels
{
    public class AdminViewModel
    {
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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models;

namespace SirmiumCommercial.Models.ViewModels
{
    public class CompanyAdminViewModel
    {
        public IQueryable<AppUser> Users { get; set; }
        public AppUser CurrentUser { get; set; }
    }
    
    public class Profile
    {
        public AppUser appUser { get; set; }
        public UserProfile userProfile { get; set; }
    }
}

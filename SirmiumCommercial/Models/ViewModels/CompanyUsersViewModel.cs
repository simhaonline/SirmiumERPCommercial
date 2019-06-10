using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models;

namespace SirmiumCommercial.Models.ViewModels
{
    public class CompanyUsersViewModel
    {
        public IEnumerable<AppUser> Users { get; set; }
    }
}

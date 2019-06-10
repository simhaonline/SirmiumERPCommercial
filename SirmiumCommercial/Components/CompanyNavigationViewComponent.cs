using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Components
{
    public class CompanyNavigationViewComponent : ViewComponent
    {
        private UserManager<AppUser> userManager;

        public CompanyNavigationViewComponent(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }
        
        public IViewComponentResult Invoke(AppUser user)
        {
            if (user != null)
            {
                return View(user);
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(ModelState);
        }
    }
}

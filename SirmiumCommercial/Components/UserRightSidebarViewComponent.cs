using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Components
{
    public class UserRightSidebarViewComponent : ViewComponent
    {
        private UserManager<AppUser> userManager;

        public UserRightSidebarViewComponent (UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(string userId)
        {
            return View(userManager.Users.FirstOrDefault(u => u.Id == userId));
        }
    }
}

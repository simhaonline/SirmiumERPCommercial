using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Components
{
    public class UserLeftNavigationViewComponent : ViewComponent
    {
        private UserManager<AppUser> userManager;

        public UserLeftNavigationViewComponent(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            AppUser user = await userManager.FindByIdAsync(userId);
            LeftNavigationComponent model = new LeftNavigationComponent {
                UserId = user.Id
            };

            if (await userManager.IsInRoleAsync(user, "Admin"))
            {
                model.IsAdmin = true;
            }
            if (await userManager.IsInRoleAsync(user, "Manager"))
            {
                model.IsManager = true;
            }

            return View(model);
        }
    }
}

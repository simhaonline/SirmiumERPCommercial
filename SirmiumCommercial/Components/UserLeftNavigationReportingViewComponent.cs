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
    public class UserLeftNavigationReportingViewComponent : ViewComponent
    {
        private UserManager<AppUser> userManager;

        public UserLeftNavigationReportingViewComponent(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            AppUser user = await userManager.FindByIdAsync(userId);
            bool IsAdminOrManager = false;
            bool IsUser = false;

            if (await userManager.IsInRoleAsync(user, "Manager") ||
                await userManager.IsInRoleAsync(user, "Admin"))
            {
                IsAdminOrManager = true;
            }
            if (await userManager.IsInRoleAsync(user, "User"))
            {
                IsUser = true;
            }

            return View(new ReportingNavigationViewModel {
                IsAdminOrManager = IsAdminOrManager,
                IsUser = IsUser
            });
        }
    }
}

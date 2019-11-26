using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace SirmiumCommercial.Components
{
    public class GroupsAdminViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public GroupsAdminViewComponent(IAppDataRepository repo, 
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId)
        {
            bool isAdminOrManager = false;

            AppUser user = await userManager.FindByIdAsync(userId);

            if(await userManager.IsInRoleAsync(user, "Admin") || 
                await userManager.IsInRoleAsync(user, "Manager"))
            {
                isAdminOrManager = true;
            }

            return View(isAdminOrManager);
        }
    }
}

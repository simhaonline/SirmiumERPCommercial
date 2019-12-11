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
    public class VideoSettingsViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public VideoSettingsViewComponent (UserManager<AppUser> userMgr,
            IAppDataRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string userId,
            int videoId)
        {
            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == userId);

            if (await userManager.IsInRoleAsync(user, "Admin") == false 
                && await userManager.IsInRoleAsync(user, "Manager") == false)
            {
                return View(new VideoSettingsViewComponentViewModel {
                    CreatedById = id,
                    UserId = userId,
                    VideoId = videoId,
                    Show = true
                });
            }
            else
            {
                return View(new VideoSettingsViewComponentViewModel
                {
                    Show = false
                });
            }
        }
    }
}

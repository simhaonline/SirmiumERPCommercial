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
    public class RepresentationVideoViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public RepresentationVideoViewComponent(IAppDataRepository repo, 
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(string videoFor, int videoForId,
            string userId)
        {
            if (videoFor == "Representation")
            {
                AppUser user = userManager.Users
                    .FirstOrDefault(u => u.Id == userId);

                Presentation presentation = repository.Presentations
                    .FirstOrDefault(p => p.CreatedBy == user &&
                    p.Representations.Any(r => r.RepresentationId == videoForId));

                if (presentation != null)
                {
                    if (repository.Representations
                        .FirstOrDefault(r => r.RepresentationId == videoForId).Rating == 0)
                    {
                        return View(new RepreVideoComponentViewModel
                        {
                            IsRepresentationVideo = true,
                            representationId = videoForId,
                            userId = userId
                        });
                    }
                }
            }

            return View(new RepreVideoComponentViewModel
            {
                IsRepresentationVideo = false,
            });
        }
    }
}

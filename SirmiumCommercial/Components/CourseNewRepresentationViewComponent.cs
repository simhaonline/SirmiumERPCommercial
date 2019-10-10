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
    public class CourseNewRepresentationViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public CourseNewRepresentationViewComponent(IAppDataRepository repo,
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(int courseId, int presentationId, string userId)
        {
            bool userRepresentation = false;

            Presentation presentation = repository.Presentations
                .FirstOrDefault(p => p.PresentationId == presentationId);
            AppUser user = userManager.Users
                .FirstOrDefault(u => u.Id == userId);

            if(presentation.Representations
                .FirstOrDefault(r => r.CreatedBy == user) != null)
            {
                userRepresentation = true;
            }

            return View(new CourseNewRepresComponent {
                UserId = userId,
                CourseId = courseId,
                PresentationId = presentationId,
                UserRepresentation = userRepresentation
            });
        }
    }
}

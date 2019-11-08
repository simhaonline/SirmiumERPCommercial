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
    public class ReportingAllCoursesViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public ReportingAllCoursesViewComponent (IAppDataRepository repo,
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(string userId)
        {
            List<ReportingAllCourseVCViewModel> models = 
                new List<ReportingAllCourseVCViewModel>();

            foreach (CourseUsers cu in repository.CourseUsers)
            {
                if (cu.AppUserId == userId)
                {
                    int totalPArts = 0;
                    int completedParts = 0;

                    Course course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == cu.CourseId);
                    
                    foreach(Presentation presentation in course.Presentations)
                    {
                        totalPArts++;
                        foreach (Representation representation in
                            repository.Presentations
                                .FirstOrDefault(p => p.PresentationId == presentation.PresentationId)
                                .Representations)
                        {
                            AppUser representationUser = repository.Representations
                                    .Where(r => r.CreatedBy != null
                                        && r.RepresentationId == representation.RepresentationId)
                                    .Select(c => c.CreatedBy).FirstOrDefault();
                            if (representation.CreatedBy.Id == userId)
                            {
                                completedParts++;
                            }
                        }
                    }

                    ReportingAllCourseVCViewModel model = new ReportingAllCourseVCViewModel
                    {
                        CourseId = course.CourseId,
                        CourseTitle = course.Title,
                        CourseAward = course.AwardIcon,
                        TotalParts = totalPArts,
                        CompletedParts = completedParts
                    };
                    models.Add(model);
                }
            }

            return View(models.AsQueryable());
        }
    }
}

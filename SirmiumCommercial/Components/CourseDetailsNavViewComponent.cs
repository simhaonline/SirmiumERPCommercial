using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Components
{
    public class CourseDetailsNavViewComponent : ViewComponent
    {
        private IAppDataRepository repository;

        public CourseDetailsNavViewComponent(IAppDataRepository repo)
        {
            repository = repo;
        }

        public IViewComponentResult Invoke(int courseId, string userId)
        {
            bool userOnCourse = false;

            if(repository.CourseUsers.FirstOrDefault(cu => cu.CourseId == courseId
                    && cu.AppUserId == userId) != null)
            {
                userOnCourse = true;
            }

            return View(new CourseNavViewModel { CourseId = courseId,
                CurrentUserOnCourse = userOnCourse });
        }
    }
}

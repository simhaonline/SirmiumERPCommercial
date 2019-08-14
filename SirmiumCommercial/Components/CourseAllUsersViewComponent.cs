using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirmiumCommercial.Models;
using Microsoft.AspNetCore.Identity;

namespace SirmiumCommercial.Components
{
    public class CourseAllUsersViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public CourseAllUsersViewComponent(IAppDataRepository repo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(int courseId)
        {
            List<AppUser> users = new List<AppUser>();

            foreach(CourseUsers c in repository.CourseUsers
                .Where(cu => cu.CourseId == courseId))
            {
                users.Add(userManager.Users
                    .FirstOrDefault(u => u.Id == c.AppUserId));
            }

            return View(users.AsQueryable());
        }
    }
}

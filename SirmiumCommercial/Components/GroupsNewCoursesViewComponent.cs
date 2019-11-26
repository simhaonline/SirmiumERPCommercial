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
    public class GroupsNewCoursesViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public GroupsNewCoursesViewComponent(IAppDataRepository repo, 
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(int groupId)
        {
            Group group = repository.Groups.FirstOrDefault(g => g.GroupId == groupId);

            //courses
            List<Course> courses = new List<Course>();
            foreach (Course course in repository.Courses
                        .Where(c => c.CreatedBy.CompanyName == group.CreatedBy.CompanyName
                                    && c.Status == "Public"))
            {
                if(group.Courses
                    .FirstOrDefault(c => c.CourseId == course.CourseId) == null)
                {
                    courses.Add(course);
                }
            }

            return View(new NewGroupStep3ViewModel
            {
                Group = group,
                Courses = courses.AsQueryable()
            });
        }
    }
}

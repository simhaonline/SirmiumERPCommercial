using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Controllers
{
    public class CoursesController : Controller
    {
        private UserManager<AppUser> userManager;
        private IDetailsRepository repository;

        public CoursesController(UserManager<AppUser> userMgr, 
            IDetailsRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public IActionResult MyCourses(string id)
        {
            ViewData["Id"] = id;
            return View(repository.Courses);
        }

        public async Task<IActionResult> AllCourses(string id, string sort, string order)
        {
            ViewData["Id"] = id;
            ViewData["Sort"] = sort;
            ViewData["Order"] = order;
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                switch (sort)
                {
                    case "End Date":
                        return View(new AllCourse
                        {
                            Courses = (order == "asc") ?
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == ""
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderBy(c => c.EndDate) :
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == ""
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderByDescending(c => c.EndDate)
                        });
                    case "Date Added":
                        return View(new AllCourse
                        {
                            Courses = (order == "asc") ?
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == ""
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderBy(c => c.DateAdded) :
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == ""
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderByDescending(c => c.DateAdded)
                        });
                    default:
                        return View(new AllCourse
                        {
                            Courses = (order == "asc") ?
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == ""
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderBy(c => c.Title) :
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == ""
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderByDescending(c => c.Title)
                        });
                }
            }
            return View();
        }

        public IActionResult CourseDetails(string id, int courseId)
        {
            ViewData["Id"] = id;
            return View(new CourseViewModel
            {
                Course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == courseId),
                User = repository.Courses.Where(c => c.CreatedBy != null
                    && c.CourseId == courseId).Select(u => u.CreatedBy)
                    .FirstOrDefault()
            });
        }
    }
}

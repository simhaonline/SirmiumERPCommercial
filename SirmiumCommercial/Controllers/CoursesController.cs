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

        public async Task<IActionResult> AllCourses(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                ViewData["Id"] = user.Id;
                return View(new AllCourse
                {
                    Courses = repository.Courses
                });
            }
            return View();
        }

        public IActionResult CourseDetails(string id, string courseId)
        {
            ViewData["Id"] = id;
            int cId = Convert.ToInt32(courseId);
            return View(
                new CourseDetails
                {
                    Course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == cId)
                });   
        }
    }
}

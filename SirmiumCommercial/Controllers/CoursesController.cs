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

        public IActionResult MyCourses()
        {
            return View();
        }

        public async Task<IActionResult> AllCourses(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                ViewData["Id"] = user.Id;
                return View(new UserCourse
                {
                    User = user,
                    Courses = repository.Courses
                });
            }
            return View();
        }

        public IActionResult NewCourses()
        {
            return View();
        }
    }
}

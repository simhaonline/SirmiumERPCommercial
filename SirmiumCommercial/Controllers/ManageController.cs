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
    public class ManageController : Controller
    {
        private UserManager<AppUser> userManager;
        private IDetailsRepository repository;

        public ManageController(UserManager<AppUser> userMgr,
            IDetailsRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public IActionResult Index(string id)
        {
            ViewData["Id"] = id;
            return View(repository.Courses);
        }

        public async Task<IActionResult> NewCourse(string id)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                return View(new Course());
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewCourse(string id, Course model)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    model.Status = "private";
                    model.CreatedBy = user;
                    model.DateAdded = DateTime.Now;
                    model.DateModified = DateTime.Now;
                    repository.SaveCourse(model);
                    return RedirectToAction("Index", id);
                }
            }
            return View();
        }

        public IActionResult CourseManage(string id, int courseId)
        {
            ViewData["Id"] = id;
            return View(repository.Courses
                .FirstOrDefault(c => c.CourseId == courseId));
        }
        
    }
}

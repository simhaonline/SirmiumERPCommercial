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

        public IActionResult Index(string id, string status, string sort, string order)
        {
            ViewData["Id"] = id;
            ViewData["Status"] = status;
            ViewData["Sort"] = sort;
            ViewData["Order"] = order;
            if (status == null)
            {
                ViewData["Status"] = "Public";
                ViewData["Sort"] = "Title";
                ViewData["Order"] = "asc";
                return View(repository.Courses.Where(c => c.Status == "Public")
                        .OrderBy(c => c.Title));
            }
            else if (status == "All")
            {
                switch (sort)
                {
                    case "End Date":
                        return View((order == "asc") ?
                                repository.Courses.OrderBy(c => c.EndDate) :
                                repository.Courses.OrderByDescending(c => c.EndDate));
                    case "Date Added":
                        return View((order == "asc") ?
                                repository.Courses.OrderBy(c => c.DateAdded) :
                                repository.Courses.OrderByDescending(c => c.DateAdded));
                    default:
                        return View((order == "asc") ?
                                repository.Courses.OrderBy(c => c.Title) :
                                repository.Courses.OrderByDescending(c => c.Title));
                }
            }
            else
            {
                switch (sort)
                {
                    case "End Date":
                        return View((order == "asc") ?
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderBy(c => c.EndDate) :
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderByDescending(c => c.EndDate));
                    case "Date Added":
                            return View((order == "asc") ?
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderBy(c => c.DateAdded) :
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderByDescending(c => c.DateAdded));
                    default:
                        return View((order == "asc") ?
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderBy(c => c.Title) :
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderByDescending(c => c.Title));
                }
            }
            //TODO: if status == private
            //      select only course where course.createdBy.id == asp-route-id    
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
                    model.Status = "Private";
                    model.CreatedBy = user;
                    model.DateAdded = DateTime.Now;
                    model.DateModified = DateTime.Now;
                    model.Status = "Private";
                    repository.SaveCourse(model);
                    return RedirectToAction("Index", new { id, status = "Private",
                                                            sort = "Title", order = "asc"});
                }
            }
            return View();
        }

        public IActionResult CourseManage(string id, int courseId)
        {
            ViewData["Id"] = id;
            return View(new CourseViewModel {
                Course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == courseId),
                User = repository.Courses.Where(c => c.CreatedBy != null
                    && c.CourseId == courseId).Select(u => u.CreatedBy)
                    .FirstOrDefault()
            });
        }
        
        public IActionResult DeleteCourse(string id, int courseId)
        {
            ViewData["Id"] = id;
            Course deletedCourse = repository.DeleteCourse(courseId);
            if (deletedCourse != null)
            {
                TempData["message"] = $"Course: '{deletedCourse.Title}' was deleted!";
            }
            return RedirectToAction("Index", new{id, status = "Private",
                                                sort = "Title", order = "asc"});
        }
    }
}

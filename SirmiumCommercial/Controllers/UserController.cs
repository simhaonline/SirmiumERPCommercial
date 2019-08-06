using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Controllers
{
    public class UserController : Controller
    {
        private UserManager<AppUser> userManager;
        private IDetailsRepository repository;

        public UserController(UserManager<AppUser> userMgr,
                IDetailsRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public async Task<IActionResult> Index(string id)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                List<HomeViewModel> models = new List<HomeViewModel>();
                foreach (Course course in repository.Courses
                    .Where(c => c.CreatedBy.CompanyName == user.CompanyName
                    || c.CreatedBy.CompanyName == null))
                {
                    HomeViewModel content = new HomeViewModel
                    {
                        ContentId = course.CourseId,
                        CreatedBy = repository.Courses
                            .Where(c => c.CourseId == course.CourseId)
                            .Select(u => u.CreatedBy.UserName).FirstOrDefault(),
                        Title = course.Title,
                        DateAdded = course.DateAdded,
                        DateModified = course.DateModified,
                        EndDate = course.EndDate,
                        AwardIcon = course.AwardIcon,
                        VideoURL = course.VideoURL,
                        ContentType = "course"
                    };
                    models.Add(content);
                    foreach (Presentation presentation in course.Presentations)
                    {
                        content = new HomeViewModel
                        {
                            ContentId = course.CourseId,
                            CreatedBy = repository.Courses
                            .Where(c => c.CourseId == course.CourseId)
                            .Select(u => u.CreatedBy.UserName).FirstOrDefault(),
                            Title = presentation.Title,
                            DateAdded = presentation.DateAdded,
                            DateModified = presentation.DateModified,
                            EndDate = course.EndDate,
                            AwardIcon = "text-primary-2 fa fa-puzzle-piece",
                            VideoURL = presentation.VideoURL,
                            ContentType = "presentation"
                        };
                        models.Add(content);
                    }
                }
                return View(new HomeContent
                {
                    Content = models.AsQueryable()
                });
            }
            return View();
        }

        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult Messages()
        {
            return View();
        }

        public async Task<IActionResult> EditProfile(string id)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                ViewData["Id"] = user.Id;
                return View(new ProfileModel
                {
                    appUser = user
                }); ;
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(ModelState);
        }

        public IActionResult Settings()
        {
            return View();
        }

        public async Task<IActionResult> MyProfile(string id)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(new ProfileModel
                {
                    appUser = user
                });
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(ModelState);
        }

        public async Task<IActionResult> UserProfile(string id, string userId)
        {
            ViewData["Id"] = id;

            if(userId == id)
            {
                return RedirectToAction("MyProfile", new { id });
            }

            AppUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                ViewData["Title"] = user.FirstName + " " +
                    user.LastName + " Profile";
                return View(user);
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(ModelState);
        }
    }
}

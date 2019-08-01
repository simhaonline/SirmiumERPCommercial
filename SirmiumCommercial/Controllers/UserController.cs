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

        public UserController(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        public IActionResult Index(string id)
        {
            ViewData["Id"] = id;
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
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Controllers
{
    [Authorize (Roles = "Company Admin")]
    public class CompanyAdminController : Controller
    {
        private UserManager<AppUser> userManager;

        public CompanyAdminController(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        [Authorize(Roles = "Company Admin")]
        public ViewResult Index(AppUser user)
        {
                if (user != null)
                {
                    return View(user);
                }
                else
                {
                    ModelState.AddModelError("", "User Not Found");
                }
            return View(ModelState);
        }

        [Authorize(Roles = "Company Admin")]
        public async Task<IActionResult> Back(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return RedirectToAction("Index", user);
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(ModelState);
        }

        [Authorize(Roles = "Company Admin")]
        public async Task<IActionResult> MyProfile(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(ModelState);
        }

        [Authorize(Roles = "Company Admin")]
        public async Task<IActionResult> CompanyUsers (string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View( new CompanyAdminViewModel
                {
                    Users = userManager.Users
                        .Select(x => x)
                        .Where(x => x.CompanyName == user.CompanyName)
                        .OrderBy(x => x),
                    CurrentUser = user
                });
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(ModelState);
        }

        [Authorize(Roles = "Company Admin")]
        public async Task<IActionResult> EditMyProfile(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(ModelState);
        }

        [Authorize(Roles = "Company Admin")]
        public async Task<ViewResult> CapturePhoto(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
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

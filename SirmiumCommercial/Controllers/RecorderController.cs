using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Controllers
{
    public class RecorderController : Controller
    {
        private UserManager<AppUser> userManager;

        public RecorderController(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        public async Task<IActionResult> CapturePhoto(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);

            if(user != null)
            {
                ViewData["Id"] = user.Id;
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

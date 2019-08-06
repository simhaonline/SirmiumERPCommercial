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
    public class CompanyController : Controller
    {
        private UserManager<AppUser> userManager;

        public CompanyController(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        public async Task<ViewResult> AllUsers(string id)
        {
            ViewData["Id"] = id;
            AppUser currentUser = await userManager.FindByIdAsync(id);
            if(currentUser != null)
            {
                return View(userManager.Users
                    .Where(u => u.CompanyName == currentUser.CompanyName));
            }
            return View();
        }
    }
}

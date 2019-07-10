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
        private IUserRepository userRepository;

        public UserController(UserManager<AppUser> userMgr, IUserRepository userRepo)
        {
            userManager = userMgr;
            userRepository = userRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        /*public IActionResult MyProfile()
        {
            return View();
        }*/

        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult Messages()
        {
            return View();
        }

        public IActionResult EditProfile()
        {
            return View();
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
                    appUser = user,
                    userProfile = userRepository.Users
                        .FirstOrDefault(p => p.UserId == user.Id)
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

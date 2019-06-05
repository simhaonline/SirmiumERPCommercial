using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models;
using System.Threading.Tasks;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<AppUser> userManager;

        public AdminController(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        public ViewResult Index()
        {
            return View(userManager.Users);
        }

        public ViewResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    CompanyName = model.CompanyName,
                    PhoneNumber = model.PhoneNumber,
                    RegistrationDate = DateTime.Now,
                    Status = model.Status
                };
                IdentityResult result
                    = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ResultError(result);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string status)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                user.Status = status;
                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ResultError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ResultError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", userManager.Users);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatust(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                switch (user.Status)
                {
                    case "Active":
                        user.Status = "Inactive";
                        break;
                    case "Inactive":
                        user.Status = "Active";
                        break;
                    default:
                        user.Status = "Active";
                        break;
                }
                
                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ResultError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index");
        }

        private void ResultError(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}

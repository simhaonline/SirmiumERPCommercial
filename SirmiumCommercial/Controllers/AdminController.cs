﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models;
using System.Threading.Tasks;
using SirmiumCommercial.Models.ViewModels;
using SirmiumCommercial.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SirmiumCommercial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private UserManager<AppUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public AdminController(UserManager<AppUser> userMgr,
            RoleManager<IdentityRole> roleMgr)
        {
            userManager = userMgr;
            roleManager = roleMgr;
        }

        [Authorize(Roles = "Admin")]
        public async Task<ViewResult> Index(string id)
        {
            ViewData["Id"] = id;
            AppUser admin = await userManager.FindByIdAsync(id);
            if (admin != null)
            {
                ViewData["AdminCompany"] = admin.CompanyName;
                if (admin.CompanyName == null)
                {
                    return View(userManager.Users);
                }
                else
                {
                    return View("Index2", userManager.Users
                        .Where(u => u.CompanyName == admin.CompanyName));
                }
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ViewResult Create(string id)
        {
            ViewData["Id"] = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(string id, CreateModel model)
        {
            ViewData["Id"] = id;
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
                    return RedirectToAction("Index", id);
                }
                else
                {
                    ResultError(result);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", id);
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, string status)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                user.Status = status;
                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", id);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id, string deleteId)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(deleteId);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", id);
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
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatust(string id, string changeId)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(changeId);
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
                    return RedirectToAction("Details", new { id = id, detailsId = changeId });
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
            return View("Details", new { id = id, detailsId = changeId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Roles(string id)
        {
            ViewData["Id"] = id;
            AppUser admin = await userManager.FindByIdAsync(id);
            if(admin != null)
            {
                if (HttpContext.User.Identity.Name == admin.UserName)
                {
                    if (admin.CompanyName == null)
                    {
                        return View(roleManager.Roles);
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Account", new { returnUrl = "Login" });
                }
            }
            return RedirectToAction("Index", new { id });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole(string id)
        {
            ViewData["Id"] = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(string id, [Required]string name)
        {
            ViewData["Id"] = id;
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles", id);
                }
                else
                {
                    ResultError(result);
                }
            }
            return View(name);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id, string detailsId)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(detailsId);
            if (user != null)
            {
                return View(new AdminViewModel
                {
                    User = user,
                    Roles = roleManager.Roles
                });
            }
            else
            {
                return RedirectToAction("Index", id);
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditRole(AdminViewModel admin)
        {
            AppUser user = await userManager.FindByIdAsync(admin.User.Id);
            IdentityRole role = await roleManager.FindByIdAsync(admin.Role.Id);
            if (user != null)
            {
                if (role != null)
                {
                    IdentityResult result = await userManager.AddToRoleAsync(user, role.Name);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Details");
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
            }
            else
            {
                ModelState.AddModelError("", "Role Not Found");
            }
            return View("Details");
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

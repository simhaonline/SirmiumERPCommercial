using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SirmiumCommercial.Models.ViewModels;
using SirmiumCommercial.Models;
using Microsoft.AspNetCore.Identity;

namespace SirmiumCommercial.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<AppUser> userMgr,
            SignInManager<AppUser> signinMgr, RoleManager<IdentityRole> roleMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            roleManager = roleMgr;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel detalis, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(detalis.UserName);
                if (user != null)
                {
                    if (user.Status == "Active")
                    {
                        await signInManager.SignOutAsync();
                        Microsoft.AspNetCore.Identity.SignInResult result =
                            await signInManager.PasswordSignInAsync(
                                user, detalis.Password, false, false);
                        if (result.Succeeded)
                        {
                            //Redirect to AdminController, if user-role="Admin"
                            if (await userManager.IsInRoleAsync(user, "Admin"))
                            {
                                return Redirect("/Admin/Index");
                            }
                            return Redirect(returnUrl ?? "/");
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(LoginModel.UserName),
                                "Invalid password!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(LoginModel.UserName),
                            "The username '" + user.UserName + "' has not been activated or is blocked!");
                        return View(detalis);
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(LoginModel.UserName),
                        "Invalid username!");
                }
            }
            return View(detalis);
        }

        [AllowAnonymous]
        public ViewResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(CreateModel model)
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
                    PhoneNumber = (model.PhoneNumber != "") ? model.PhoneNumber : "",
                    RegistrationDate = DateTime.Now,
                    Status = "Inactive"
                };
                IdentityResult result
                    = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("SuccessSignUp");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View(model);
        }

        [AllowAnonymous]
        public ViewResult SuccessSignUp()
        {
            return View();
        }
    }
}

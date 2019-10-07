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
        private IAppDataRepository repository;

        public CompanyController(UserManager<AppUser> userMgr, IAppDataRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public async Task<ViewResult> AllUsers(string id)
        {
            ViewData["Id"] = id;
            AppUser currentUser = await userManager.FindByIdAsync(id);

            List<CompanyViewModel> users = new List<CompanyViewModel>();
            foreach (AppUser user in userManager.Users
                    .Where(u => u.CompanyName == currentUser.CompanyName))
            {
                //Awards

                //Rating

                //Views
                int totalViews = 0;
                foreach (Video video in repository.Videos.
                            Where(v => v.CreatedBy == user.Id))
                {
                    totalViews += video.Views;
                }

                CompanyViewModel cvm = new CompanyViewModel
                {
                    User = user,
                    Views = totalViews
                };
                users.Add(cvm);
            }

            if(currentUser != null)
            {
                return View(users.AsQueryable());
            }
            return View();
        }
    }
}

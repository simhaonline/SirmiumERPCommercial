using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace SirmiumCommercial.Components
{
    public class CommentViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public CommentViewComponent (IAppDataRepository repo, 
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(string For, int forId)
        {
            return View(new CommentViewModel {
                Comments = repository.Comments
                    .Where(c => c.For == For && c.ForId == forId)
                    .OrderBy(c => c.DateAdded),
                Users = userManager.Users
            });
        }
    }
}

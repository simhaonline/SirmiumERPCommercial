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
    public class GroupsNewUsersViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public GroupsNewUsersViewComponent(IAppDataRepository repo, 
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(string userId, int groupId)
        {
            Group group = repository.Groups.FirstOrDefault(g => g.GroupId == groupId);

            //users
            List<AppUser> users = new List<AppUser>();
            foreach (AppUser user in userManager.Users
                .Where(u => u.CompanyName == group.CreatedBy.CompanyName))
            {
                if(group.Users.FirstOrDefault(u => u.AppUserId == user.Id) == null)
                {
                    users.Add(user);
                }
            }

            return View(new NewGroupStep2ViewModel
            {
                Group = group,
                Users = users.AsQueryable()
            });
        }
    }
}

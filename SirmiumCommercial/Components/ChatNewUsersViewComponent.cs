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
    public class ChatNewUsersViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public ChatNewUsersViewComponent(IAppDataRepository repo, 
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(string userId, int chatId)
        {
            AppUser currentUser = userManager.Users.FirstOrDefault(u => u.Id == userId);
            GroupChat chat = repository.GroupChats.FirstOrDefault(c => c.ChatId == chatId);

            //users
            List<AppUser> users = new List<AppUser>();
            foreach (AppUser user in userManager.Users
                .Where(u => u.CompanyName == currentUser.CompanyName))
            {
                if(repository.GroupChatUsers.Any(u => u.UserId == user.Id && u.GroupChatId == chat.ChatId) == false
                    && user.Id != currentUser.Id)
                {
                    users.Add(user);
                }
            }

            return View(new NewGroupChatStep2ViewModel
            {
                GroupChatId = chatId,
                Users = users.AsQueryable()
            });
        }
    }
}

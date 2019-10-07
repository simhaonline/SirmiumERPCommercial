using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Components
{
    public class HeaderChatViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public HeaderChatViewComponent(IAppDataRepository repo,
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(string userId)
        {
            ViewData["Id"] = userId;
            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == userId);

            IQueryable<AppUser> users = userManager.Users
                .Where(u => u.CompanyName == user.CompanyName && u.Id != userId);

            List<IndexChat> chats = new List<IndexChat>();
            int totalNewMsg = 0;

            foreach (Chat chat in repository.Chats)
            {
                if (chat.User1Id == userId || chat.User2Id == userId)
                {
                    if (chat.Messages.LastOrDefault() != null)
                    {
                        int unseenMsg = chat.Messages
                            .Where(m => m.Seen == false && m.UserId != userId).Count();
                        totalNewMsg += unseenMsg;

                        IndexChat ci = new IndexChat
                        {
                            chat = chat,
                            LastMessage = chat.Messages.LastOrDefault(),
                            LastMessageDate = chat.Messages.LastOrDefault().DateAdded,
                            UnseenMsg = unseenMsg
                        };
                        chats.Add(ci);
                    }
                }
            }

            return View(new ChatViewModel
            {
                Chats = chats.AsQueryable().OrderByDescending(c => c.LastMessageDate).Take(7),
                TotalNewMessages = totalNewMsg,
                Users = users
            });
        }
    }
}

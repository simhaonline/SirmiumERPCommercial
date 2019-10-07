using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using SirmiumCommercial.Hubs;
using Microsoft.AspNetCore.Identity;

namespace SirmiumCommercial.Controllers
{
    public class ChatController : Controller
    {
        private UserManager<AppUser> userManager;
        private IAppDataRepository repository;

        public ChatController(UserManager<AppUser> userMgr, IAppDataRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public async Task<ViewResult> Index(string id)
        {
            ViewData["Id"] = id;

            AppUser user = await userManager.FindByIdAsync(id);

            IQueryable<AppUser> users = userManager.Users
                .Where(u => u.CompanyName == user.CompanyName && u.Id != user.Id);

            List<IndexChat> chats = new List<IndexChat>();
            foreach (Chat chat in repository.Chats)
            {
                if (chat.User1Id == id || chat.User2Id == id)
                {
                    if (chat.Messages.LastOrDefault() != null)
                    {
                        int unseenMsg = chat.Messages
                            .Where(m => m.Seen == false && m.UserId != id).Count();

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
                Chats = chats.AsQueryable().OrderByDescending(c => c.LastMessageDate),
                Users = users
            });
        }

        public IActionResult Chat(string id, string user2Id)
        {
            ViewData["Id"] = id;

            Chat chat1 = repository.Chats
                .FirstOrDefault(c => c.User1Id == id && c.User2Id == user2Id);
            Chat chat2 = repository.Chats
                .FirstOrDefault(c => c.User1Id == user2Id && c.User2Id == id);
            AppUser sender = userManager.Users
                .FirstOrDefault(u => u.Id == id);
            AppUser recipient = userManager.Users
                .FirstOrDefault(u => u.Id == user2Id);

            if (chat1 == null)
            {
                if (chat2 == null)
                {
                    return View(new NewChat
                    {
                        Sender = sender,
                        Recipient = recipient
                    });
                }
                else
                {
                    return View(new NewChat
                    {
                        Chat = chat2,
                        Sender = sender,
                        Recipient = recipient
                    });
                }
            }
            else
            {
                return View(new NewChat
                {
                    Chat = chat1,
                    Sender = sender,
                    Recipient = recipient
                });
            }
        }
    }
}

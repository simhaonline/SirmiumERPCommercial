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

        /*public IViewComponentResult Invoke(string userId)
        {
            ViewData["Id"] = userId;
            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == userId);

            IQueryable<AppUser> users = userManager.Users
                    .Where(u => u.CompanyName == user.CompanyName || 
                    u.CompanyName == null || u.CompanyName == "" && u.Id != userId);

            if (user.CompanyName == null || user.CompanyName == "")
            {
                users = userManager.Users
                .Where(u => u.Id != userId);
            }

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
        }*/
        public IViewComponentResult Invoke(string userId)
        {
            ViewData["Id"] = userId;
            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == userId);

            IQueryable<AppUser> users = userManager.Users
                    .Where(u => u.CompanyName == user.CompanyName ||
                    u.CompanyName == null || u.CompanyName == "" && u.Id != userId);

            if (user.CompanyName == null || user.CompanyName == "")
            {
                users = userManager.Users
                .Where(u => u.Id != userId);
            }

            List<HeaderChatViewModel> chats = new List<HeaderChatViewModel>();
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

                        HeaderChatViewModel ci = new HeaderChatViewModel
                        {
                            GroupChat = false,
                            chat = chat,
                            LastMessage = chat.Messages.LastOrDefault(),
                            LastMessageDate = chat.Messages.LastOrDefault().DateAdded,
                            UnseenMsg = unseenMsg
                        };
                        chats.Add(ci);
                    }
                }
            }

            foreach (GroupChat groupChat in repository.GroupChats.Where(g => g.Status == "Public"))
            {
                if (repository.GroupChatUsers.FirstOrDefault(g => g.GroupChatId == groupChat.ChatId
                            && g.UserId == user.Id) != null)
                {
                    GroupChatMessage lastMsg = repository.GroupChatMessages
                        .LastOrDefault(m => m.GroupChatId == groupChat.ChatId);
                    if (lastMsg != null)
                    {
                        int totalMsg = 0;
                        foreach (GroupChatMessage msg in repository.GroupChatMessages
                            .Where(m => m.GroupChatId == groupChat.ChatId))
                        {
                            if (msg.UserId != user.Id)
                            {
                                if (repository.GroupMessageViews.Any(m => m.MessageId == msg.MessageId
                                     && m.UserId == user.Id) == false)
                                {
                                    totalMsg++;
                                    totalNewMsg++;
                                }
                            }
                        }

                        HeaderChatViewModel ci = new HeaderChatViewModel
                        {
                            GroupChat = true,
                            GChat = groupChat,
                            LastGroupMessage = lastMsg,
                            LastMessageDate = lastMsg.DateAdded,
                            UnseenMsg = totalMsg
                        };
                        chats.Add(ci);
                    }
                }
            }

            return View(new FullChatViewModel
            {
                Chats = chats.AsQueryable().OrderByDescending(c => c.LastMessageDate).Take(7),
                TotalNewMessages = totalNewMsg,
                Users = users
            });
        }
    }
}

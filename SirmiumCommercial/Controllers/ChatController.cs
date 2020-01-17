using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using SirmiumCommercial.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace SirmiumCommercial.Controllers
{
    public class ChatController : Controller
    {
        private UserManager<AppUser> userManager;
        private IAppDataRepository repository;
        private IHostingEnvironment hostingEnvironment;

        public ChatController(UserManager<AppUser> userMgr, IAppDataRepository repo,
                 IHostingEnvironment hosting)
        {
            userManager = userMgr;
            repository = repo;
            hostingEnvironment = hosting;
        }

        /*public async Task<ViewResult> Index(string id)
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
        }*/
        public async Task<ViewResult> Index(string id)
        {
            ViewData["Id"] = id;

            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == id);

            IQueryable<AppUser> users = userManager.Users
                    .Where(u => u.CompanyName == user.CompanyName ||
                    u.CompanyName == null || u.CompanyName == "" && u.Id != id);

            if (user.CompanyName == null || user.CompanyName == "")
            {
                users = userManager.Users
                .Where(u => u.Id != id);
            }

            List<HeaderChatViewModel> chats = new List<HeaderChatViewModel>();
            int totalNewMsg = 0;

            foreach (Chat chat in repository.Chats)
            {
                if (chat.User1Id == id || chat.User2Id == id)
                {
                    if (chat.Messages.LastOrDefault() != null)
                    {
                        int unseenMsg = chat.Messages
                            .Where(m => m.Seen == false && m.UserId != id).Count();
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

            foreach (GroupChat groupChat in repository.GroupChats)
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
                Chats = chats.AsQueryable().OrderByDescending(c => c.LastMessageDate),
                TotalNewMessages = totalNewMsg,
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

        //--------Group Chat---------
        public ViewResult NewGroupChatStep1(string id)
        {
            ViewData["Id"] = id;

            return View();
        }

        [HttpPost]
        public IActionResult NewGroupChatStep1(NewGroupChatStep1ViewModel model)
        {
            ViewData["Id"] = model.CreatedBy;

            string name = (model.Name == null || model.Name.Trim() == "") ? "Group" : model.Name;

            GroupChat groupChat = new GroupChat
            {
                CreatedBy = model.CreatedBy,
                Title = model.Name
            };

            //create new chat
            repository.EditGroupChat(groupChat);

            if(model.Name == null || model.Name.Trim() == "")
            {
                groupChat.Title = "GroupChat" + groupChat.ChatId;
                repository.EditGroupChat(groupChat);
            }

            if(model.Photo != null)
            {
                AddGroupChatPhoto(groupChat.ChatId, model.Photo);
            }

            return RedirectToAction("NewGroupChatStep2", new
            {
                id = model.CreatedBy,
                groupChatId = groupChat.ChatId
            });
        }

        public ViewResult NewGroupChatStep2(string id, int groupChatId)
        {
            ViewData["Id"] = id;

            AppUser appUser = userManager.Users.FirstOrDefault(u => u.Id == id);

            return View(new NewGroupChatStep2ViewModel {
                UserId = id,
                GroupChatId = groupChatId,
                Users = userManager.Users.Where(u => u.CompanyName == appUser.CompanyName
                    && u.Id != id)
            });
        }

        [HttpPost]
        public IActionResult NewGroupChatStep2(NewGroupChatStep2ViewModel model)
        {
            if (model.UsersIds == null || model.UsersIds.Trim() == "")
            {
                return RedirectToAction("AbortNewGroupChat", new
                {
                    id = model.UserId,
                    groupChatId = model.GroupChatId
                });
            }
            else
            {
                repository.AddUserToGroupChat(model.GroupChatId, model.UserId);
                //list of users id
                string userIdList = model.UsersIds.Trim();
                while (userIdList.Trim().Length > 0)
                {
                    int index = userIdList.IndexOf(";");
                    if (index != -1)
                    {
                        string newUserId = userIdList.Substring(0, index);

                        repository.AddUserToGroupChat(model.GroupChatId, newUserId);

                        userIdList = userIdList.Replace(newUserId + ";", "");
                    }
                }

                return RedirectToAction("GroupChat", new
                {
                    id = model.UserId,
                    groupChatId = model.GroupChatId
                });
            }
        }

        public IActionResult AbortNewGroupChat(string id, int groupChatId)
        {
            ViewData["Id"] = id;

            repository.DeleteGroupChat(groupChatId);

            return RedirectToAction("Index", new { id });
        }

        public ViewResult GroupChat(string id, int groupChatId)
        {
            ViewData["Id"] = id;

            AppUser currentUser = userManager.Users
                .FirstOrDefault(u => u.Id == id);
            GroupChat chat = repository.GroupChats
                .FirstOrDefault(g => g.ChatId == groupChatId);

            IQueryable<GroupChatMessage> chatMessages = repository.GroupChatMessages
                .Where(g => g.GroupChatId == chat.ChatId);
            IQueryable<GroupMessageView> views = repository.GroupMessageViews
                .Where(g => g.UserId == currentUser.Id);
            List<GroupMsgInfo> allMsgs = new List<GroupMsgInfo>();
            List<int> msgIds = new List<int>();
            foreach (GroupChatMessage chatMessage in chatMessages)
            {
                if(chatMessage.UserId != currentUser.Id)
                {
                    if(views.Any(v => v.MessageId == chatMessage.MessageId) == false)
                    {
                        msgIds.Add(chatMessage.MessageId);
                    }
                }

                GroupMsgInfo msgInfo = new GroupMsgInfo
                {
                    Message = chatMessage,
                    Views = repository.GroupMessageViews.Where(v => v.MessageId == chatMessage.MessageId)
                };
                allMsgs.Add(msgInfo);
            }
            //repository.AddViewToGroupMessage(msgIds, currentUser.Id, chat.ChatId);

            //chat users
            List<AppUser> users = new List<AppUser>();
            foreach(GroupChatUsers chatUsers in repository.GroupChatUsers
                .Where(g => g.GroupChatId == groupChatId))
            {
                users.Add(userManager.Users.FirstOrDefault(u => u.Id == chatUsers.UserId));
            }



            return View(new GroupChatViewModel
            {
                CurrentUser = currentUser,
                Chat = chat,
                AllUsers = users.AsQueryable().OrderBy(u => u.LastName),
                AllMessages = allMsgs.AsQueryable()
            });
        }

        private void AddGroupChatPhoto(int groupChatId, IFormFile photo)
        {
            GroupChat groupChat = repository.GroupChats
                .FirstOrDefault(g => g.ChatId == groupChatId);

            //Create user directory
            string dirPath = Path.Combine(hostingEnvironment.WebRootPath, $@"UsersData\GroupChats\{groupChat.ChatId}");
            System.IO.Directory.CreateDirectory(dirPath);

            string fileName = "chatPhoto.jpeg";
            string filePath = Path.Combine(dirPath, fileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                photo.CopyTo(stream);
            }

            //add photo path to Group/GroupPhotoPath
            string photoPath = @"GroupChats/" + groupChat.ChatId + @"/chatPhoto.jpeg";
            groupChat.ChatPhotoPath = photoPath;
            repository.EditGroupChat(groupChat);
        }
    }
}

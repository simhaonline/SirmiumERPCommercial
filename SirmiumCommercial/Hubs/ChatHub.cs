using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace SirmiumCommercial.Hubs
{
    public class ChatHub : Hub
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public ChatHub(IAppDataRepository repo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public async Task NewMessage(string senderId, string recipientId, int chatId,
            string content)
        {
            AppUser sender = userManager.Users
                .FirstOrDefault(u => u.Id == senderId);
            string senderPhoto = (sender.ProfilePhotoUrl != null) ?
                $"/UsersData/{sender.ProfilePhotoUrl}" : "/defaultAvatar.png";
            string senderName = (sender.FirstName == null && sender.LastName == null) ?
                sender.UserName : $"{sender.FirstName} {sender.LastName}";

            AppUser recipient = userManager.Users
                .FirstOrDefault(u => u.Id == recipientId);
            string recipientPhoto = (recipient.ProfilePhotoUrl != null) ?
                $"/UsersData/{recipient.ProfilePhotoUrl}" : "/defaultAvatar.png";
            string recipientName = (recipient.FirstName == null && recipient.LastName == null) ?
                recipient.UserName : $"{recipient.FirstName} {recipient.LastName}";

            string date = DateTime.Now.ToString("HH:mm");
            int msgId;
            Chat chat;

            if (chatId == 0)
            {
                chat = new Chat
                {
                    User1Id = senderId,
                    User2Id = recipientId,
                    User1Checkpoint = DateTime.Now,
                    User2Checkpoint = DateTime.Now
                };
                repository.NewChat(chat);

                chat = repository.Chats
                    .FirstOrDefault(c => c.User1Id == senderId && c.User2Id == recipientId);

                ChatMessage msg = new ChatMessage
                {
                    UserId = senderId,
                    DateAdded = DateTime.Now,
                    MessageType = "Txt",
                    MessageContent = content,
                    Seen = false
                };
                repository.NewChatMessage(msg, chat);

                msgId = repository.Chats
                    .FirstOrDefault(c => c.ChatId == chat.ChatId)
                    .Messages.LastOrDefault(m => m.UserId == senderId).Id;
            }
            else
            {
                chat = repository.Chats
                    .FirstOrDefault(c => c.ChatId == chatId);

                ChatMessage msg = new ChatMessage
                {
                    UserId = senderId,
                    DateAdded = DateTime.Now,
                    MessageType = "Txt",
                    MessageContent = content,
                    Seen = false
                };
                repository.NewChatMessage(msg, chat);

                msgId = repository.Chats
                    .FirstOrDefault(c => c.ChatId == chatId)
                    .Messages.LastOrDefault(m => m.UserId == senderId).Id;
            }

            await Clients.All.SendAsync("PostNewMessage", msgId, recipientId,
                senderId, senderName, date, content);
            await Clients.All.SendAsync("ChatIndexNewMsg", senderId, recipientId,
                chat.ChatId, senderName, date, content,
                chat.Messages.Where(m => m.Seen == false).Count(), senderPhoto);

            await Clients.All.SendAsync("UserHeaderNewMsg", senderId, recipientId,
                chat.ChatId, senderName, date, content,
                chat.Messages.Where(m => m.Seen == false).Count(), senderPhoto,
                recipientPhoto, recipientName);
        }

        public async Task MessageSeen(int chatId, int msgId, string currentUserId)
        {
            Chat chat = repository.Chats
                .FirstOrDefault(c => c.ChatId == chatId);

            if (chat != null)
            {
                ChatMessage msg = chat.Messages
                    .FirstOrDefault(m => m.Id == msgId);
                repository.AddSeenChat(msg);

                await Clients.All.SendAsync("MsgSeen", chatId, msgId);
                await Clients.All.SendAsync("CheckTotalUnseenHeader",
                    chat.User1Id, chat.User2Id);
                await Clients.All.SendAsync("HeaderMsgSeen", chatId, currentUserId);
            }
        }

        public async Task TotalUnseenMsgHeader(string userId)
        {
            int totalUnseenMsg = 0;
            foreach (Chat chat in repository.Chats)
            {
                if (chat.User1Id == userId || chat.User2Id == userId)
                {
                    totalUnseenMsg += chat.Messages
                        .Where(m => m.Seen == false && m.UserId != userId).Count();
                }
            }

            foreach(GroupChatUsers groupChat in repository.GroupChatUsers.Where(g => g.UserId == userId))
            {
                foreach(GroupChatMessage msg in repository.GroupChatMessages.Where(m => m.GroupChatId == groupChat.GroupChatId))
                {
                    if(repository.GroupMessageViews.Any(v => v.UserId == userId && v.MessageId == msg.MessageId) == false
                        && msg.UserId != userId)
                    {
                        totalUnseenMsg++;
                    }
                }
            }

            await Clients.All.SendAsync("PostTotalUnseenHeader", userId, totalUnseenMsg);
        }

        public async Task DeleteMessage(int msgId, int chatId)
        {
            string currentUserId = repository.ChatMessages.
                FirstOrDefault(m => m.Id == msgId).UserId;

            repository.DeleteChatMessage(msgId);

            Chat chat = repository.Chats.FirstOrDefault(c => c.ChatId == chatId);

            //if lastMsg not exists
            if (chat.Messages == null)
            {
                //Remove this chat from Index-Chat and Header-chat

            }
            else
            {
                //retrun new lastMsg
                ChatMessage lastMsg = chat.Messages.LastOrDefault();

                string senderId = lastMsg.UserId;
                AppUser sender = userManager.Users
                    .FirstOrDefault(u => u.Id == senderId);
                string senderPhoto = (sender.ProfilePhotoUrl != null) ?
                    $"/UsersData/{sender.ProfilePhotoUrl}" : "/defaultAvatar.png";
                string senderName = (sender.FirstName == null && sender.LastName == null) ?
                    sender.UserName : $"{sender.FirstName} {sender.LastName}";

                string recipientId = (chat.User1Id == senderId) ? chat.User2Id :
                    chat.User1Id;
                AppUser recipient = userManager.Users
                    .FirstOrDefault(u => u.Id == recipientId);
                string recipientPhoto = (recipient.ProfilePhotoUrl != null) ?
                    $"/UsersData/{recipient.ProfilePhotoUrl}" : "/defaultAvatar.png";
                string recipientName = (recipient.FirstName == null && recipient.LastName == null) ?
                    recipient.UserName : $"{recipient.FirstName} {recipient.LastName}";

                string date = MsgSentDate(lastMsg.DateAdded);

                await Clients.All.SendAsync("ChatIndexNewLastMsg", senderId, recipientId,
                    chat.ChatId, senderName, date, lastMsg.MessageContent,
                    chat.Messages.Where(m => m.Seen == false).Count(), senderPhoto);

                await Clients.All.SendAsync("UserHeaderNewLastMsg", senderId, recipientId,
                    chat.ChatId, senderName, date, lastMsg.MessageContent,
                    chat.Messages.Where(m => m.Seen == false).Count(), senderPhoto,
                    recipientPhoto, recipientName);
            }

            //delete msg from chat view
            await Clients.All.SendAsync("DeleteMsgChat", msgId, chatId);
        }

        public string MsgSentDate(DateTime dateAdded)
        {
            var dateDiff = DateTime.Now - dateAdded;
            string val = "";

            int days = dateDiff.Days;
            int hours = dateDiff.Hours;
            int minutes = dateDiff.Minutes;

            //years
            if (days / 365 > 0)
            {
                val = (days / 365 == 1) ? "1 year ago" : days / 365 + " years ago";
            }
            else if (days / 30 > 0)
            {
                //Months
                val = (days / 30 == 1) ? "1 month ago" : days / 30 + " months ago";
            }
            else if (days > 0)
            {
                //Days
                val = (days == 1) ? "1 day ago" : days + " days ago";
            }
            else
            {
                val = dateAdded.ToString("HH:mm");
            }

            return val;
        }

        public async Task NewGroupMessage(string userId, int chatId, string content, string msgType = "")
        {
            AppUser sender = userManager.Users
                .FirstOrDefault(u => u.Id == userId);
            string senderPhoto = (sender.ProfilePhotoUrl != null) ?
                $"/UsersData/{sender.ProfilePhotoUrl}" : "/defaultAvatar.png";
            string senderName = (sender.FirstName == null && sender.LastName == null) ?
                sender.UserName : $"{sender.FirstName} {sender.LastName}";

            GroupChat groupChat = repository.GroupChats.FirstOrDefault(c => c.ChatId == chatId);
            string chatPhoto = (groupChat.ChatPhotoPath != null) ?
                $"/UsersData/{groupChat.ChatPhotoPath}" : "/defaultGroup.png";

            if (groupChat != null)
            {
                GroupChatMessage newMsg = new GroupChatMessage
                {
                    GroupChatId = groupChat.ChatId,
                    UserId = sender.Id,
                    MessageContent = content,
                    MessageType = msgType
                };

                repository.NewGroupChatMessage(newMsg, groupChat);

                await Clients.All.SendAsync("PostNewGroupMessage", newMsg.MessageId, groupChat.ChatId,
                    sender.Id, senderName, DateTime.Now.ToShortTimeString(), content, senderPhoto, msgType);

                foreach (GroupChatUsers chatUser in repository.GroupChatUsers
                    .Where(g => g.GroupChatId == groupChat.ChatId))
                {
                    int userUnseenMsgs = 0;
                    foreach (GroupChatMessage msg in repository.GroupChatMessages
                        .Where(m => m.GroupChatId == groupChat.ChatId))
                    {
                        if (msg.UserId != chatUser.UserId)
                        {
                            if (repository.GroupMessageViews.Any(m => m.MessageId == msg.MessageId
                                 && m.UserId == chatUser.UserId) == false)
                            {
                                userUnseenMsgs++;
                            }
                        }
                    }

                    await Clients.All.SendAsync("UserHeaderNewGroupMsg", sender.Id, chatUser.UserId,
                        groupChat.ChatId, senderName, DateTime.Now.ToShortTimeString(), content,
                        userUnseenMsgs, chatPhoto, groupChat.Title);
                    await Clients.All.SendAsync("ChatIndexNewGroupMsg", sender.Id, chatUser.UserId,
                        groupChat.ChatId, senderName, DateTime.Now.ToShortTimeString(), content,
                        userUnseenMsgs, chatPhoto, groupChat.Title);
                }
            }
        }

        public async Task DeleteGroupMessage(int msgId, int chatId)
        {
            repository.DeleteGroupChatMessage(msgId);

            await Clients.All.SendAsync("DeleteGroupMsgChat", msgId, chatId);

            GroupChat chat = repository.GroupChats.FirstOrDefault(c => c.ChatId == chatId);
            string chatPhoto = (chat.ChatPhotoPath != null) ?
                $"/UsersData/{chat.ChatPhotoPath}" : "/defaultGroup.png";
            GroupChatMessage lastMsg = repository.GroupChatMessages.LastOrDefault(m => m.GroupChatId == chatId);
            AppUser sender = userManager.Users.FirstOrDefault(u => u.Id == lastMsg.UserId);
            string senderPhoto = (sender.ProfilePhotoUrl != null) ?
                $"/UsersData/{sender.ProfilePhotoUrl}" : "/defaultAvatar.png";
            string senderName = (sender.FirstName == null && sender.LastName == null) ?
                sender.UserName : $"{sender.FirstName} {sender.LastName}";

            foreach (GroupChatUsers user in repository.GroupChatUsers.Where(u => u.GroupChatId == chatId))
            {
                int userUnseenMsgs = 0;
                foreach (GroupChatMessage msg in repository.GroupChatMessages
                    .Where(m => m.GroupChatId == chat.ChatId))
                {
                    if (msg.UserId != user.UserId)
                    {
                        if (repository.GroupMessageViews.Any(m => m.MessageId == msg.MessageId
                             && m.UserId == user.UserId) == false)
                        {
                            userUnseenMsgs++;
                        }
                    }
                }

                await Clients.All.SendAsync("ChatIndexNewLastGroupMsg", sender.Id, user.UserId,
                        chat.ChatId, senderName, DateTime.Now.ToShortTimeString(), lastMsg.MessageContent,
                        userUnseenMsgs, chatPhoto, chat.Title);

                await Clients.All.SendAsync("UserHeaderNewLastGroupMsg", sender.Id, user.UserId,
                        chat.ChatId, senderName, DateTime.Now.ToShortTimeString(), lastMsg.MessageContent,
                        userUnseenMsgs, chatPhoto, chat.Title);
            }
        }

        public async Task GroupMessageSeen(int chatId, int msgId, string currentUserId)
        {
            GroupChat chat = repository.GroupChats.FirstOrDefault(g => g.ChatId == chatId);
            AppUser currentUser = userManager.Users.FirstOrDefault(u => u.Id == currentUserId);
            string userName = (currentUser.FirstName == null || currentUser.LastName == null) ?
                currentUser.UserName : currentUser.FirstName + " " + currentUser.LastName;
            string userPhoto = (currentUser.ProfilePhotoUrl != null) ?
                $"/UsersData/{currentUser.ProfilePhotoUrl}" : "/defaultAvatar.png";

            GroupMessageView view = new GroupMessageView
            {
                UserId = currentUser.Id,
                MessageId = msgId
            };
            repository.AddViewToGroupMsg(view);

            await Clients.All.SendAsync("GroupMsgSeen", chatId, msgId, userName, userPhoto);

            await Clients.All.SendAsync("CheckTotalUnseenHeader", currentUserId, currentUserId);

            await Clients.All.SendAsync("HeaderGroupMsgSeen", chatId, currentUserId);
        }

        public async Task GroupChatNewUsers(string currentUserId, int chatId, string userIds)
        {
            if(userIds != null && userIds.Trim() != "")
            {
                string userIdList = userIds.Trim();
                while (userIdList.Trim().Length > 0)
                {
                    int index = userIdList.IndexOf(";");
                    if (index != -1)
                    {
                        string newUserId = userIdList.Substring(0, index);

                        repository.AddUserToGroupChat(chatId, newUserId);

                        userIdList = userIdList.Replace(newUserId + ";", "");

                        //create system msg
                        AppUser admin = userManager.Users.FirstOrDefault(u => u.Id == currentUserId);
                        string adminName = (admin.FirstName == null || admin.LastName == null) ?
                            admin.UserName : $"{admin.FirstName} {admin.LastName}";
                        AppUser user = userManager.Users.FirstOrDefault(u => u.Id == newUserId);
                        string userName = (user.FirstName == null && user.LastName == null) ?
                            user.UserName : $"{user.FirstName} {user.LastName}";
                        string userPhoto = (user.ProfilePhotoUrl != null) ?
                                 $"/UsersData/{user.ProfilePhotoUrl}" : "/defaultAvatar.png";
                        string msgContent = $"{admin} added {userName}";
                        await NewGroupMessage(currentUserId, chatId, msgContent, "SystemMsg");

                        await Clients.All.SendAsync("AddedUsersList", chatId, user.Id, userName, userPhoto);
                    }
                }
            }
        }

        public async Task GroupChatAddNewPhoto(string userId, int chatId)
        {
            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == userId);
            GroupChat chat = repository.GroupChats.FirstOrDefault(c => c.ChatId == chatId);

            string msg = ((user.FirstName == null && user.LastName == null) ?
                user.UserName : $"{user.FirstName} {user.LastName}") + " change chat photo";

            await NewGroupMessage(userId, chatId, msg, "SystemMsg");
            await Clients.All.SendAsync("ChatAddPhoto", chatId, chat.ChatPhotoPath);

        }

        public async Task GroupChatRemoveUser(int chatId, string userId)
        {
            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == userId);
            GroupChat chat = repository.GroupChats.FirstOrDefault(c => c.ChatId == chatId);
            AppUser chatAdmin = userManager.Users.FirstOrDefault(u => u.Id == chat.CreatedBy);

            GroupChatUsers chatUser = repository.GroupChatUsers.FirstOrDefault(cu => cu.GroupChatId == chatId 
                                                            && cu.UserId == userId);
            repository.RemoveUserFromGroupChat(chatId, user.Id);

            string msg = ((user.FirstName == null && user.LastName == null) ?
                user.UserName : $"{user.FirstName} {user.LastName}") + " has left the conversation";

            await NewGroupMessage(chatAdmin.Id, chatId, msg, "SystemMsg");

            await Clients.All.SendAsync("DeleteChatFromHeader", chatId, userId);
            await Clients.All.SendAsync("DeleteChatFromIndex", chatId, userId);
            await Clients.All.SendAsync("DeleteUserFromList", chatId, userId);

            await Clients.All.SendAsync("ExitChat", chatId, userId);
        }

        public async Task DeleteGroupChat(int chatId)
        {
            GroupChat chat = repository.GroupChats.FirstOrDefault(c => c.ChatId == chatId);

            IQueryable<string> chatUsers = repository.GroupChatUsers.Where(cu => cu.GroupChatId == chatId).Select(c => c.UserId);

            repository.DeleteGroupChat(chatId);

            await Clients.All.SendAsync("DeleteGroupChat-DeleteChatFromHeader", chatId);
            await Clients.All.SendAsync("DeleteGroupChat-DeleteChatFromIndex", chatId);
            await Clients.All.SendAsync("DeleteGroupChat-ExitChat", chatId);
        }
    }
}


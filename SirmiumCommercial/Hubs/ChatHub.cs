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

        public ChatHub (IAppDataRepository repo, UserManager<AppUser> userMgr)
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
                    totalUnseenMsg  += chat.Messages
                        .Where(m => m.Seen == false && m.UserId != userId).Count();
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
    }
}


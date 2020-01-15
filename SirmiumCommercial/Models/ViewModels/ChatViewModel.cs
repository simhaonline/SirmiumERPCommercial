using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SirmiumCommercial.Models.ViewModels
{
    public class ChatViewModel
    {
        public IQueryable<IndexChat> Chats { get; set; }
        public int TotalNewMessages { get; set; }
        public IQueryable<AppUser> Users { get; set; }
    }

    public class FullChatViewModel
    {
        public IQueryable<HeaderChatViewModel> Chats { get; set; }
        public int TotalNewMessages { get; set; }
        public IQueryable<AppUser> Users { get; set; }
    }

    public class HeaderChatViewModel
    {
        //for gorupChat
        public bool GroupChat { get; set; }
        public GroupChat GChat { get; set; }
        public GroupChatMessage LastGroupMessage { get; set; }

        //for chat
        public Chat chat { get; set; }
        public ChatMessage LastMessage { get; set; }
        public int UnseenMsg { get; set; }


        public DateTime LastMessageDate { get; set; }
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

    public class IndexChat
    {
        public Chat chat { get; set; }
        public ChatMessage LastMessage { get; set; }
        public DateTime LastMessageDate { get; set; }
        public int UnseenMsg { get; set; }

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

    public class NewChat
    {
        public Chat Chat { get; set; }
        public AppUser Sender { get; set; }
        public AppUser Recipient { get; set; }

        public string MsgSentDate(DateTime dateAdded)
        {
            var dateDiff = DateTime.Now - dateAdded;
            string val = "";

            int days = dateDiff.Days;

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

    public class NewGroupChatStep1ViewModel
    {
        public string CreatedBy { get; set; }
        public string Name { get; set; }
        public IFormFile Photo { get; set; }
    }

    public class NewGroupChatStep2ViewModel
    {
        public string UserId { get; set; }
        public int GroupChatId { get; set; }
        public IQueryable<AppUser> Users { get; set; }
        public string UsersIds { get; set; }
    }

    public class GroupChatViewModel
    {
        public AppUser CurrentUser { get; set; }
        public GroupChat Chat { get; set; }
        public IQueryable<AppUser> AllUsers { get; set; }
        public IQueryable<GroupChatMessage> AllMessages { get; set; }

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

        public string MessageNewLine(string msg)
        {
            int msgLength = msg.Length;

            if (msgLength > 100)
            {
                //first part of msg
                string substr1 = msg.Substring(0, 100);
                int indexOfBr = substr1.IndexOf("</br>");

                if (indexOfBr == -1)
                {
                    //msg tail
                    string substr2 = msg.Substring(100);

                    return $"{substr1}</br>" + MessageNewLine(substr2);
                }
                else
                {
                    substr1 = msg.Substring(0, indexOfBr + 6);
                    string substr2 = msg.Substring(indexOfBr + 6);

                    return substr1 + MessageNewLine(substr2);
                }
            }
            return msg;
        }
    }
}


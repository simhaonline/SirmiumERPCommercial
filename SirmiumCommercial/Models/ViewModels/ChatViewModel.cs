using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class ChatViewModel
    {
        public IQueryable<IndexChat> Chats { get; set; }
        public int TotalNewMessages { get; set; }
        public IQueryable<AppUser> Users { get; set; }
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
}

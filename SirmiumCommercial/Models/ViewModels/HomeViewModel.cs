using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class HomeViewModel
    {
        public int ContentId { get; set; }
        public string Title { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedById { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime EndDate { get; set; }
        public string AwardIcon { get; set; }
        public string VideoURL { get; set; }
        public string ContentType { get; set; }
    }

    public class HomeContent
    {
        public IQueryable<HomeViewModel> Content { get; set; }
        public string DateDifference(DateTime date1, DateTime date2)
        {
            var dateDiff = date1 - date2;
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
            else if (hours > 0)
            {
                //hours
                val = (hours == 1) ? "1 hour ago" : hours + " hours ago";
            }
            else if (minutes > 0)
            {
                //Minutes
                val = (minutes == 1) ? "1 minute ago" : minutes + " minutes ago";
            }
            else
            {
                val = "few seconds ago";
            }

            return val;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class HomeViewModel
    {
        public int CourseId { get; set; }
        public int ContentId { get; set; }
        public string Title { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime EndDate { get; set; }
        public string AwardIcon { get; set; }
        public Video Video { get; set; }

        //video likes & dislikes
        public IQueryable<AppUser> Likes { get; set; }
        public IQueryable<AppUser> Dislikes { get; set; }

        //presentation, course, representation
        public string ContentType { get; set; }

        public double CourseAverageRating { get; set; }

        public double PresentationAverageRating { get; set; }

        public int RepresentationRating { get; set; }

        public IQueryable<AppUser> UsersOnCourse { get; set; }
    }

    /*
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
    }*/

    public class HomeContent
    {
        public IQueryable<HomeViewModel> Content { get; set; }
        public string DateDifference(DateTime date1)
        {
            DateTime currentDateTime = DateTime.Now;

            if (date1 == DateTime.MinValue)
            {
                return "<strong class='text-success'>NO END DATE</strong>";
            }
            else if (date1 < currentDateTime)
            {
                return "<strong class='text-danger' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}'>FINISHED</strong>";
            }

            var dateDiff = date1 - currentDateTime;
            string val = "";

            int days = dateDiff.Days;
            int hours = dateDiff.Hours;
            int minutes = dateDiff.Minutes;

            //years
            if (days / 365 > 0)
            {
                val = (days / 365 == 1) ? "<strong class='text-info' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}' >1 year</strong>" :
                    $"<strong class='text-info' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}' >{days / 365} years</strong>";
            }
            else if (days / 30 > 0)
            {
                //Months
                val = (days / 30 == 1) ? "<strong class='text-info' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}' > 1 month</strong>" :
                    $"<strong class='text-info' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}' > {days / 30} months</strong>";
            }
            else if (days > 0)
            {
                //Days
                val = (days == 1) ? "<strong class='text-danger' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}' > 1 day</strong>" :
                    $"<strong class='text-warning' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}' > {days} days</strong>";
            }
            else if (hours > 0)
            {
                //hours
                val = (hours == 1) ? "<strong class='text-danger' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}' > 1 hour</strong>" :
                    $"<strong class='text-danger' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}' > {hours} days</strong>";
            }
            else if (minutes > 0)
            {
                //Minutes
                val = (minutes == 1) ? "<strong class='text-danger' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}' > 1 minute</strong>" :
                    $"<strong class='text-danger' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}' > {minutes} minutes</strong>";
            }
            else
            {
                val = "<strong class='text-danger' data-toggle='tooltip' data-placement='bottom' " +
                    $"title='{date1.ToString("dd.mm.yyyy. HH:mm")}' > few seconds</strong>";
            }

            return val;
        }
    }
}

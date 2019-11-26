using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class GroupViewModel
    {
    }

    public class NewGroupViewModel
    {
        public string CreatedById { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class NewGroupStep2ViewModel
    {
        public Group Group { get; set; }
        public IQueryable<AppUser> Users { get; set; }

        public int GroupId { get; set; }
        public string UserId { get; set; }
        public string UsersIdString { get; set; }
    }
    public class NewGroupStep3ViewModel
    {
        public Group Group { get; set; }
        public IQueryable<Course> Courses { get; set; }

        public int GroupId { get; set; }
        public string UserId { get; set; }
        public string CoursesIdString { get; set; }
    }

    public class GroupDetailsViewModel
    {
        public Group Group { get; set; }
        public IQueryable<AppUser> Users { get; set; }
        public IQueryable<GroupCourseDetails> Courses { get; set; }

        public string userPhoto(string path)
        {
            return (path == null) ? "/defaultAvatar.png" : $"/UsersData/{path}";
        }

        public string firstLastNameExists(AppUser user)
        {
            return (user.FirstName == null || user.LastName == null) ?
                user.UserName : user.FirstName + " " + user.LastName;
        }

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

    public class GroupCourseDetails
    {
        public Course Course { get; set; }
        public IQueryable<AppUser> CourseUsers { get; set; }
        public Video Video { get; set; }
    }
}

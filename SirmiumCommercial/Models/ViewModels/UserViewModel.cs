using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace SirmiumCommercial.Models.ViewModels
{
    public class CreateModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string RepeatPassword { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        [Required]
        public string CompanyName { get; set; }

        public DateTime RegistrationDate { get; set; } 

        public string Status { get; set; }

        public string Remark { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [UIHint("username")]
        public string UserName { get; set; }
        [Required]
        [UIHint("password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ProfileModel
    {
        public AppUser appUser { get; set; }
        public IFormFile ProfilePhoto { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<IdentityError> errors { get; set; }
    }

    public class SettingsViewModel
    {
        public AppUser User { get; set; }

        public string UserId { get; set; }
        [Required]
        public string CurrentPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }

    public class AllCourses
    {
        public Course Course { get; set; }
        public Video Video { get; set; }
        public IQueryable<AppUser> UsersOnCourse { get; set; }
        public double CourseAverageRating { get; set; }

        //video likes & dislikes
        public IQueryable<AppUser> Likes { get; set; }
        public IQueryable<AppUser> Dislikes { get; set; }
    }

    public class AllCoursesViewModel
    {
        public IQueryable<AllCourses> Content { get; set; }

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

        public string firstLastNameExists(AppUser user)
        {
            return (user.FirstName == null || user.LastName == null) ?
                user.UserName : user.FirstName + " " + user.LastName;
        }

        public string userPhoto(string path)
        {
            return (path == null) ? "/defaultAvatar.png" : $"/UsersData/{path}";
        }

        public string rating(double avgRating)
        {
            var returnStr = "";

            if (avgRating == 0)
            {
                returnStr = "<strong class='text-muted' >Average Rating: </strong>";
                returnStr += "<strong style='color: #868686'>Not Rated Yet </strong>";
            }
            else
            {
                returnStr += "<strong class='text-muted'>Average Rating: </strong>";
                for (var i = 0; i < Math.Round(avgRating, MidpointRounding.ToEven); i++)
                {
                    returnStr += "<span class='fa fa-star text-warning'></span>";
                }
                if (avgRating < 5)
                {
                    for (var i = Math.Round(avgRating, MidpointRounding.ToEven); i < 5; i++)
                    {
                        returnStr += "<span class='fa fa-star text-muted'></span>";
                    }
                }
                returnStr += $"<span>&emsp; {String.Format("{0:0.0}", avgRating)} / 5.0 </span>";
            }

            return returnStr;
        }
    }

    public class CourseDetails
    {
        public Course Course { get; set; }
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

    public class MyCompanyUsers
    {
        public AppUser User { get; set; }
        public Course Course { get; set; }
    }

    public class ChangeProfilePhoto
    {
        public string userId { get; set; }
        public string photoUrl { get; set; }
    }

    public class UploadPhoto
    {
        public string UserId { get; set; }
        public IFormFile ProfilePhoto { get; set; }
    }

    public class VideoModel
    {
        public int Id { get; set; }
        public string Title { get; set; }

        //Course, Presentation, Representation
        public string For { get; set; }
        public int ForId { get; set; }
        public string UserId { get; set; }
        public string videoUrl { get; set; }
        public string returnUrl { get; set; }
    }

    public class CommentViewModel
    {
        public IQueryable<Comment> Comments { get; set; }
        public IQueryable<AppUser> Users { get; set; }
        public string CommentDateAdded(DateTime dateAdded, DateTime currentDate)
        {
            var dateDiff = dateAdded - currentDate;
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
                val = (minutes < 10) ? "Few minutes ago" : minutes + " minutes ago";
            }
            else
            {
                val = "Just now";
            }

            return val;
        }
    }

    public class LeftNavigationComponent {
        public string UserId { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsManager { get; set; }
    }
}

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
    }

    public class AllCourse
    {
        public IQueryable<Course> Courses { get; set; }
        public IQueryable<CourseUsers> Users { get; set; }
        public IQueryable<Video> Videos { get; set; }
        public string DateDifference (DateTime date1, DateTime date2)
        {
            var dateDiff = date1 - date2;
            string val = "";

            int days = dateDiff.Days;
            int hours = dateDiff.Hours;
            int minutes = dateDiff.Minutes;

            //years
            if (days/365 > 0)
            {
                val = (days / 365 == 1) ? "1 year ago" : days / 365 + " years ago";
            }
            else if(days/30 > 0)
            {
                //Months
                val = (days / 30 == 1) ? "1 month ago" : days / 30 + " months ago";
            } 
            else if(days > 0)
            {
                //Days
                val = (days == 1) ? "1 day ago" : days + " days ago";
            }
            else if(hours > 0)
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

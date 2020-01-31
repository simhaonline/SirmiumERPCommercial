using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class CourseViewModel
    {
        public Course Course { get; set; }
        public AppUser User { get; set; }
        public IQueryable<AppUser> UsersOnCourse { get; set; }
        public IQueryable<Video> Videos { get; set; }
        public IQueryable<Comment> Comments { get; set; }
        public IQueryable<AppUser> CommentUserInfo { get; set; }

        //For CourseManage
        public CourseManagePresentations AllPresentations { get; set; }

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

    public class CourseManagePresentations
    {
        public IQueryable<Presentation> Presentations { get; set; }
        public IQueryable<Video> Videos { get; set; }
        public CoursePresentationsPageInfo PageInfo { get; set; }

        public int CourseId { get; set; }
        public string UserId { get; set; }
    }

    public class CoursePresentationsPageInfo
    {
        public int TotalPresentations { get; set; }
        public int PresentationsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalPresentations / PresentationsPerPage);
    }

    public class CourseNavViewModel
    {
        public int CourseId { get; set; }
        public bool CurrentUserOnCourse { get; set; }
    }

    public class NewEditCourse
    {
        public Course Course { get; set; }
        public bool NoEndDate { get; set; }
        public Video Video { get; set; }

        //Edit
        public int CourseId { get; set; }
        public string UserId { get; set; }
        public string NewTitle { get; set; }
        public DateTime NewEndDate { get; set; }
        public bool NewNoEndDate { get; set; }
        public string NewDescription { get; set; }
        public string NewAwardIcon { get; set; }
        public string VideoTitle { get; set; }
        public int VideoId { get; set; }
    }

    public class NewCourseStep2ViewModel
    {
        public int CourseId { get; set; }
        public string VideoTitle { get; set; }
        public string UserId { get; set; }
        public string videoUrl { get; set; }

        public string TitlePlaceholder { get; set; }
    }

    public class NewEditPresentation
    {
        public Presentation Presentation { get; set; }
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public int PresentationPart { get; set; }
    }

    public class NewRepresentation
    {
        public int CourseId { get; set; }
        public int PresentationId { get; set; }
        public string RepresentationTitle { get; set; }
        public string VideoTitle { get; set; }
        public string UserId { get; set; }
        public string videoUrl { get; set; }

        public string TitlePlaceholder { get; set; }
    }

    public class CourseNewRepresComponent
    {
        public int CourseId { get; set; }
        public int PresentationId { get; set; }
        public string UserId { get; set; }
        public bool UserRepresentation { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class ReportingNavigationViewModel
    {
        public bool IsUser { get; set; }
        public bool IsAdminOrManager { get; set; }
    }

    public class CourseReportingViewModel
    {
        public Course Course { get; set; }
        public IQueryable<UserRepresentationsInfo> UsersOnCourse { get; set; }
    }

    public class UserRepresentationsInfo
    {
        public AppUser User { get; set; }
        public IQueryable<Representation> Representations { get; set; }
    }

    //My progress models
    public class MyProgressViewModel
    {
        public AppUser User { get; set; }
        public IQueryable<MyProgressCourseInfo> Courses { get; set; }
        public int Views { get; set; }
        public int Representations { get; set; }
        public double AvgRating { get; set; }

        public string IsPresentationCompleted(double rating)
        {
            return (rating > 0) ? "fa fa-check text-success" : "fa fa-close text-danger";
        }

        public string Rating(double rating)
        {
            var returnStr = "";

            if (rating == 0)
            {
                returnStr += "<strong style='color: #868686'>Not Rated Yet </strong>";
            }
            else
            {
                for (var i = 0; i < Math.Round(rating, MidpointRounding.ToEven); i++)
                {
                    returnStr += "<span class='fa fa-star text-warning'></span>";
                }
                if (rating < 5)
                {
                    for (var i = Math.Round(rating, MidpointRounding.ToEven); i < 5; i++)
                    {
                        returnStr += "<span class='fa fa-star text-muted'></span>";
                    }
                }
                returnStr += $"<span>&emsp; {String.Format("{0:0.0}", rating)} / 5.0 </span>";
            }

            return returnStr;
        }
    }

    public class MyProgressCourseInfo
    {
        public Course Course { get; set; }
        public IQueryable<MyProgressPresentationsInfo> Presentations { get; set; }

        public int CompletedPresentations { get; set; }
        public double AvgRating { get; set; }
    }

    public class MyProgressPresentationsInfo
    {
        public Presentation Presentation { get; set; }
        public double Rating { get; set; } = 0;
    }
}

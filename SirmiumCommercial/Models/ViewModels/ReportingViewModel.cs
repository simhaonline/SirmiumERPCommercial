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

    //course reports
    public class CourseReportsPageViewModel
    {
        public IQueryable<CourseReportingViewModel> ViewModel { get; set; }
        public CourseReportsPageInfo PageInfo { get; set; }

        public string UserRating(double avgRating)
        {
            var returnStr = "";

            if (avgRating == 0)
            {
                returnStr += "<strong style='color: #868686'>Not Rated Yet </strong>";
            }
            else
            {
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

        public string UserProgress(double totalPresentation, double userReprese)
        {
            if (totalPresentation == 0)
            {
                return "No Presentation Yet";
            }
            else
            {
                double progress = 100 / totalPresentation * userReprese;
                return progress.ToString("0.0") + "%";
            }
        }

        public string userPhoto(string path)
        {
            return (path == null) ? "/defaultAvatar.png" : $"/UsersData/{path}";
        }
    }

    public class CourseReportingViewModel
    {
        public Course Course { get; set; }
        public IQueryable<UserRepresentationsInfo> UsersOnCourse { get; set; }
    }

    public class UserRepresentationsInfo
    {
        public AppUser User { get; set; }
        public double AvgRating { get; set; }
        public int TotalRepresentations { get; set; }
    }

    public class CourseReportsPageInfo
    {
        public int TotalCourses { get; set; }
        public int CoursesPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalCourses / CoursesPerPage);
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

        public string userPhoto(string path)
        {
            return (path == null) ? "/defaultAvatar.png" : $"/UsersData/{path}";
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


    //presentation reports
    public class PresentationPageViewModel
    {
        public Course Course { get; set; }
        public IQueryable<PresentationReportsInfo> Presentations { get; set; }
        public PresentationReportsPageInfo PageInfo { get; set; }

        public string UserRating(double rating)
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

        public string IsPresentationCompleted(double rating)
        {
            return (rating > 0) ? "fa fa-check text-success" : "fa fa-close text-danger";
        }

        public string userPhoto(string path)
        {
            return (path == null) ? "/defaultAvatar.png" : $"/UsersData/{path}";
        }
    }

    public class PresentationReportsInfo
    {
        public Presentation Presentation { get; set; }
        public IQueryable<PresentationUserInfo> Users { get; set; }
    }

    public class PresentationUserInfo
    {
        public AppUser User { get; set; }
        public double Rating { get; set; }
    }

    public class PresentationReportsPageInfo
    {
        public int TotalPresentations { get; set; }
        public int PresentationsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalPresentations / PresentationsPerPage);
    }

    //group reports
    public class GroupCourseReportsPageViewModel
    {
        public IQueryable<CourseReportingViewModel> ViewModel { get; set; }
        public Group Group { get; set; }
        public CourseReportsPageInfo PageInfo { get; set; }

        public string UserRating(double avgRating)
        {
            var returnStr = "";

            if (avgRating == 0)
            {
                returnStr += "<strong style='color: #868686'>Not Rated Yet </strong>";
            }
            else
            {
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

        public string UserProgress(double totalPresentation, double userReprese)
        {
            if (totalPresentation == 0)
            {
                return "No Presentation Yet";
            }
            else
            {
                double progress = 100 / totalPresentation * userReprese;
                return progress.ToString("0.0") + "%";
            }
        }

        public string userPhoto(string path)
        {
            return (path == null) ? "/defaultAvatar.png" : $"/UsersData/{path}";
        }

        public string groupPhoto(string path)
        {
            return (path == null) ? "/defaultGroup.png" : $"/UsersData/Groups/{path}";
        }
    }

    public class GroupReportsPageViewModel
    {
        public IQueryable<GroupReportsInfo> GroupInfo { get; set; }
        public AllGroupPageInfo PageInfo { get; set; }


        public string groupPhoto(string path)
        {
            return (path == null) ? "/defaultGroup.png" : $"/UsersData/Groups/{path}";
        }

        public string UserRating(double avgRating)
        {
            var returnStr = "";

            if (avgRating == 0)
            {
                returnStr += "<strong style='color: #868686'>Not Rated Yet </strong>";
            }
            else
            {
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

        public string UserProgress(double totalPresentation, double userReprese)
        {
            if (totalPresentation == 0)
            {
                return "No Presentation Yet";
            }
            else
            {
                double progress = 100 / totalPresentation * userReprese;
                return progress.ToString("0.0") + "%";
            }
        }

        public string userPhoto(string path)
        {
            return (path == null) ? "/defaultAvatar.png" : $"/UsersData/{path}";
        }
    }

    public class GroupReportsInfo
    {
        public Group Group { get; set; }
        public IQueryable<GroupUsersInfo> UsersInfos { get; set; }
    }

    public class GroupUsersInfo
    {
        public AppUser User { get; set; }
        public double AvgRating { get; set; }
        public int TotalRepresentations { get; set; }
        public int TotalPresentations { get; set; }
    }

    public class AllGroupPageInfo
    {
        public int TotalGroups { get; set; }
        public int GroupsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalGroups / GroupsPerPage);
    }
}

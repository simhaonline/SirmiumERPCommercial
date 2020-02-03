using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class SearchViewModel
    {
        public string KeyWord { get; set; }
        public SearchAllUsers Users { get; set; }
        public SearchAllCourses Courses { get; set; }
        public SearchAllPresentation Presentations { get; set; }
        public SearchAllRepresentation Representations { get; set; }
        public SearchAllVideos Videos { get; set; }
        public SearchAllFiles Files { get; set; }
    }

    public class SearchUserPageInfo
    {
        public int TotalUsers { get; set; }
        public int UsersPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalUsers / UsersPerPage);
    }

    public class SearchAllUsers
    {
        public IQueryable<AppUser> AllUsers { get; set; }
        public SearchUserPageInfo UserPageInfo { get; set; }
        public string userImgSrc(string path)
        {
            return (path == null && path == "") ? $"/UsersData/{path}" : "/defaultAvatar.png";
        }
        public string firstLastNameNull(AppUser user)
        {
            string returnStr = "";

            if (user.FirstName != null && user.LastName != null)
            {
                returnStr += user.FirstName + "</br>" + user.LastName;
            }
            else if (user.FirstName != null)
            {
                returnStr += user.FirstName;
            }
            else if (user.LastName != null)
            {
                returnStr += user.LastName;
            }

            return returnStr;
        }
    }

    public class SearchCourse
    {
        public Course Course { get; set; }
        public AppUser CreatedBy { get; set; }
    }

    public class SearchCoursePageInfo
    {
        public int TotalCourses { get; set; }
        public int CoursesPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalCourses / CoursesPerPage);
    }

    public class SearchAllCourses
    {
        public IQueryable<SearchCourse> AllCourses { get; set; }
        public SearchCoursePageInfo CoursePageInfo { get; set; }

        public string userImgSrc(string path)
        {
            return (path == null && path == "") ? $"/UsersData/{path}" : "/defaultAvatar.png";
        }

        public string courseCreatedBy(AppUser user)
        {
            return (user.FirstName == null || user.LastName == null) ? user.UserName :
                $"{user.FirstName} {user.LastName}";
        }

        public string courseEndDate(DateTime date)
        {
            string returnStr = "";

            if (date == DateTime.MinValue)
            {
                returnStr += "<strong class='text-success'>NO End Date</strong>";
            }
            else if (date < DateTime.Now)
            {
                returnStr += "<strong class='text-danger'>FINISHED</strong>";
            }
            else
            {
                returnStr += $"<strong class='text-warning'>{date.ToString("dd.MM.yyyy.")}</strong>";
            }

            return returnStr;
        }

        public string courseTitle(string title)
        {
            return (title.Length > 12) ? title.Substring(0, 11) + "..." : title;
        }
    }

    public class SearchPresentation
    {
        public Presentation Presentation { get; set; }
        public Course Course { get; set; }
        public AppUser CreatedBy { get; set; }
    }

    public class SearchPresentationPageInfo
    {
        public int TotalPresentations { get; set; }
        public int PresentationsPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalPresentations / PresentationsPerPage);
    }

    public class SearchAllPresentation
    {
        public IQueryable<SearchPresentation> AllPresentations { get; set; }
        public SearchPresentationPageInfo PresentationPageInfo { get; set; }
        public string userImgSrc(string path)
        {
            return (path == null && path == "") ? $"/UsersData/{path}" : "/defaultAvatar.png";
        }

        public string courseCreatedBy(AppUser user)
        {
            return (user.FirstName == null || user.LastName == null) ? user.UserName :
                $"{user.FirstName} {user.LastName}";
        }

        public string courseEndDate(DateTime date)
        {
            string returnStr = "";

            if (date == DateTime.MinValue)
            {
                returnStr += "<strong class='text-success'>NO End Date</strong>";
            }
            else if (date < DateTime.Now)
            {
                returnStr += "<strong class='text-danger'>FINISHED</strong>";
            }
            else
            {
                returnStr += $"<strong class='text-warning'>{date.ToString("dd.MM.yyyy.")}</strong>";
            }

            return returnStr;
        }

        public string presentationTitle(string title)
        {
            return (title.Length > 18) ? title.Substring(0, 18) + "..." : title;
        }
    }

    public class SearchRepresentation
    {
        public Representation Representation { get; set; }
        public Video Video { get; set; }
        public string PresentationTitle { get; set; }
        public Course Course { get; set; }
        public AppUser CreatedBy { get; set; }
    }

    public class SearchRepresentationPageInfo
    {
        public int TotalRepresentation { get; set; }
        public int RepresentationPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalRepresentation / RepresentationPerPage);
    }

    public class SearchAllRepresentation
    {
        public IQueryable<SearchRepresentation> AllRepresentation { get; set; }
        public SearchRepresentationPageInfo RepresentationPageInfo { get; set; }

        public string userImgSrc(string path)
        {
            return (path == null && path == "") ? $"/UsersData/{path}" : "/defaultAvatar.png";
        }

        public string courseCreatedBy(AppUser user)
        {
            return (user.FirstName == null || user.LastName == null) ? user.UserName :
                $"{user.FirstName} {user.LastName}";
        }

        public string representationRating(double rating)
        {
            string returnStr = "";

            if (rating == 0)
            {
                returnStr = "<strong class='text-muted'>Not Rated Yet</strong>";
            }
            else
            {
                for (var i = 0; i < rating; i++)
                {
                    returnStr += "<span class='fa fa-star text-warning'></span>";
                }
                if (rating < 5)
                {
                    for (var i = 0; i < 5 - rating; i++)
                    {
                        returnStr += "<span class='fa fa-star text-muted'></span>";
                    }
                }
                returnStr += $"<span>&emsp; {String.Format("{0:0.0}", rating)} / 5.0 </span>";
            }

            return returnStr;
        }

    }

    public class SearchVideo
    {
        public Video Video { get; set; }
        public AppUser CreatedBy { get; set; }
        public string VideoForInfo { get; set; }
    }

    public class SearchVideoPageInfo
    {
        public int TotalVideos { get; set; }
        public int VideosPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalVideos / VideosPerPage);
    }

    public class SearchAllVideos
    {
        public IQueryable<SearchVideo> AllVideos { get; set; }
        public SearchVideoPageInfo VideoPageInfo { get; set; }

        public string userImgSrc(string path)
        {
            return (path == null && path == "") ? $"/UsersData/{path}" : "/defaultAvatar.png";
        }

        public string courseCreatedBy(AppUser user)
        {
            return (user.FirstName == null || user.LastName == null) ? user.UserName :
                $"{user.FirstName} {user.LastName}";
        }
    }

    public class SearchFiles
    {
        public PresentationFiles File { get; set; }
        public AppUser CreatedBy { get; set; }
        public string FileForInfo { get; set; }
    }

    public class SearchFilesPageInfo
    {
        public int TotalFiles { get; set; }
        public int FilesPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalFiles / FilesPerPage);
    }

    public class SearchAllFiles
    {
        public IQueryable<SearchFiles> AllFiles { get; set; }
        public SearchFilesPageInfo FilePageInfo { get; set; }

        public string userImgSrc(string path)
        {
            return (path == null && path == "") ? $"/UsersData/{path}" : "/defaultAvatar.png";
        }

        public string courseCreatedBy(AppUser user)
        {
            return (user.FirstName == null || user.LastName == null) ? user.UserName :
                $"{user.FirstName} {user.LastName}";
        }
    }
}

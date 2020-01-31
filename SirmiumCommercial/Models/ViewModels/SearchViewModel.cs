using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class SearchViewModel
    {
        public SearchAllUsers Users { get; set; }
        public SearchAllCourses Courses { get; set; }
        public SearchAllPresentation Presentations { get; set; }
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
    }

    public class SearchCourse
    {
        public Course Course { get; set; }
        public Video CourseVideo { get; set; }
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
    }

    public class SearchPresentation
    {
        public Presentation Presentation { get; set; }
        public Video PresentationVideo { get; set; }
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
    }
}

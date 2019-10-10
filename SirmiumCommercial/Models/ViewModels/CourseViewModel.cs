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
    }

    public class NewEditPresentation
    {
        public Presentation Presentation { get; set; }
        public int CourseId { get; set; }
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

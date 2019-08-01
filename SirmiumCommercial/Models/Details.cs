using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models
{
    //user details
    public class UserDetails
    {
        public int Id { get; set; }
        public AppUser User { get; set; }
        public string ProfilePhotoURL { get; set; }
    }

    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public AppUser CreatedBy { get; set; }
        public string CompanyName { get; set; }
        public ICollection<AppUser> Users { get; set; }
            = new List<AppUser>();
    }

    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<Presentation> Presentations { get; set; }
            = new List<Presentation>();
        //Users on the Courses
        public ICollection<AppUser> Users { get; set; }
            = new List<AppUser>();
        public string AwardIcon { get; set; }

        //Public -- all users can see
        //Private -- Default value, only creator can see
        //GroupPublic -- TODO
        public string Status { get; set; } = "Private";

        public string Description { get; set; }
        public string VideoURL { get; set; }
    }

    public class Presentation
    {
        public int PresentationId { get; set; }
        public string Title { get; set; }
        public int Part { get; set; }
        public string Description { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

        //Public -- all users can see
        //Private -- Default value, only creator can see
        public string Status { get; set; } = "Private";

        public ICollection<Representation> Representations { get; set; }
            = new List<Representation>();

        public string VideoURL { get; set; }
    }

    public class Representation
    {
        public int RepresentationId { get; set; }
        public string Title { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }

        //Public -- all users can see
        //Private -- Default value, only creator can see
        public string Status { get; set; } = "Private";

        public string VideoURL { get; set; }
    }
}

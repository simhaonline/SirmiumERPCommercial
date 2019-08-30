using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public DateTime RegistrationDate { get; set; }

        //Status Active or Inactive
        public string Status { get; set; }

        //Profile Photo
        public string ProfilePhotoUrl { get; set; }

        //User Courses
        public ICollection<CourseUsers> Courses { get; set; }
            = new List<CourseUsers>();

        //User Groups
        public ICollection<GroupUsers> Groups { get; set; }
            = new List<GroupUsers>();
    }

    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public AppUser CreatedBy { get; set; }
        public string CompanyName { get; set; }
        public ICollection<GroupUsers> Users { get; set; }
            = new List<GroupUsers>();
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
        public ICollection<CourseUsers> Users { get; set; }
            = new List<CourseUsers>();

        public string AwardIcon { get; set; }

        //Public -- all users can see
        //Private -- Default value, only creator can see
        //GroupPublic -- TODO
        public string Status { get; set; } = "Private";

        public string Description { get; set; }
        public int VideoId { get; set; }
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

        public int VideoId { get; set; }
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

        public int VideoId { get; set; }
    }

    public class CourseUsers
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string AppUserId { get; set; }
    }

    public class GroupUsers
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string AppUserId { get; set; }
    }

    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //User Id
        public string CreatedBy { get; set; }
        //Course, Presentation, Representation or Practice
        public string For { get; set; }
        //CourseId, PresentationId, RepresentationId or PracticeId
        public int ForId { get; set; }
        //Public or Private
        public string Status { get; set; }
        public DateTime DateAdded { get; set; }
        public int Views { get; set; }
        //video path
        public string VideoPath { get; set; }
    }
}

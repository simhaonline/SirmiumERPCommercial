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
        public string ProfilePhoto { get; set; }
    }

    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public AppUser CreatedBy { get; set; }
        public string CompanyName { get; set; }
        public IQueryable<AppUser> Users { get; set; }
    }

    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime EndDate { get; set; }
        public IQueryable<Presentation> Presentations { get; set; }
        //Users on the Courses
        public IQueryable<AppUser> Users { get; set; }
        public string AwardIcon { get; set; }
        public string Status { get; set; } //public or private
        public string Description { get; set; }
    }

    public class Presentation
    {
        public int PresentationId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime EndDate { get; set; }
        public IQueryable<Representation> Representations { get; set; }
        public string Status { get; set; }
    }

    public class Representation
    {
        public int RepresentationId { get; set; }
        public string Title { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public string Status { get; set; }
        public double Score { get; set; }
    }
}

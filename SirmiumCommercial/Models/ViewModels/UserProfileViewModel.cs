using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class UserProfileViewModel
    {
        public AppUser User { get; set; }

        public double AverageRating { get; set; }
        public int Views { get; set; }
        public int TotalRepresentations { get; set; }
        public IQueryable<Course> StartedCourses { get; set; }
        public IQueryable<Course> FinishedCourses { get; set; }

        public HomeContent UserTimeline { get; set; }
    }
}

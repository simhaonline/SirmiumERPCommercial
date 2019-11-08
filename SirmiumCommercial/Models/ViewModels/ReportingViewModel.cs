using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class ReportingViewModel
    {
    }

    public class ReportingAllCourseVCViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseAward { get; set; }
        public int TotalParts { get; set; }
        public int CompletedParts { get; set; }
    }
}

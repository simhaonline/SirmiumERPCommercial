using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class FilesViewModel
    {
        public Course Course { get; set; }
        public IQueryable<PresentationFiles> Files { get; set; }
        public FilesPerCourseInfo CoursePageInfo { get; set; }
    }

    public class FileSearchViewModel
    {
        public string UserId { get; set; }
        public string KeyWord { get; set; }
        public int TotalFiles { get; set; }
        public IQueryable<PresentationFiles> Files { get; set; }
        public FilesPerCourseInfo PageInfo { get; set; }
    }

    public class FilesPerCourseInfo
    {
        public int TotalFiles { get; set; }
        public int FilesPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalFiles / FilesPerPage);
    }
}

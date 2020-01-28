using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class NewPresentationStep1ViewModel
    {
        public Presentation Presentation { get; set; }
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public int PresentationPart { get; set; }
    }

    public class NewPresentationStep2ViewModel
    {
        public int CourseId { get; set; }
        public int PresentationId { get; set; }
        public string VideoTitle { get; set; }
        public string UserId { get; set; }
        public string videoUrl { get; set; }

        public string TitlePlaceholder { get; set; }
    }

    public class NewPresentationStep3ViewModel
    {
        public int CourseId { get; set; }
        public int PresentationId { get; set; }
        public string FileTitle { get; set; }
        public string UserId { get; set; }
        public int Part { get; set; }
        public IFormFile File { get; set; }

        //uploaded files
        public IQueryable<PresentationFiles> PresentationFiles { get; set; }
    }

    public class EditPresentation
    {
        public Presentation Presentation { get; set; }
        public Video Video { get; set; }
        public EditPresentationFiles Files { get; set; }

        //-----
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public int PresentationId { get; set; }
        public string Title { get; set; }
        public int Part { get; set; }
        public string Description { get; set; }
        public int VideoId { get; set; }
        public string VideoTitle { get; set; }
    }

    public class EditPresentationFiles
    {
        public IQueryable<PresentationFiles> Files { get; set; }
        public EditPresentationFilesPageInfo PageInfo { get; set; }

        //.
        public string UserId { get; set; }
        public int CourseId { get; set; }
        public int PresentationId { get; set; }
        public int FileId { get; set; }
        public string FileTitle { get; set; }
        public int FilePart { get; set; }
    }

    public class EditPresentationFilesPageInfo
    {
        public int TotalFiles { get; set; }
        public int FilesPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalFiles / FilesPerPage);
    }
    /*
     
        public int FileId { get; set; }

        public int PresentationId { get; set; }
        public string Title { get; set; }
        public int Part { get; set; }
        public string CreatedById { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string FilePath { get; set; } */
}

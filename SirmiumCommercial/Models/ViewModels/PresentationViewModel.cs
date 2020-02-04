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

        //---new file (change file)
        public int NewFileId { get; set; }
        public string NewFileTitle { get; set; }
        public int NewFilePart { get; set; }
        public IFormFile NewFile { get; set; }
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

    public class PresentationDetailsFilesModel
    {
        public IQueryable<PresentationFiles> Files { get; set; }
        public EditPresentationFilesPageInfo PresentationFilesPageInfo { get; set; }
    }
   
    public class PresentationDetailsRepres
    {
        public Representation Representation { get; set; }
        public AppUser CreatedBy { get; set; }
        public Video Video { get; set; }
    }

    public class PresentationDetailsRepresPageInfo
    {
        public int TotalRepres { get; set; }
        public int RepresPerPage { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages =>
        (int)Math.Ceiling((decimal)TotalRepres / RepresPerPage);
    }

    public class PresentationDetailsRepresentationModel
    {
        public IQueryable<PresentationDetailsRepres> Representations { get; set; }
        public PresentationDetailsRepresPageInfo RepresPageInfo { get; set; }

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

    public class PresentationDetailsViewModel
    {
        public Presentation Presentation { get; set; }
        public AppUser CreatedBy { get; set; }
        public Video Video { get; set; }
        public PresentationDetailsFilesModel Files { get; set; }
        public PresentationDetailsRepresentationModel Representations { get; set; }
        public int CourseId { get; set; }
    }
}

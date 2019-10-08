using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class PlayerViewModel
    {
        public Video Video { get; set; }
        public IQueryable<Likes> Likes { get; set; }
        public IQueryable<Dislikes> Dislikes { get; set; }
        public IQueryable<Comment> Comments { get; set; }
        public IQueryable<AppUser> Users { get; set; }
    }

    public class VideoInfo
    {
        public Video Video { get; set; }
        public AppUser CreatedBy { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int Comments { get; set; }
    }

    public class GalleryViewModel
    {
        public IQueryable<VideoInfo> Top10 { get; set; }
        public IQueryable<VideoInfo> MostPopular { get; set; }
        public IQueryable<VideoInfo> Private { get; set; }
        public IQueryable<VideoInfo> CourseVideos { get; set; }
        public IQueryable<VideoInfo> PresentationVideos { get; set; }
        public IQueryable<VideoInfo> RepresentationVideos { get; set; }
    }

    public class GalleryPartialViewModel
    {
        public IQueryable<VideoInfo> VideoInfo { get; set; }
        public string DivId { get; set; }
        public string BtnId { get; set; }
        public string OnClickAction { get; set; }

        public string VideoName(string name)
        {
            return (name.Length < 20) ?
                $"<h3>{name}</h3>" :
                "<h3 data-toggle='tooltip' data-placement='bottom'" +
                $"title='{name}'>{name.Substring(0, 16)}...</h3>";
        }

        public string GalleryName(string name)
        {
            switch (name)
            {
                case "mostPopular":
                    return "Most Popular";
                case "top10":
                    return "Top 10 Videos";
                case "course":
                    return "Course Videos";
                case "presentation":
                    return "Presentation Videos";
                case "representation":
                    return "Reresentation Videos";
                default:
                    return "Private Video";
            }
        }

        public string onClickFunction (string onClickAction)
        {
            return $"collapseBtn.{onClickAction}()";
        }

        public string panelTopColor (string str)
        {
            switch (str)
            {
                case "mostPopular":
                    return "hreddeep";
                case "top10":
                    return "hviolet";
                case "course":
                    return "horange";
                case "presentation":
                    return "hyellow";
                case "representation":
                    return "hgreen";
                default:
                    return "hnavyblue";
            }
        }
    }
}

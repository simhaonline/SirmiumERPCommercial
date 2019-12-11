using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class VideoSettingsViewModel
    {
        public int VideoId { get; set; }
        public string VideoTitle { get; set; }
        public string UserId { get; set; }

        public IQueryable<AppUser> CanSeeVideo { get; set; }
        public IQueryable<AppUser> AllUsers { get; set; }

        public string UsersIds { get; set; }
    }

    public class VideoSettingsViewComponentViewModel
    {
        public string CreatedById { get; set; }
        public string UserId { get; set; }
        public int VideoId { get; set; }
        public bool Show { get; set; }
    }
}

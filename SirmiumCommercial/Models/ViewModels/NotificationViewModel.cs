using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class NotificationViewModel
    {
        public NotificationCard NotificationCard { get; set; }
        public IQueryable<NotificationViews> Views { get; set; }
        public string For { get; set; }
        public int ForId { get; set; }
        public string UserProfilePhoto { get; set; }
    }
}

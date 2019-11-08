using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using SirmiumCommercial.Hubs;
using Microsoft.AspNetCore.Identity;

namespace SirmiumCommercial.Controllers
{
    public class NotificationController : Controller
    {
        private UserManager<AppUser> userManager;
        private IAppDataRepository repository;

        public NotificationController(UserManager<AppUser> userMgr, IAppDataRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public ViewResult Index(string id)
        {
            ViewData["Id"] = id;

            return View();
        }
        
        public RedirectToActionResult Redirect (string id, int notificationCardId,
            string For, int ForId)
        {
            if (For == "Video")
            {
                NotificationCard card = repository.NotificationCards
                    .FirstOrDefault(c => c.NotificationCardId == notificationCardId);
                if(card.NotificationViews.FirstOrDefault(v => v.UserId == id) == null)
                {
                    repository.NewNotificationCardView(notificationCardId, id);
                }

                return RedirectToAction("Index", "Recorder", 
                    new { id, videoId = ForId });
            }
            else if (For == "Course")
            {
                NotificationCard card = repository.NotificationCards
                    .FirstOrDefault(c => c.NotificationCardId == notificationCardId);
                if (card.NotificationViews.FirstOrDefault(v => v.UserId == id) == null)
                {
                    repository.NewNotificationCardView(notificationCardId, id);
                }

                return RedirectToAction("CourseDetailsComments", "Courses",
                    new { id, courseId = ForId });
            }

            return RedirectToAction("Error", "Error",
                    new { id, errMsg = "" });
        }
        /*
        public RedirectToActionResult Remove (string id, int notificationId)
        {
            repository.DeleteNotification(notificationId);
            return RedirectToAction("Index", id);
        }*/
    }
}

using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace SirmiumCommercial.Hubs
{
    public class NotificationHub : Hub
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public NotificationHub(IAppDataRepository repo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public void Participate(string userId, int courseId)
        {
            //add user to course
            repository.AddUserToCourse(userId, courseId);

            //notification
            _ = ShowNewUserNotification(userId, courseId);
        }

        public async Task ShowNewUserNotification(string userId, int courseId)
        {
            //for user who created this course
            Course course = repository.Courses
                .FirstOrDefault(c => c.CourseId == courseId);

            Notification notification = repository.Notifications
                .FirstOrDefault(n => n.Subject == "UserJoinCourse"
                                && n.For == "Course" && n.ForId == courseId);
            NotificationCard notificationCard = notification.NotificationCards
                .LastOrDefault(c => c.CreatedBy == userId);

            AppUser notificationCardCreatedBy = userManager.Users
                .FirstOrDefault(u => u.Id == notificationCard.CreatedBy);
            string notificationCardCreatedByPhoto = (notificationCardCreatedBy.ProfilePhotoUrl != null) ?
                $"/UsersData/{notificationCardCreatedBy.ProfilePhotoUrl}" : "/defaultAvatar.png";

            if (userId != course.CreatedBy.Id)
            {
                //for user who created this course
                await Clients.All.SendAsync("ShowNewUserOnCourseNotification", course.CreatedBy.Id,
                    notificationCard.NotificationCardId, course.CourseId,
                    notificationCard.Msg, notificationCardCreatedByPhoto);
            }
        }

        public void RepresentationNewRating(string userId, int representationId, int rating)
        {
            //representation rating
            Representation representation = repository.Representations
                .FirstOrDefault(r => r.RepresentationId == representationId);

            representation.Rating = rating;
            repository.SaveRepresentation(representation);
            //add new notification
            repository.NewNotification(userId, "RepresentationRating", "Video", representation.VideoId);

            _ = ShowRepresentationNewRating(userId, representationId);
        }

        public async Task ShowRepresentationNewRating(string userId, int representationId)
        {
            //for user who created representation
            Representation representation = repository.Representations
                .FirstOrDefault(r => r.RepresentationId == representationId);
            Notification notification = repository.Notifications
                .FirstOrDefault(n => n.Subject == "RepresentationRating"
                                && n.For == "Video" && n.ForId == representation.VideoId);
            NotificationCard notificationCard = notification.NotificationCards
                .LastOrDefault(c => c.CreatedBy == userId);

            AppUser notificationCardCreatedBy = userManager.Users
                .FirstOrDefault(u => u.Id == notificationCard.CreatedBy);
            string notificationCardCreatedByPhoto = (notificationCardCreatedBy.ProfilePhotoUrl != null) ?
                $"/UsersData/{notificationCardCreatedBy.ProfilePhotoUrl}" : "/defaultAvatar.png";

            if (userId != representation.CreatedBy.Id)
            {
                //for user who created this course
                await Clients.All.SendAsync("ShowNewRatingNotification", representation.CreatedBy.Id,
                    notificationCard.NotificationCardId, representation.VideoId,
                    notificationCard.Msg, notificationCardCreatedByPhoto);
            }
        }
    }
}

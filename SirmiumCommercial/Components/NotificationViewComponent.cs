using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace SirmiumCommercial.Components
{
    public class NotificationViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public NotificationViewComponent (IAppDataRepository repo, 
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }
        
        public IViewComponentResult Invoke(string userId)
        {
            List<NotificationViewModel> notifications = new List<NotificationViewModel>();

            foreach (Notification notification in repository.Notifications
                .OrderByDescending(n => n.NotificationDateAdded))
            {
                if (notification.For == "Video")
                {
                    //for LikeVideo, DislikeVideo, NewComment
                    if (userId == repository.Videos
                            .FirstOrDefault(v => v.Id == notification.ForId).CreatedBy
                          && notification.Subject != "LikeComment"
                          && notification.Subject != "DislikeComment")
                    {
                        NotificationCard notificationCard = notification.NotificationCards
                                    .Last(nc => nc.CreatedBy != userId);
                        NotificationViewModel nModel = new NotificationViewModel
                        {
                            NotificationCard = notificationCard,
                            Views = notificationCard.NotificationViews.AsQueryable(),
                            UserProfilePhoto = userManager.Users
                                .FirstOrDefault(u => u.Id == notificationCard.CreatedBy)
                                .ProfilePhotoUrl,
                            For = notification.For,
                            ForId = notification.ForId
                        };

                        notifications.Add(nModel);
                    }
                    //For New Comment
                    else if (notification.Subject == "NewComment")
                    {
                        IQueryable<Comment> comments = repository.Comments
                            .Where(c => c.For == "Video" && c.ForId == notification.ForId);

                        if (comments.FirstOrDefault(c => c.CreatedBy == userId) != null)
                        {
                            NotificationCard notificationCard = notification.NotificationCards
                                    .Last(nc => nc.CreatedBy != userId);
                            NotificationViewModel nModel = new NotificationViewModel
                            {
                                NotificationCard = notificationCard,
                                Views = notificationCard.NotificationViews.AsQueryable(),
                                UserProfilePhoto = userManager.Users
                                    .FirstOrDefault(u => u.Id == notificationCard.CreatedBy)
                                    .ProfilePhotoUrl,
                                For = notification.For,
                                ForId = notification.ForId
                            };

                            notifications.Add(nModel);
                        }
                    }
                }
                else if (notification.For == "Course")
                {
                    Course course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == notification.ForId);

                    if (course.CreatedBy != null) { 
                    if (userId == course.CreatedBy.Id)
                    {
                        NotificationCard notificationCard = notification.NotificationCards
                                    .Last(nc => nc.CreatedBy != userId);
                        NotificationViewModel nModel = new NotificationViewModel
                        {
                            NotificationCard = notificationCard,
                            Views = notificationCard.NotificationViews.AsQueryable(),
                            UserProfilePhoto = userManager.Users
                                .FirstOrDefault(u => u.Id == notificationCard.CreatedBy)
                                .ProfilePhotoUrl,
                            For = notification.For,
                            ForId = notification.ForId
                        };

                        notifications.Add(nModel);
                    }
                    }
                    else if (notification.Subject == "NewComment")
                    {
                        IQueryable<Comment> comments = repository.Comments
                            .Where(c => c.For == "Course" && c.ForId == notification.ForId);

                        if (comments.FirstOrDefault(c => c.CreatedBy == userId) != null)
                        {
                            NotificationCard notificationCard = notification.NotificationCards
                                    .Last(nc => nc.CreatedBy != userId);
                            NotificationViewModel nModel = new NotificationViewModel
                            {
                                NotificationCard = notificationCard,
                                Views = notificationCard.NotificationViews.AsQueryable(),
                                UserProfilePhoto = userManager.Users
                                    .FirstOrDefault(u => u.Id == notificationCard.CreatedBy)
                                    .ProfilePhotoUrl,
                                For = notification.For,
                                ForId = notification.ForId
                            };

                            notifications.Add(nModel);
                        }
                    }
                }
                else if (notification.For == "Comment")
                {
                    //For Like and Dislike Comment
                    if (notification.Subject == "LikeDislikeComment")
                    {
                        //notification.for = "Comment" 
                        //notification.forID = CommentId
                        //get comment where commentId == notification.forID
                        Comment comment = repository.Comments
                            .FirstOrDefault(c => c.Id == notification.ForId);

                        //if user with user.id == userId
                        //created this comment
                        if (comment.CreatedBy == userId)
                        {
                            NotificationCard notificationCard = notification.NotificationCards
                                    .Last(nc => nc.CreatedBy != userId);
                            NotificationViewModel nModel = new NotificationViewModel
                            {
                                NotificationCard = notificationCard,
                                Views = notificationCard.NotificationViews.AsQueryable(),
                                UserProfilePhoto = userManager.Users
                                    .FirstOrDefault(u => u.Id == notificationCard.CreatedBy)
                                    .ProfilePhotoUrl,
                                For = comment.For,
                                ForId = comment.ForId
                            };

                            notifications.Add(nModel);
                        }
                    }
                }
            }

            return View(notifications.AsQueryable());
        }
    }
}

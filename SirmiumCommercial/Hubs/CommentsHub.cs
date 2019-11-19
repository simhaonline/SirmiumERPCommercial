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
    public class CommentsHub : Hub
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public CommentsHub(IAppDataRepository repo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public async Task NewComment(string currentUser, string commentFor, int commentForId,
               string commentContent)
        {
            Comment comment = new Comment
            {
                CreatedBy = currentUser,
                For = commentFor,
                ForId = commentForId,
                Content = commentContent,
                DateAdded = DateTime.Now
            };
            repository.SaveComment(comment);

            if (commentFor == "Course")
            {
                _ = ShowCourseCommentNotification(currentUser, commentForId);
            }
            else if (commentFor == "Video")
            {
                _ = ShowVideoCommentNotification(currentUser, commentForId);
            }

            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == currentUser);

            await Clients.All.SendAsync("PostNewComment", comment.For,
                comment.ForId, comment.DateAdded, comment.Content, comment.Id, comment.CreatedBy, user.FirstName,
                user.LastName, user.ProfilePhotoUrl);
        }

        public async Task NewSubComment(string currentUser, string commentFor, int commentForId,
                        int parentCommentId, string content)
        {
            Comment comment = new Comment
            {
                CreatedBy = currentUser,
                For = commentFor,
                ForId = commentForId,
                Content = content,
                CommentId = parentCommentId,
                DateAdded = DateTime.Now
            };
            repository.SaveComment(comment);

            if (commentFor == "Course")
            {
                _ = ShowCourseCommentNotification(currentUser, commentForId);
            }
            else if (commentFor == "Video")
            {
                _ = ShowVideoCommentNotification(currentUser, commentForId);
            }

            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == currentUser);

            await Clients.All.SendAsync("PostNewSubComment", comment.For,
                comment.ForId, comment.DateAdded, comment.Content, parentCommentId, comment.CreatedBy, user.FirstName,
                user.LastName, user.ProfilePhotoUrl, comment.Id);
        }

        public async Task CommentLikesDislikesInitial(string userId, int commentId,
            string likeSpan, string dislikeSpan)
        {
            Comment comment = repository.Comments
                .FirstOrDefault(c => c.Id == commentId);

            bool userLikeInd = repository.Likes
                .Any(l => l.For == "Comment" && l.ForId == commentId && l.UserId == userId);
            bool userDislikeInd = repository.Dislikes
                .Any(d => d.For == "Comment" && d.ForId == commentId && d.UserId == userId);
            int totalLikes = repository.Likes
                .Where(l => l.For == "Comment" && l.ForId == commentId)
                .Count();
            int totalDislikes = repository.Dislikes
                .Where(d => d.For == "Comment" && d.ForId == commentId)
                .Count();

            await Clients.All.SendAsync("InitialLikesDislikes", comment.For, comment.ForId, userLikeInd,
                userDislikeInd, totalLikes, totalDislikes, likeSpan, dislikeSpan);
        }

        public void CommentLike(string userId, int commentId,
            string likeSpan, string dislikeSpan)
        {
            Likes like = repository.Likes
                .FirstOrDefault(l => l.For == "Comment" && l.ForId == commentId
                    && l.UserId == userId);

            if (like == null)
            {
                like = new Likes
                {
                    For = "Comment",
                    ForId = commentId,
                    UserId = userId
                };
                repository.AddLike(like);

                //notification
                _ = ShowNotification(userId, commentId);
            }
            else
            {
                repository.DeleteLike(like.Id);
            }

            _ = CommentLikesDislikesInitial(userId, commentId, likeSpan, dislikeSpan);
        }

        public void CommentDislike(string userId, int commentId,
            string likeSpan, string dislikeSpan)
        {
            Dislikes dislike = repository.Dislikes
                .FirstOrDefault(d => d.For == "Comment" && d.ForId == commentId
                    && d.UserId == userId);

            if (dislike == null)
            {
                dislike = new Dislikes
                {
                    For = "Comment",
                    ForId = commentId,
                    UserId = userId
                };
                repository.AddDislike(dislike);

                //notification
                _ = ShowNotification(userId, commentId);
            }
            else
            {
                repository.DeleteDislike(dislike.Id);
            }

            _ = CommentLikesDislikesInitial(userId, commentId, likeSpan, dislikeSpan);
        }

        public async Task RemoveComment(int commentId)
        {
            Comment comment = repository.Comments
                .FirstOrDefault(c => c.Id == commentId);
            string cFor = comment.For;
            int cForId = comment.ForId;

            repository.DeleteComment(comment.Id);

            await Clients.All.SendAsync("CommentRemove", commentId, cFor, cForId);
        }

        //comment like/dislike notifications
        public async Task ShowNotification(string userId, int commentId)
        {
            Comment comment = repository.Comments
                .FirstOrDefault(c => c.Id == commentId);
            Notification notification = repository.Notifications
                .FirstOrDefault(n => n.Subject == "LikeDislikeComment"
                        && n.For == "Comment" && n.ForId == comment.Id);
            NotificationCard notificationCard = notification.NotificationCards
                .LastOrDefault(c => c.CreatedBy == userId);
            AppUser notificationCardCreatedBy = userManager.Users
                .FirstOrDefault(u => u.Id == notificationCard.CreatedBy);
            string notificationCardCreatedByPhoto = (notificationCardCreatedBy.ProfilePhotoUrl != null) ?
                    $"/UsersData/{notificationCardCreatedBy.ProfilePhotoUrl}" : "/defaultAvatar.png";

            //if NotificationCreatedBy == CommentCreatedBy
            //DO NOT SHOW notification
            if (userId != comment.CreatedBy)
            {
                await Clients.All.SendAsync("ShowNewNotification", comment.CreatedBy,
                    notificationCard.NotificationCardId, comment.For, comment.ForId,
                    notificationCard.Msg, notificationCardCreatedByPhoto);
            }
        }

        //new comment notifications
        //comment for course
        public async Task ShowCourseCommentNotification(string userId, int courseId)
        {
            //for all users who commented this course
            //and user who created this course
            Course course = repository.Courses
                .FirstOrDefault(c => c.CourseId == courseId);

            Notification notification = repository.Notifications
                .FirstOrDefault(n => n.Subject == "NewComment"
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
                await Clients.All.SendAsync("ShowNewCourseCommentNotification", course.CreatedBy.Id,
                    notificationCard.NotificationCardId, course.CourseId,
                    notificationCard.Msg, notificationCardCreatedByPhoto);
            }

            //for all users who commented this course 
            foreach (string commentCreatedBy in repository.Comments
                    .Where(c => c.For == "Course" && c.ForId == courseId
                            && c.CreatedBy != course.CreatedBy.Id
                            && c.CreatedBy != notificationCardCreatedBy.Id)
                    .Select(c => c.CreatedBy).Distinct())
            {
                await Clients.All.SendAsync("ShowNewCourseCommentNotification", commentCreatedBy,
                notificationCard.NotificationCardId, course.CourseId,
                notificationCard.Msg, notificationCardCreatedByPhoto);
            }
        }

        //comment for video
        public async Task ShowVideoCommentNotification(string userId, int videoId)
        {
            //for all users who commented this course
            //and user who created this course
            Video video = repository.Videos
                .FirstOrDefault(v => v.Id == videoId);

            Notification notification = repository.Notifications
                .FirstOrDefault(n => n.Subject == "NewComment"
                        && n.For == "Video" && n.ForId == videoId);
            NotificationCard notificationCard = notification.NotificationCards
                .LastOrDefault(c => c.CreatedBy == userId);
            AppUser notificationCardCreatedBy = userManager.Users
                .FirstOrDefault(u => u.Id == notificationCard.CreatedBy);
            string notificationCardCreatedByPhoto = (notificationCardCreatedBy.ProfilePhotoUrl != null) ?
                    $"/UsersData/{notificationCardCreatedBy.ProfilePhotoUrl}" : "/defaultAvatar.png";

            if (userId != video.CreatedBy)
            {
                //for user who created this course
                await Clients.All.SendAsync("ShowNewVideoCommentNotification", video.CreatedBy,
                    notificationCard.NotificationCardId, videoId,
                    notificationCard.Msg, notificationCardCreatedByPhoto);
            }

            //for all users who commented this course 
            foreach (string commentCreatedBy in repository.Comments
                    .Where(c => c.For == "Video" && c.ForId == videoId
                            && c.CreatedBy != video.CreatedBy
                            && c.CreatedBy != notificationCardCreatedBy.Id)
                    .Select(c => c.CreatedBy).Distinct())
            {
                await Clients.All.SendAsync("ShowNewVideoCommentNotification", commentCreatedBy,
                notificationCard.NotificationCardId, videoId,
                notificationCard.Msg, notificationCardCreatedByPhoto);
            }
        }
    }
}

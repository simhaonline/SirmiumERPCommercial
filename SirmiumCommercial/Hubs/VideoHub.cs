using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SirmiumCommercial.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Hubs
{
    public class VideoHub : Hub
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public VideoHub(IAppDataRepository repo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public async Task VideoViews (int videoId)
        {
            Video video = repository.Videos
                .FirstOrDefault(v => v.Id == videoId);
            int views = video.Views + 1;
            video.Views = views;
            repository.SaveVideo(video);

            await Clients.All.SendAsync("NewVideoView", videoId, views);
        }

        public async Task VideoLikes (int videoId, string userId)
        {
            bool likeInd = false;
            bool dislikeInd = false;
            Likes like = repository.Likes
                .FirstOrDefault(l => l.For == "Video" && 
                    l.ForId == videoId && l.UserId == userId);

            if(like == null)
            {
                like = new Likes
                {
                    For = "Video",
                    ForId = videoId,
                    UserId = userId
                };
                repository.AddLike(like);
                likeInd = true;

                _ = ShowNotification(userId, videoId);
            }
            else
            {
                repository.DeleteLike(like.Id);
            }

            int totalLikes = repository.Likes
                .Where(l => l.For == "Video" && l.ForId == videoId)
                .Count();
            int totalDislikes = repository.Dislikes
                .Where(d => d.For == "Video" && d.ForId == videoId)
                .Count();

            await Clients.All.SendAsync("AddRemoveLike", videoId, userId,
                totalLikes, likeInd, totalDislikes, dislikeInd);
        }

        public async Task VideoDislikes (int videoId, string userId)
        {
            bool likeInd = false;
            bool dislikeInd = false;
            Dislikes dislike = repository.Dislikes
                .FirstOrDefault(d => d.For == "Video" && 
                    d.ForId == videoId && d.UserId == userId);

            if (dislike == null)
            {
                dislike = new Dislikes
                {
                    For = "Video",
                    ForId = videoId,
                    UserId = userId
                };
                repository.AddDislike(dislike);
                dislikeInd = true;

                _ = ShowNotification(userId, videoId);
            }
            else
            {
                repository.DeleteDislike(dislike.Id);
            }

            int totalLikes = repository.Likes
                .Where(l => l.For == "Video" && l.ForId == videoId)
                .Count();
            int totalDislikes = repository.Dislikes
                .Where(d => d.For == "Video" && d.ForId == videoId)
                .Count();

            await Clients.All.SendAsync("AddRemoveDislike", videoId, userId,
                totalLikes, likeInd, totalDislikes, dislikeInd);
        }

        //notifications
        public async Task ShowNotification(string userId, int videoId)
        {
            Video video = repository.Videos
                .FirstOrDefault(v => v.Id == videoId);
            Notification notification = repository.Notifications
                .FirstOrDefault(n => n.Subject == "LikeDislikeVideo"
                        && n.For == "Video" && n.ForId == videoId);
            NotificationCard notificationCard = notification.NotificationCards
                .LastOrDefault(c => c.CreatedBy == userId);
            AppUser notificationCardCreatedBy = userManager.Users
                .FirstOrDefault(u => u.Id == notificationCard.CreatedBy);
            string notificationCardCreatedByPhoto = (notificationCardCreatedBy.ProfilePhotoUrl != null) ?
                    $"/UsersData/{notificationCardCreatedBy.ProfilePhotoUrl}" : "/defaultAvatar.png";

            //if NotificationCreatedBy == VideoCreatedBy
            //DO NOT SHOW notification
            if (userId != video.CreatedBy)
            {
                await Clients.All.SendAsync("ShowNewNotification", video.CreatedBy,
                    notificationCard.NotificationCardId, video.Id,
                    notificationCard.Msg, notificationCardCreatedByPhoto);
            }
        }
    }
}

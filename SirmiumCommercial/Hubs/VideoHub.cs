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

        public VideoHub(IAppDataRepository repo)
        {
            repository = repo;
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
    }
}

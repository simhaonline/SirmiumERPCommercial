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

        public CommentsHub (IAppDataRepository repo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public async Task NewComment (string currentUser, string commentFor, int commentForId,
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

            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == currentUser);

            await Clients.All.SendAsync("PostNewComment", comment.For,
                comment.ForId, comment.DateAdded, comment.Content, comment.Id, comment.CreatedBy, user.FirstName, 
                user.LastName, user.ProfilePhotoUrl);
        }

        public async Task NewSubComment (string currentUser, string commentFor, int commentForId,
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

            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == currentUser);

            await Clients.All.SendAsync("PostNewSubComment", comment.For,
                comment.ForId, comment.DateAdded, comment.Content, parentCommentId, comment.CreatedBy, user.FirstName,
                user.LastName, user.ProfilePhotoUrl, comment.Id);
        }

        public async Task CommentLikesDislikesInitial (string userId, int commentId, 
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

        public void CommentLike (string userId, int commentId,
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
            }
            else
            {
                repository.DeleteLike(like.Id);
            }

            _ = CommentLikesDislikesInitial(userId, commentId, likeSpan, dislikeSpan);
        }

        public void CommentDislike (string userId, int commentId,
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
            }
            else
            {
                repository.DeleteDislike(dislike.Id);
            }

            _ = CommentLikesDislikesInitial(userId, commentId, likeSpan, dislikeSpan);
        }

        public async Task RemoveComment (int commentId)
        {
            Comment comment = repository.Comments
                .FirstOrDefault(c => c.Id == commentId);
            string cFor = comment.For;
            int cForId = comment.ForId;

            repository.DeleteComment(comment.Id);

            await Clients.All.SendAsync("CommentRemove", commentId, cFor, cForId);
        }
    }
}

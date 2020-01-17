using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SirmiumCommercial.Hubs;

namespace SirmiumCommercial.Models
{
    public class EFAppDataRepository : IAppDataRepository
    {
        private AppDbContext context;
        private UserManager<AppUser> userManager;

        public EFAppDataRepository (AppDbContext ctx, UserManager<AppUser> userMgr) 
        {
            context = ctx;
            userManager = userMgr;
        }

        public IQueryable<Group> Groups => context.Groups.Include(u => u.CreatedBy)
            .Include(c => c.Courses).Include(u => u.Users);
        public IQueryable<Course> Courses => context.Courses.Include(u => u.CreatedBy)
            .Include(p => p.Presentations)
            .ThenInclude(r => r.Representations);
        public IQueryable<Presentation> Presentations => context.Presentations
            .Include(r => r.Representations);
        public IQueryable<PresentationFiles> PresentationFiles => context.PresentationFiles;
        public IQueryable<Representation> Representations => context.Representations
            .Include(u => u.CreatedBy);
        public IQueryable<CourseUsers> CourseUsers => context.CourseUsers;
        public IQueryable<GroupUsers> GroupUsers => context.GroupUsers;
        public IEnumerable<GroupCourses> GroupCourses => context.GroupCourses;
        public IQueryable<Video> Videos => context.Videos;
        public IEnumerable<VideoShared> VideoShareds => context.VideoShared;
        public IQueryable<Comment> Comments => context.Comments;
        public IQueryable<Likes> Likes => context.Likes;
        public IQueryable<Dislikes> Dislikes => context.Dislikes;
        public IQueryable<Chat> Chats => context.Chats.Include(c => c.Messages);
        public IQueryable<ChatMessage> ChatMessages => context.ChatMessages;
        public IQueryable<GroupChat> GroupChats => context.GroupChats
            .Include(g => g.Messages).Include(g => g.Users);
        public IQueryable<GroupChatUsers> GroupChatUsers => context.GroupChatUsers;
        public IQueryable<GroupChatMessage> GroupChatMessages => context.GroupChatMessages
            .Include(m => m.Views);
        public IQueryable<GroupMessageView> GroupMessageViews => context.GroupMessageViews;
        public IEnumerable<Notification> Notifications => context.Notifications
            .Include(c => c.NotificationCards).ThenInclude(v => v.NotificationViews);
        public IEnumerable<NotificationCard> NotificationCards => context.NotificationCards
            .Include(v => v.NotificationViews);
        public IEnumerable<NotificationViews> NotificationViews => context.NotificationViews;

        public void SaveCourse(Course course)
        {
            if (course.CourseId == 0)
            {
                context.Attach(course.CreatedBy);
                course.CreatedAt = DateTime.Now;
                course.UpdatedAt = DateTime.Now;
                context.Courses.Add(course);
            }
            else
            {
                Course dbEntry = context.Courses
                    .FirstOrDefault(c => c.CourseId == course.CourseId);
                if (dbEntry != null)
                {
                    dbEntry.UpdatedAt = DateTime.Now;
                    dbEntry.Title = course.Title;
                    dbEntry.Description = course.Description;
                    dbEntry.CreatedBy = course.CreatedBy;
                    dbEntry.EndDate = course.EndDate;
                    dbEntry.DateAdded = course.DateAdded;
                    dbEntry.DateModified = course.DateModified;
                    dbEntry.AwardIcon = course.AwardIcon;
                    dbEntry.Status = course.Status;
                    AddPresentation(course.Presentations, course);
                    dbEntry.Presentations = course.Presentations;
                    dbEntry.VideoId = course.VideoId;
                }
            }
            context.SaveChanges();
        }

        public Course DeleteCourse(int courseId)
        {
            Course dbEntry = context.Courses
                .FirstOrDefault(c => c.CourseId == courseId);
            if (dbEntry != null)
            {
                //delete course video
                Video courseVideo = context.Videos
                    .FirstOrDefault(v => v.Id == dbEntry.VideoId);
                if(courseVideo != null)
                {
                    _ = DeleteVideo(courseVideo.Id);
                }

                //delete presentation 
                foreach (Presentation presentation in dbEntry.Presentations)
                {
                    Presentation p = DeletePresentation(presentation.PresentationId);
                }

                //delete all users from course
                DeleteAllFromCourseUsers(dbEntry.CourseId);

                //delete all comments
                foreach (Comment comment in context.Comments
                    .Where(c => c.ForId == dbEntry.CourseId && c.For == "Course"))
                {
                    _ = DeleteComment(comment.Id);
                }

                //delete course from groups
                foreach (GroupCourses group in context.GroupCourses
                            .Where(g => g.CourseId == dbEntry.CourseId))
                {
                    RemoveCourseFromGroup(group.GroupId, dbEntry.CourseId);
                }

                context.Courses.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public void SavePresentation(Presentation presentation)
        {
            if (presentation.PresentationId == 0)
            {
                context.Attach(presentation.CreatedBy);
                presentation.CreatedAt = DateTime.Now;
                presentation.UpdatedAt = DateTime.Now;
                context.Presentations.Add(presentation);
            }
            else
            {
                Presentation dbEntry = context.Presentations
                    .FirstOrDefault(p => p.PresentationId == presentation.PresentationId);
                if (dbEntry != null)
                {
                    dbEntry.UpdatedAt = DateTime.Now;
                    dbEntry.Title = presentation.Title;
                    dbEntry.Part = presentation.Part;
                    dbEntry.CreatedBy = presentation.CreatedBy;
                    dbEntry.DateAdded = presentation.DateAdded;
                    dbEntry.DateModified = presentation.DateModified;
                    dbEntry.Description = presentation.Description;
                    dbEntry.Status = presentation.Status;
                    AddRepresentation(presentation.Representations, presentation);
                    dbEntry.Representations = presentation.Representations;
                    dbEntry.VideoId = presentation.VideoId;
                }
            }
            context.SaveChanges();
        }

        public Presentation DeletePresentation(int presentationId)
        {
            Presentation dbEntry = context.Presentations
                .FirstOrDefault(p => p.PresentationId == presentationId);
            if (dbEntry != null)
            {
                //delete presentation video
                Video pVideo = context.Videos
                    .FirstOrDefault(v => v.Id == dbEntry.VideoId);
                if (pVideo != null)
                {
                    _ = DeleteVideo(pVideo.Id);
                }

                //delete presentation files
                foreach(PresentationFiles pFile in context.PresentationFiles
                    .Where(f => f.PresentationId == dbEntry.PresentationId))
                {
                    DeleteFile(pFile.FileId);
                }

                //delete representations
                foreach (Representation representation in dbEntry.Representations)
                {
                    _ = DeleteRepresentation(representation.RepresentationId);
                }

                //delete all comments
                foreach (Comment comment in context.Comments
                    .Where(c => c.ForId == dbEntry.PresentationId && c.For == "Presentation"))
                {
                    _ = DeleteComment(comment.Id);
                }

                context.Presentations.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public void AddPresentation(ICollection<Presentation> presentations, Course course)
        {
            foreach (Presentation p in presentations)
            {
                if (context.Presentations
                    .FirstOrDefault(pr => pr.PresentationId == p.PresentationId) == null)
                {
                    context.Attach(course);
                    SavePresentation(p);
                }
            }
        }

        public void AddUserToCourse(string userId, int courseId)
        {
            CourseUsers courseUsers = new CourseUsers
            {
                CourseId = courseId,
                AppUserId = userId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            context.CourseUsers.Add(courseUsers);
            context.SaveChanges();

            //add notification
            NewNotification(userId, "UserJoinCourse", "Course", courseId);
        }

        public void DeleteAllFromCourseUsers(int courseId)
        {
            foreach(CourseUsers cu in context.CourseUsers
                .Where(c => c.CourseId == courseId))
            {
                if(cu != null)
                {
                    context.CourseUsers.Remove(cu);
                }
            }
            context.SaveChanges();
        }

        public void DeleteUserFromCourse(string userId, int courseId)
        {
            CourseUsers dbEntry = context.CourseUsers
                .FirstOrDefault(cu => cu.AppUserId == userId &&
                cu.CourseId == courseId);
            if(dbEntry != null)
            {
                Course course = context.Courses
                    .FirstOrDefault(c => c.CourseId == courseId);
                foreach(Presentation p in course.Presentations)
                {
                    Representation deleteRep = p.Representations
                        .FirstOrDefault(r => r.CreatedBy.Id == userId);
                    DeleteRepresentation(deleteRep.RepresentationId);
                }
                context.CourseUsers.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void SaveRepresentation(Representation representation)
        {
            if (representation.RepresentationId == 0)
            {
                context.Attach(representation.CreatedBy);
                representation.CreatedAt = DateTime.Now;
                representation.UpdatedAt = DateTime.Now;
                context.Representations.Add(representation);
            }
            else
            {
                Representation dbEntry = context.Representations
                    .FirstOrDefault(r => r.RepresentationId == representation.RepresentationId);
                if (dbEntry != null)
                {
                    dbEntry.UpdatedAt = DateTime.Now;
                    dbEntry.Title = representation.Title;
                    dbEntry.CreatedBy = representation.CreatedBy;
                    dbEntry.DateAdded = representation.DateAdded;
                    dbEntry.Status = representation.Status;
                    dbEntry.VideoId = representation.VideoId;
                }
            }
            context.SaveChanges();
        }

        public Representation DeleteRepresentation(int representationId)
        {
            Representation dbEntry = context.Representations
                .FirstOrDefault(r => r.RepresentationId == representationId);
            if (dbEntry != null)
            {
                //delete representation video
                Video rVideo = context.Videos
                    .FirstOrDefault(v => v.Id == dbEntry.VideoId);
                if (rVideo != null)
                {
                    _ = DeleteVideo(rVideo.Id);
                }

                //delete all comments
                foreach (Comment comment in context.Comments
                    .Where(c => c.ForId == dbEntry.RepresentationId && c.For == "Representation"))
                {
                    _ = DeleteComment(comment.Id);
                }

                context.Representations.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public void AddRepresentation(ICollection<Representation> representations, 
            Presentation presentation)  
        {
            foreach (Representation r in representations)
            {
                if (context.Representations
                    .FirstOrDefault(rp => rp.RepresentationId == r.RepresentationId) == null)
                {
                    context.Attach(presentation);
                    SaveRepresentation(r);
                }
            }
        }

        public void SaveVideo(Video video)
        {
            if (video.Id == 0)
            {
                video.CreatedAt = DateTime.Now;
                video.UpdatedAt = DateTime.Now;
                context.Videos.Add(video);
            }
            else
            {
                Video dbEntry = context.Videos
                    .FirstOrDefault(v => v.Id == video.Id);
                if (dbEntry != null)
                {
                    dbEntry.UpdatedAt = DateTime.Now;
                    dbEntry.Title = video.Title;
                    dbEntry.CreatedBy = video.CreatedBy;
                    dbEntry.For = video.For;
                    dbEntry.ForId = video.ForId;
                    dbEntry.Status = video.Status;
                    dbEntry.DateAdded = video.DateAdded;
                    dbEntry.Views = video.Views;
                    dbEntry.VideoPath = video.VideoPath;
                }
            }
            context.SaveChanges();

            //add notification
            if (video.For == "Representation")
            {
                NewNotification(video.CreatedBy, "NewRepresentation", "Video", video.Id);
            }
        }

        public void SaveVideoShared(int videoId, string userId)
        {
            VideoShared shared = context.VideoShared
                .FirstOrDefault(v => v.VideoId == videoId && v.UserId == userId);

            if (shared == null)
            {
                VideoShared dbEntry = new VideoShared
                {
                    UserId = userId,
                    VideoId = videoId
                };
                context.VideoShared.Add(dbEntry);
            }
            context.SaveChanges();
        }

        public void DeleteVideoShared(int videoId, string userId)
        {
            VideoShared dbEntry = context.VideoShared
                .FirstOrDefault(v => v.VideoId == videoId && v.UserId == userId);

            if (dbEntry != null)
            {
                context.VideoShared.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void DeleteVideoSharedId(int id)
        {
            VideoShared dbEntry = context.VideoShared
                .FirstOrDefault(v => v.id == id);

            if (dbEntry != null)
            {
                context.VideoShared.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public Video DeleteVideo(int videoId)
        {
            Video dbEntry = context.Videos
                .FirstOrDefault(v => v.Id == videoId);
            if (dbEntry != null)
            {
                //delete all comments
                foreach (Comment comment in context.Comments
                    .Where(c => c.ForId == dbEntry.Id && c.For == "Video"))
                {
                    _ = DeleteComment(comment.Id);
                }

                //delete all likes
                foreach (Likes like in context.Likes
                    .Where(l => l.For == "Video" && l.ForId == dbEntry.Id))
                {
                    DeleteLike(like.Id);
                }

                //delete all dislikes
                foreach (Dislikes dislike in context.Dislikes
                    .Where(d => d.For == "Video" && d.ForId == dbEntry.Id))
                {
                    DeleteDislike(dislike.Id);
                }

                //*delete all videoshared
                /*
                IQueryable<VideoShared> videoShared = context.VideoShared
                    .Where(v => v.VideoId == dbEntry.Id);
                foreach (VideoShared shared in videoShared)
                {
                    DeleteVideoSharedId(shared.id);
                }*/

                context.Videos.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public void SaveComment(Comment comment)
        {
            if(comment.Id == 0)
            {
                string forTmp = comment.For;
                string userTmp = comment.CreatedBy;
                int forIdTmp = comment.ForId;

                comment.CreatedAt = DateTime.Now;
                comment.UpdatedAt = DateTime.Now;
                context.Comments.Add(comment);
                context.SaveChanges();

                NewNotification(comment.CreatedBy, "NewComment", comment.For,
                    comment.ForId);
            }
        }

        public Comment DeleteComment (int commentId)
        {
            Comment dbEntry = context.Comments.
                FirstOrDefault(c => c.Id == commentId);
            if(dbEntry != null)
            {
                //delete all likes
                foreach (Likes like in context.Likes
                    .Where(l => l.For == "Comment" && l.ForId == dbEntry.Id))
                {
                    if(like != null)
                    {
                        context.Likes.Remove(like);
                    }
                }

                //delete all dislikes
                foreach (Dislikes dislike in context.Dislikes
                    .Where(d => d.For == "Comment" && d.ForId == dbEntry.Id))
                {
                    if (dislike != null)
                    {
                        context.Dislikes.Remove(dislike);
                    }
                }

                foreach(Comment subComm in context.Comments)
                {
                    if(subComm.CommentId == dbEntry.Id)
                    {
                        context.Comments.Remove(subComm);
                    }
                }
                context.Comments.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public void AddLike(Likes like)
        {
            if(like.Id == 0)
            {
                //remove dislike added by user like.userId
                Dislikes dislike = context.Dislikes
                    .FirstOrDefault(d => d.For == like.For &&
                        d.ForId == like.ForId && d.UserId == like.UserId);
                if(dislike != null)
                {
                    DeleteDislike(dislike.Id);
                }

                like.CreatedAt = DateTime.Now;
                like.UpdatedAt = DateTime.Now;
                context.Likes.Add(like);
                context.SaveChanges();

                if (like.For == "Video")
                {
                    NewNotification(like.UserId, "LikeDislikeVideo", like.For, like.ForId);
                }
                else if (like.For == "Comment")
                {
                    NewNotification(like.UserId, "LikeDislikeComment", like.For, like.ForId);
                }
            }
        }

        public void DeleteLike(int likeId)
        {
            Likes dbEntry = context.Likes
                .FirstOrDefault(l => l.Id == likeId);
            if(dbEntry != null)
            {
                context.Likes.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void AddDislike(Dislikes dislike)
        {
            if(dislike.Id == 0)
            {
                //remove like added by user dislike.UserId
                Likes like = context.Likes
                    .FirstOrDefault(l => l.For == dislike.For &&
                        l.ForId == dislike.ForId && l.UserId == dislike.UserId);
                if(like != null)
                {
                    DeleteLike(like.Id);
                }

                dislike.CreatedAt = DateTime.Now;
                dislike.UpdatedAt = DateTime.Now;
                context.Dislikes.Add(dislike);
                context.SaveChanges();


                if (dislike.For == "Video")
                {
                    NewNotification(dislike.UserId, "LikeDislikeVideo", dislike.For, dislike.ForId);
                }
                else if (dislike.For == "Comment")
                {
                    NewNotification(dislike.UserId, "LikeDislikeComment", dislike.For, dislike.ForId);
                }
            }
        }

        public void DeleteDislike(int dislikeId)
        {
            Dislikes dbEntry = context.Dislikes
                .FirstOrDefault(d => d.Id == dislikeId);
            if(dbEntry != null)
            {
                context.Dislikes.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        // ---------CHAT--------------
        public void NewChat(Chat chat)
        {
            //if chat not exists create new else return this chat
            if (chat.ChatId == 0)
            {
                context.Chats.Add(chat);
                chat.CreatedAt = DateTime.Now;
                chat.UpdatedAt = DateTime.Now;
                context.SaveChanges();
            }
        }

        public void EditGroupChat(GroupChat chat)
        {
            //if not exists create new
            if(chat.ChatId == 0)
            {
                context.GroupChats.Add(chat);
                chat.CreatedAt = DateTime.Now;
                chat.UpdatedAt = DateTime.Now;
                context.SaveChanges();
            }
            else
            {
                GroupChat dbEntry = context.GroupChats
                    .FirstOrDefault(c => c.ChatId == chat.ChatId);
                dbEntry.UpdatedAt = DateTime.Now;
                dbEntry.ChatPhotoPath = chat.ChatPhotoPath;
                dbEntry.Title = chat.Title;
                context.SaveChanges();
            }
        }

        public void AddUserToGroupChat (int chatId, string userId)
        {
            GroupChatUsers dbEntry = new GroupChatUsers
            {
                GroupChatId = chatId,
                UserId = userId
            };
            GroupChat chat = context.GroupChats
                .FirstOrDefault(c => c.ChatId == chatId);

            context.Attach(chat);
            dbEntry.CreatedAt = DateTime.Now;
            dbEntry.UpdatedAt = DateTime.Now;
            context.GroupChatUsers.Add(dbEntry);
            context.SaveChanges();
        }

        public void RemoveUserFromGroupChat (int id)
        {
            GroupChatUsers dbEntry = context.GroupChatUsers
                .FirstOrDefault(cu => cu.id == id);
            context.GroupChatUsers.Remove(dbEntry);
            context.SaveChanges();
        }

        public void NewChatMessage (ChatMessage msg, Chat chat)
        {
            if (msg.Id == 0)
            {
                msg.DateAdded = DateTime.Now;
                context.Attach(chat);
                msg.CreatedAt = DateTime.Now;
                msg.UpdatedAt = DateTime.Now;
                context.ChatMessages.Add(msg);
                chat.Messages.Add(msg);
                context.SaveChanges();
            }
        }

        public void NewGroupChatMessage (GroupChatMessage msg, GroupChat chat)
        {
            if (msg.MessageId == 0)
            {
                msg.DateAdded = DateTime.Now;
                context.Attach(chat);
                msg.CreatedAt = DateTime.Now;
                msg.UpdatedAt = DateTime.Now;
                context.GroupChatMessages.Add(msg);
                context.SaveChanges();
            }
        }

        public void AddSeenChat (ChatMessage msg)
        {
            if (msg.Id != 0)
            {
                msg.Seen = true;
                msg.UpdatedAt = DateTime.Now;
                context.SaveChanges();
            }
        }

        public void AddViewToGroupMessage (List<int> ids, string userId, int groupChatId)
        {
            foreach(int id in ids)
            {
                GroupMessageView view = new GroupMessageView
                {
                    UserId = userId,
                    MessageId = id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                context.GroupMessageViews.Add(view);
            }
            context.SaveChanges();
        }

        public void AddViewToGroupMsg (GroupMessageView view)
        {
            if (view.Id == 0)
            {
                view.CreatedAt = DateTime.Now;
                view.UpdatedAt = DateTime.Now;
                context.Add(view);
                context.SaveChanges();
            }
        }

        public void DeleteChatMessage (int msgId)
        {
            ChatMessage dbEntry = context.ChatMessages
                .FirstOrDefault(c => c.Id == msgId);
            if( dbEntry != null)
            {
                context.ChatMessages.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void DeleteGroupChatMessage (int msgId)
        {
            GroupChatMessage dbEntry = context.GroupChatMessages
                .FirstOrDefault(c => c.MessageId == msgId);
            if (dbEntry != null)
            {
                //delete all group message views
                foreach (GroupMessageView view in dbEntry.Views)
                {
                    context.GroupMessageViews.Remove(view);
                    context.SaveChanges();
                }

                context.GroupChatMessages.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void DeleteGroupChat (int chatId)
        {
            GroupChat dbEntry = context.GroupChats
                .FirstOrDefault(c => c.ChatId == chatId);
            if(dbEntry != null)
            {
                //delete all chat users
                foreach (GroupChatUsers user in dbEntry.Users)
                {
                    RemoveUserFromGroupChat(user.id);
                }

                //delete all chat messages
                foreach (GroupChatMessage msg in dbEntry.Messages)
                {
                    DeleteGroupChatMessage(msg.MessageId);
                }
                    
                context.GroupChats.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void ClearChat(string userId, int chatId)
        {
            Chat chat = context.Chats
                .FirstOrDefault(c => c.ChatId == chatId);
            if (chat != null)
            {
                if (chat.User1Id == userId)
                {
                    chat.User1Checkpoint = DateTime.Now;
                    chat.UpdatedAt = DateTime.Now;
                    context.SaveChanges();
                }
                else
                {
                    chat.User2Checkpoint = DateTime.Now;
                    chat.UpdatedAt = DateTime.Now;
                    context.SaveChanges();
                }

                CheckpointDelete(chat);
            }
        }

        public void CheckpointDelete (Chat chat)
        {
            DateTime date = (chat.User1Checkpoint >= chat.User2Checkpoint) ?
                chat.User1Checkpoint : chat.User2Checkpoint;

            foreach (ChatMessage msg in chat.Messages.Where(c => c.DateAdded >= date))
            {   
                DeleteChatMessage(msg.Id);
                context.SaveChanges();
            }

            chat.UpdatedAt = DateTime.Now;
            context.SaveChanges();
        }

        //notifications
        public void NewNotification (string userId, string subject, string For,
            int forId)
        {
            Notification notification = context.Notifications
                .FirstOrDefault(n => n.Subject == subject && n.For == For
                    && n.ForId == forId);

            if (notification != null)
            {
                AppUser user = userManager.Users
                    .FirstOrDefault(u => u.Id == userId);
                string userName = (user.FirstName == null && user.LastName == null) ?
                    user.UserName : $"{user.FirstName} {user.LastName}";
                string forName = "";
                int totalUsers = 0;

                NotificationCard newNotificationCard = new NotificationCard
                {
                    CreatedBy = user.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                if (notification.Subject == "NewComment")
                {
                    switch (notification.For)
                    {
                        case "Video":
                            forName = context.Videos
                                .FirstOrDefault(v => v.Id == notification.ForId).Title;
                            totalUsers = context.Comments.Where(c => c.For == "Video"
                                && c.ForId == notification.ForId)
                                .Select(c => c.CreatedBy).Distinct().Count() - 1;
                            break;
                        case "Course":
                            forName = context.Courses
                                .FirstOrDefault(c => c.CourseId == notification.ForId).Title;
                            totalUsers = context.Comments.Where(c => c.For == "Course"
                                && c.ForId == notification.ForId)
                                .Select(c => c.CreatedBy).Distinct().Count() - 1;
                            break;
                    }

                    newNotificationCard.Msg = (totalUsers >= 1) ?
                     $"{userName} and {totalUsers} others comment on {notification.For} '{forName}'" :
                     $"{userName} comment on {notification.For} '{forName}'";

                    context.Attach(notification);
                    context.NotificationCards.Add(newNotificationCard);
                    notification.NotificationCards.Add(newNotificationCard);
                    //set datetime of last notification
                    notification.NotificationDateAdded = DateTime.Now;
                    context.SaveChanges();
                }
                
                else if (notification.Subject == "LikeDislikeVideo")
                {
                    forName = context.Videos
                                .FirstOrDefault(v => v.Id == notification.ForId).Title;

                    //all likes and dislikes on video
                    totalUsers = context.Likes
                        .Where(l => l.For == "Video" && l.ForId == notification.ForId)
                        .Select(c => c.UserId).Count();
                    totalUsers += context.Dislikes
                        .Where(d => d.For == "Video" && d.ForId == notification.ForId)
                        .Select(c => c.UserId).Count();

                    newNotificationCard.Msg = (totalUsers > 1) ?
                     $"{userName} and {totalUsers} others reacted on your video '{forName}'" :
                     $"{userName} reacted on your video '{forName}'";

                    context.Attach(notification);
                    context.NotificationCards.Add(newNotificationCard);
                    notification.NotificationCards.Add(newNotificationCard);
                    //set datetime of last notification
                    notification.NotificationDateAdded = DateTime.Now;
                    context.SaveChanges();
                }

                else if (notification.Subject == "LikeDislikeComment")
                {
                    //notification.for = "Comment" 
                    //notification.forID = CommentId
                    //get comment where commentId == notification.forID
                    Comment comment = context.Comments
                        .FirstOrDefault(c => c.Id == notification.ForId);

                    //all likes and dislikes for comment where comment.id = notification.forId
                   totalUsers = context.Likes
                        .Where(l => l.For == "Comment" && l.ForId == notification.ForId)
                        .Select(c => c.UserId).Count() - 1;
                    totalUsers += context.Dislikes
                        .Where(d => d.For == "Comment" && d.ForId == notification.ForId)
                        .Select(c => c.UserId).Count() - 1;

                    string commentContent = (comment.Content.Length > 25) ?
                        comment.Content.Substring(0, 25) + "..." : comment.Content;

                    newNotificationCard.Msg = (totalUsers > 1) ?
                     $"{userName} and {totalUsers} others reacted on your comment '{commentContent}'" :
                     $"{userName} reacted on your comment for '{commentContent}'";

                    context.Attach(notification);
                    context.NotificationCards.Add(newNotificationCard);
                    notification.NotificationCards.Add(newNotificationCard);
                    //set datetime of last notification
                    notification.NotificationDateAdded = DateTime.Now;
                    context.SaveChanges();
                    
                }
                /*else if (notification.Subject == "NewVideo")
                {
                    //for all user on course
                    //if video for == course or presentation
                }*/
                else if (notification.Subject == "UserJoinCourse")
                {
                    Course course = context.Courses
                        .FirstOrDefault(c => c.CourseId == notification.ForId);

                    newNotificationCard.Msg = 
                        $"{userName} has joined the Course '{course.Title}'";

                    context.Attach(notification);
                    context.NotificationCards.Add(newNotificationCard);
                    notification.NotificationCards.Add(newNotificationCard);
                    //set datetime of last notification
                    notification.NotificationDateAdded = DateTime.Now;
                    context.SaveChanges();
                }
                else if (notification.Subject == "RepresentationRating")
                {
                    //for user who created representation
                    Video representationVideo = context.Videos
                        .FirstOrDefault(v => v.Id == notification.ForId);

                    if (representationVideo != null)
                    {
                        Representation representation = context.Representations
                            .FirstOrDefault(r => r.RepresentationId == representationVideo.ForId);

                        newNotificationCard.Msg =
                        $"Your representation {representation.Title} has been rated.";

                        context.Attach(notification);
                        context.NotificationCards.Add(newNotificationCard);
                        notification.NotificationCards.Add(newNotificationCard);
                        //set datetime of last notification
                        notification.NotificationDateAdded = DateTime.Now;
                        context.SaveChanges();
                    }
                }
                else if (notification.Subject == "NewRepresentation")
                {
                    //for user who created course
                    Video representationVideo = context.Videos
                        .FirstOrDefault(v => v.Id == notification.ForId);

                    if (representationVideo != null)
                    {
                        Representation representation = context.Representations
                            .FirstOrDefault(r => r.RepresentationId == representationVideo.ForId);

                        newNotificationCard.Msg =
                        $"{userName} added a new representation '{representation.Title}'";

                        context.Attach(notification);
                        context.NotificationCards.Add(newNotificationCard);
                        notification.NotificationCards.Add(newNotificationCard);
                        //set datetime of last notification
                        notification.NotificationDateAdded = DateTime.Now;
                        context.SaveChanges();
                    }
                }

                notification.UpdatedAt = DateTime.Now;
                context.SaveChanges();
            }
            else
            {
                notification = new Notification
                {
                    Subject = subject,
                    For = For,
                    ForId = forId,
                    NotificationDateAdded = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                context.Notifications.Add(notification);
                context.SaveChanges();

                NewNotification(userId, subject, For, forId);
            }
        }

        public void NewNotificationCardView(int notificationCarId, string userId)
        {
            NotificationCard card = context.NotificationCards
                .FirstOrDefault(c => c.NotificationCardId == notificationCarId);

            NotificationViews view = new NotificationViews
            {
                UserId = userId
            };
            context.Attach(card);
            view.CreatedAt = DateTime.Now;
            view.UpdatedAt = DateTime.Now;
            context.NotificationViews.Add(view);
            context.SaveChanges();
            card.UpdatedAt = DateTime.Now;
            card.NotificationViews.Add(view);
            context.SaveChanges();
        }

        //groups
        public int SaveGroup (Group group)
        {
            if (group.GroupId == 0)
            {
                context.Attach(group.CreatedBy);
                group.CreatedAt = DateTime.Now;
                group.UpdatedAt = DateTime.Now;
                context.Groups.Add(group);

            }
            else
            {
                Group dbEntry = context.Groups
                    .FirstOrDefault(g => g.GroupId == group.GroupId);
                dbEntry.UpdatedAt = DateTime.Now;
                dbEntry.Name = group.Name;
                dbEntry.Description = group.Description;
                dbEntry.GroupPhotoPath = group.GroupPhotoPath;
            }
            context.SaveChanges();

            return group.GroupId;
        }

        public void AddUserToGroup (string userId, int groupId)
        {
            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == userId);

            if (user != null)
            {
                Group group = context.Groups
                    .FirstOrDefault(g => g.GroupId == groupId);
                if (group != null)
                {
                    GroupUsers dbEntry = new GroupUsers
                    {
                        GroupId = group.GroupId,
                        AppUserId = user.Id
                    };
                    dbEntry.CreatedAt = DateTime.Now;
                    dbEntry.UpdatedAt = DateTime.Now;
                    context.GroupUsers.Add(dbEntry);
                    context.SaveChanges();
                    group.UpdatedAt = DateTime.Now;
                    context.SaveChanges();
                }
            }
        }

        public void AddCourseToGroup(int courseId, int groupId)
        {
            Course course = context.Courses
                .FirstOrDefault(c => c.CourseId == courseId);

            if (course != null)
            {
                Group group = context.Groups
                    .FirstOrDefault(g => g.GroupId == groupId);
                if (group != null)
                {
                    GroupCourses dbEntry = new GroupCourses
                    {
                        GroupId = group.GroupId,
                        CourseId = course.CourseId
                    };
                    dbEntry.CreatedAt = DateTime.Now;
                    dbEntry.UpdatedAt = DateTime.Now;
                    context.GroupCourses.Add(dbEntry);
                    context.SaveChanges();
                    group.UpdatedAt = DateTime.Now;
                    context.SaveChanges();
                }
            }
        }

        public void DeleteGroup (int groupId)
        {
            Group dbEntry = context.Groups
                .FirstOrDefault(g => g.GroupId == groupId);

            if (dbEntry != null)
            {
                //delete all users from group
                foreach(GroupUsers user in dbEntry.Users)
                {
                    RemoveUserFromGroup(dbEntry.GroupId, user.AppUserId);
                }

                //delete all courses from group
                foreach (GroupCourses course in dbEntry.Courses)
                {
                    RemoveCourseFromGroup(dbEntry.GroupId, course.CourseId);
                }

                //delete group
                context.Groups.Remove(dbEntry);
                context.SaveChanges();
            }
        }

        public void RemoveUserFromGroup (int groupId, string userId)
        {
            GroupUsers dbEntry = context.GroupUsers
                .FirstOrDefault(g => g.AppUserId == userId && g.GroupId == groupId);

            if(dbEntry != null)
            {
                context.GroupUsers.Remove(dbEntry);
                context.SaveChanges();
            }

            Group group = context.Groups.FirstOrDefault(g => g.GroupId == groupId);
            group.UpdatedAt = DateTime.Now;
            context.SaveChanges();
        }

        public void RemoveCourseFromGroup (int groupId, int coursId)
        {
            GroupCourses dbEntry = context.GroupCourses
                .FirstOrDefault(g => g.GroupId == groupId && g.CourseId == coursId);

            if (dbEntry != null)
            {
                context.GroupCourses.Remove(dbEntry);
                context.SaveChanges();
            }

            Group group = context.Groups.FirstOrDefault(g => g.GroupId == groupId);
            group.UpdatedAt = DateTime.Now;
            context.SaveChanges();
        }

        //files
        public void SaveFile (PresentationFiles file)
        {
            if (file.FileId == 0)
            {
                file.CreatedAt = DateTime.Now;
                file.UpdatedAt = DateTime.Now;
                context.PresentationFiles.Add(file);
            }
            else
            {
                PresentationFiles dbEntry = context.PresentationFiles
                    .FirstOrDefault(f => f.FileId == file.FileId);
                if (dbEntry != null)
                {
                    dbEntry.UpdatedAt = DateTime.Now;
                    dbEntry.Title = file.Title;
                    dbEntry.FilePath = file.FilePath;
                }
            }
            context.SaveChanges();
        }

        public void DeleteFile(int FileId)
        {
            PresentationFiles dbEntry = context.PresentationFiles
                .FirstOrDefault(f => f.FileId == FileId);
            if (dbEntry != null)
            {
                context.PresentationFiles.Remove(dbEntry);
                context.SaveChanges();
            }
        }
    }
}

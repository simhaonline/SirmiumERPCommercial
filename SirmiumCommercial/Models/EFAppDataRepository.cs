using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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

        public IQueryable<Group> Groups => context.Groups;
        public IQueryable<Course> Courses => context.Courses.Include(p => p.Presentations);
        public IQueryable<Presentation> Presentations => context.Presentations
            .Include(r => r.Representations);
        public IQueryable<Representation> Representations => context.Representations;
        public IQueryable<CourseUsers> CourseUsers => context.CourseUsers;
        public IQueryable<GroupUsers> GroupUsers => context.GroupUsers;
        public IQueryable<Video> Videos => context.Videos;
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

        public void SaveCourse(Course course)
        {
            if (course.CourseId == 0)
            {
                context.Attach(course.CreatedBy);
                context.Courses.Add(course);
            }
            else
            {
                Course dbEntry = context.Courses
                    .FirstOrDefault(c => c.CourseId == course.CourseId);
                if (dbEntry != null)
                {
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
                _ = DeleteVideo(courseVideo.Id);

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
                context.Presentations.Add(presentation);
            }
            else
            {
                Presentation dbEntry = context.Presentations
                    .FirstOrDefault(p => p.PresentationId == presentation.PresentationId);
                if (dbEntry != null)
                {
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
                _ = DeleteVideo(pVideo.Id);

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
                AppUserId = userId
            };

            context.CourseUsers.Add(courseUsers);
            context.SaveChanges();
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
                context.Representations.Add(representation);
            }
            else
            {
                Representation dbEntry = context.Representations
                    .FirstOrDefault(r => r.RepresentationId == representation.RepresentationId);
                if (dbEntry != null)
                {
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
                _ = DeleteVideo(rVideo.Id);

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
                context.Videos.Add(video);
            }
            else
            {
                Video dbEntry = context.Videos
                    .FirstOrDefault(v => v.Id == video.Id);
                if (dbEntry != null)
                {
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

                context.Videos.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public void SaveComment(Comment comment)
        {
            if(comment.Id == 0)
            {
                context.Comments.Add(comment);
            }

            context.SaveChanges();
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

                context.Likes.Add(like);
                context.SaveChanges();
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

                context.Dislikes.Add(dislike);
                context.SaveChanges();
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

        public void NewChat(Chat chat)
        {
            //if chat not exists create new else return this chat
            if (chat.ChatId == 0)
            {
                context.Chats.Add(chat);
                context.SaveChanges();
            }
        }

        public void EditGroupChat(GroupChat chat)
        {
            //if not exists create new
            if(chat.ChatId == 0)
            {
                context.GroupChats.Add(chat);
                context.SaveChanges();
            }
            else
            {
                GroupChat dbEntry = context.GroupChats
                    .FirstOrDefault(c => c.ChatId == chat.ChatId);
                dbEntry.ChatPhotoPath = chat.ChatPhotoPath;
                dbEntry.Title = chat.Title;
                context.SaveChanges();
            }
        }

        public void AddUserToGroupChat (int chatId, string userId)
        {
            GroupChatUsers dbEntry = new GroupChatUsers
            {
                UserId = userId
            };
            GroupChat chat = context.GroupChats
                .FirstOrDefault(c => c.ChatId == chatId);

            context.Attach(chat);
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
                context.GroupChatMessages.Add(msg);
                context.SaveChanges();
            }
        }

        public void AddSeenChat (ChatMessage msg)
        {
            if (msg.Id != 0)
            {
                msg.Seen = true;
                context.SaveChanges();
            }
        }

        public void AddViewToGroupMessage (int groupMsgId, string userId, int groupChatId)
        {
            GroupMessageView dbEntry = new GroupMessageView
            {
                UserId = userId
            };
            GroupChatMessage msg = context.GroupChatMessages
                .FirstOrDefault(m => m.MessageId == groupMsgId);
            context.Attach(msg);
            context.GroupMessageViews.Add(dbEntry);
            context.SaveChanges();

            //check: if all users saw the message
            GroupChat chat = context.GroupChats
                .FirstOrDefault(c => c.ChatId == groupChatId);
            msg = context.GroupChatMessages
                .FirstOrDefault(m => m.MessageId == groupMsgId);

            if (chat.Users.Count() == msg.Views.Count())
            {
                msg.Seen = true;
                foreach (GroupMessageView view in msg.Views)
                {
                    context.GroupMessageViews.Remove(view);
                }
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
                    context.SaveChanges();
                }
                else
                {
                    chat.User2Checkpoint = DateTime.Now;
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
        }
    }
}

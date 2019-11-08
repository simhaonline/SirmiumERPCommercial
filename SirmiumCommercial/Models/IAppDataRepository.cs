using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models
{
    public interface IAppDataRepository
    {
        IQueryable<Group> Groups { get; }

        IQueryable<Course> Courses { get; }

        IQueryable<Presentation> Presentations { get; }

        IQueryable<Representation> Representations { get; }

        IQueryable<CourseUsers> CourseUsers { get; }

        IQueryable<GroupUsers> GroupUsers { get; }

        IQueryable<Video> Videos { get; }

        IQueryable<Comment> Comments { get; }

        IQueryable<Likes> Likes { get; }

        IQueryable<Dislikes> Dislikes { get; }

        IQueryable<Chat> Chats { get; }

        IQueryable<ChatMessage> ChatMessages { get; }

        IQueryable<GroupChat> GroupChats { get; }

        IQueryable<GroupChatUsers> GroupChatUsers { get; }

        IQueryable<GroupChatMessage> GroupChatMessages { get; }

        IQueryable<GroupMessageView> GroupMessageViews { get; }

        IEnumerable<Notification> Notifications { get; }

        IEnumerable<NotificationCard> NotificationCards { get; }

        IEnumerable<NotificationViews> NotificationViews { get; }

        void SaveCourse(Course course);

        Course DeleteCourse(int courseId);

        void SavePresentation(Presentation presentation);

        Presentation DeletePresentation(int presentationId);

        void AddUserToCourse(string userId, int courseId);

        void DeleteAllFromCourseUsers(int courseId);

        void DeleteUserFromCourse(string userId, int courseId);

        void SaveRepresentation(Representation representation);

        Representation DeleteRepresentation(int representationId);

        void AddRepresentation(ICollection<Representation> representations,
            Presentation presentation);

        //-------- Comment---------
        void SaveVideo(Video video);

        Video DeleteVideo(int videoId);

        void SaveComment(Comment comment);

        Comment DeleteComment(int commentId);

        //------- Like / Dislike ------------
        void AddLike(Likes like);

        void DeleteLike(int likeId);

        void AddDislike(Dislikes dislike);

        void DeleteDislike(int dislikeId);

        //-------- Chat -------
        void NewChat(Chat chat);

        void EditGroupChat(GroupChat chat);

        void AddUserToGroupChat(int chatId, string userId);

        void RemoveUserFromGroupChat(int id);

        void NewChatMessage(ChatMessage msg, Chat chat);

        void NewGroupChatMessage(GroupChatMessage msg, GroupChat chat);

        void AddSeenChat(ChatMessage msg);

        void AddViewToGroupMessage(int groupMsgId, string userId, int groupChatId);

        void DeleteChatMessage(int msgId);

        void DeleteGroupChatMessage(int msgId);

        void DeleteGroupChat(int chatId);

        void ClearChat(string userId, int chatId);

        void CheckpointDelete(Chat chat);

        //------ Notifications -----
        void NewNotification(string userId, string subject, string For,
            int forId);

        void NewNotificationCardView(int notificationCarId, string userId);
        //void AddNotification(Notification notification);

        // void DeleteNotification(int notificationId);

        //void NewNotificationView(NotificationViews notifivationView);

        //void AddNotificationViews(ICollection<NotificationViews> notificationViews,
        //  Notification notification);

        // void DeleteNotificationView(int viewId);

        //void EditNotification(Notification newNotification, Notification oldNotification);

        //void ClearNotificationViews(int notificationId);
    }
}

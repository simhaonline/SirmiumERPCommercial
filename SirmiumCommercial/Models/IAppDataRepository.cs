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

        IQueryable<PresentationFiles> PresentationFiles { get; }

        IQueryable<Representation> Representations { get; }

        IQueryable<CourseUsers> CourseUsers { get; }

        IQueryable<GroupUsers> GroupUsers { get; }

        IEnumerable<GroupCourses> GroupCourses { get; }

        IQueryable<Video> Videos { get; }

        IEnumerable<VideoShared> VideoShareds { get; }

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

        //--------Video------------
        void SaveVideo(Video video);

        Video DeleteVideo(int videoId);

        void SaveVideoShared(int videoId, string userId);

        void DeleteVideoSharedId(int id);

        void DeleteVideoShared(int videoId, string userId);

        //-------- Comment---------
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

        void AddViewToGroupMessage(List<int> ids, string userId, int groupChatId);

        void AddViewToGroupMsg(GroupMessageView view);

        void DeleteChatMessage(int msgId);

        void DeleteGroupChatMessage(int msgId);

        void DeleteGroupChat(int chatId);

        void ClearChat(string userId, int chatId);

        void CheckpointDelete(Chat chat);

        //------ Notifications -----
        void NewNotification(string userId, string subject, string For,
            int forId);

        void NewNotificationCardView(int notificationCarId, string userId);

        //------ Groups ---------
        int SaveGroup(Group group);

        void AddUserToGroup(string userId, int groupId);

        void AddCourseToGroup(int courseId, int groupId);

        void DeleteGroup(int groupId);

        void RemoveUserFromGroup(int groupId, string userId);

        void RemoveCourseFromGroup(int groupId, int coursId);

        //------ Files ---------
        void SaveFile(PresentationFiles file);

        void DeleteFile(int FileId);

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

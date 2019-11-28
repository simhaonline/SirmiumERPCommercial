using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public DateTime RegistrationDate { get; set; }

        //Status Active or Inactive
        public string Status { get; set; }

        //Profile Photo
        public string ProfilePhotoUrl { get; set; }

        //User Courses
        public ICollection<CourseUsers> Courses { get; set; }
            = new List<CourseUsers>();

        //User Groups
        public ICollection<GroupUsers> Groups { get; set; }
            = new List<GroupUsers>();
    }

    public class Group
    {
        public int GroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string GroupPhotoPath { get; set; }
        public AppUser CreatedBy { get; set; }
        public string CompanyName { get; set; }
        public ICollection<GroupUsers> Users { get; set; }
            = new List<GroupUsers>();
        public ICollection<GroupCourses> Courses { get; set; }
            = new List<GroupCourses>();
    }

    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }
        public DateTime EndDate { get; set; }
        public ICollection<Presentation> Presentations { get; set; }
            = new List<Presentation>();

        //Users on the Courses
        public ICollection<CourseUsers> Users { get; set; }
            = new List<CourseUsers>();

        public string AwardIcon { get; set; }

        //Public -- all users can see
        //Private -- Default value, only creator can see
        //GroupPublic -- TODO
        public string Status { get; set; } = "Private";

        public string Description { get; set; }
        public int VideoId { get; set; }
    }

    public class Presentation
    {
        public int PresentationId { get; set; }
        public string Title { get; set; }
        public int Part { get; set; }
        public string Description { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime DateModified { get; set; }

        //Public -- all users can see
        //Private -- Default value, only creator can see
        public string Status { get; set; } = "Private";

        public ICollection<Representation> Representations { get; set; }
            = new List<Representation>();

        public int VideoId { get; set; }
    }

    public class Representation
    {
        public int RepresentationId { get; set; }
        public string Title { get; set; }
        public AppUser CreatedBy { get; set; }
        public DateTime DateAdded { get; set; }

        //Public -- all users can see
        //Private -- Default value, only creator can see
        public string Status { get; set; } = "Private";

        public int VideoId { get; set; }
        public int Rating { get; set; }
    }

    public class CourseUsers
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string AppUserId { get; set; }
    }

    public class GroupUsers
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public string AppUserId { get; set; }
    }

    public class GroupCourses
    {
        public int Id { get; set; }
        public int GroupId { get; set; }
        public int CourseId { get; set; }
    }

    public class Video
    {
        public int Id { get; set; }
        public string Title { get; set; }
        //User Id
        public string CreatedBy { get; set; }
        //Course, Presentation, Representation or Practice
        public string For { get; set; }
        //CourseId, PresentationId, RepresentationId or PracticeId
        public int ForId { get; set; }
        //Public or Private
        public string Status { get; set; }
        public DateTime DateAdded { get; set; }
        public int Views { get; set; }
        //video path
        public string VideoPath { get; set; }
    }

    public class Comment
    {
        public int Id { get; set; }

        //UserId
        public string CreatedBy { get; set; }

        //Course, Presentation, Representation, Video
        public string For { get; set; }

        //CourseId, PresentationId, VideoId, ...
        public int ForId { get; set; }

        public string Content { get; set; }
        public DateTime DateAdded { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }

        //if this is subcomment
        public int CommentId { get; set; }
    }

    public class Likes
    {
        public int Id { get; set; }
        //Course, Presentation, Representation, Video, Comment
        public string For { get; set; }
        //CourseId, PresentationId, VideoId, ...
        public int ForId { get; set; }
        public string UserId { get; set; }
    }

    public class Dislikes
    {
        public int Id { get; set; }
        //Course, Presentation, Representation, Video, Comment
        public string For { get; set; }
        //CourseId, PresentationId, VideoId, ...
        public int ForId { get; set; }
        public string UserId { get; set; }
    }


    //-----Chat classes-----

    //2 persons chat
    public class Chat
    {
        public int ChatId { get; set; }
        public string User1Id { get; set; }
        public string User2Id { get; set; }

        public ICollection<ChatMessage> Messages { get; set; }
            = new List<ChatMessage>();

        //both checkpoint default = date of first msg
        //after delete chat = current date (delete)
        public DateTime User1Checkpoint { get; set; }
        public DateTime User2Checkpoint { get; set; }
    }

    //2 persons chat messages
    public class ChatMessage
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime DateAdded { get; set; }

        //Txt, Img, Video or Audio
        public string MessageType { get; set; }

        //For Txt type : text
        //For Img, Video, Audio type : path
        public string MessageContent { get; set; }

        //the user saw message
        public bool Seen { get; set; } = false;
    }

    public class GroupChat
    {
        [Key]
        public int ChatId { get; set; }
        public string CreatedBy { get; set; }
        public string Title { get; set; }
        public string ChatPhotoPath { get; set; }

        public ICollection<GroupChatUsers> Users { get; set; }
            = new List<GroupChatUsers>();

        public ICollection<GroupChatMessage> Messages { get; set; }
            = new List<GroupChatMessage>();
    }

    public class GroupChatUsers
    {
        [Key]
        public int id { get; set; }
        public string UserId { get; set; }
    }

    //group chat messages
    public class GroupChatMessage
    {
        [Key]
        public int MessageId { get; set; }
        public string UserId { get; set; }
        public DateTime DateAdded { get; set; }

        //Txt, Img, Video or Audio
        public string MessageType { get; set; }

        //For Txt type : text
        //For Img, Video, Audio type : path
        public string MessageContent { get; set; }

        public ICollection<GroupMessageView> Views { get; set; }
            = new List<GroupMessageView>();

        //if all users saw the massage: seen = true
        public bool Seen { get; set; } = false;
    }

    //users who saw the message
    public class GroupMessageView
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
    }

    //Notifications
    public class Notification
    {
        public int NotificationId { get; set; }

        //NewComment, LikeVideo, DislikeVideo, LikeComment, DislikeComment
        //NewVideo, UserJoinCourse, NewRepresentation, RepresentationRating
        public string Subject { get; set; }

        //Video, Course, Representation
        public string For { get; set; }
        public int ForId { get; set; }

        //------? TODO ?----------
        //CurrentDate - NotificationDateAdded > 1 year => delete Notification ?????
        public DateTime NotificationDateAdded { get; set; }
        public ICollection<NotificationCard> NotificationCards { get; set; }
            = new List<NotificationCard>();
    }

    public class NotificationCard
    {
        public int NotificationCardId { get; set; }

        //UserId
        public string CreatedBy { get; set; }

        //Notification Message
        public string Msg { get; set; }

        public ICollection<NotificationViews> NotificationViews { get; set; }
            = new List<NotificationViews>();
    }

    public class NotificationViews
    {
        public int Id { get; set; }
        public string UserId { get; set; }
    }
}

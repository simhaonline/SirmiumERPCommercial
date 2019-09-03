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

        void SaveVideo(Video video);

        Video DeleteVideo(int videoId);

        void SaveComment(Comment comment);

        Comment DeleteComment(int commentId);
    }
}

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
                context.Videos.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }
    }
}

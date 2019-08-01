using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SirmiumCommercial.Models
{
    public class EFDetailsRepository : IDetailsRepository
    {
        private AppDetailsDbContext context;

        public EFDetailsRepository(AppDetailsDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<UserDetails> UserDetails => context.UsersDetails;
        public IQueryable<Group> Groups => context.Groups;
        public IQueryable<Course> Courses => context.Courses.Include(p => p.Presentations);
        public IQueryable<Presentation> Presentations => context.Presentations;
        public IQueryable<Representation> Representations => context.Representations;

        public void SaveUser (UserDetails user)
        {
            if(user.Id == 0)
            {
                context.Attach(user.User);
                context.UsersDetails.Add(user);
            }
            else
            {
                UserDetails dbEntry = context.UsersDetails
                    .FirstOrDefault(u => u.Id == user.Id);
                if (dbEntry != null)
                {
                    dbEntry.ProfilePhotoURL = user.ProfilePhotoURL;
                }
            }
            context.SaveChanges();
        }

        public UserDetails DeleteUser(int userId)
        {
            UserDetails dbEntry = context.UsersDetails
                .FirstOrDefault(u => u.Id == userId);
            if (dbEntry != null)
            {
                context.UsersDetails.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

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
                    dbEntry.VideoURL = course.VideoURL;
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
                foreach (Presentation presentation in dbEntry.Presentations)
                {
                   Presentation p = DeletePresentation(presentation.PresentationId);
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
                    dbEntry.Representations = presentation.Representations;
                    dbEntry.VideoURL = presentation.VideoURL;
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
                context.Presentations.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public void AddPresentation (ICollection<Presentation> presentations, Course course)
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
    }
}

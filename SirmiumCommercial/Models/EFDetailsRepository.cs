using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models
{
    public class EFDetailsRepository : IDetailsRepository
    {
        private AppDetailsDBContext context;

        public EFDetailsRepository(AppDetailsDBContext ctx)
        {
            context = ctx;
        }

        public IQueryable<UserDetails> UserDetails => context.UsersDetails;
        public IQueryable<Group> Groups => context.Groups;
        public IQueryable<Course> Courses => context.Courses;
        public IQueryable<Presentation> Presentations => context.Presentations;
        public IQueryable<Representation> Representations => context.Representations;

        public void SaveCourse(Course course)
        {
            context.Attach(course.CreatedBy);
            if (course.CourseId == 0)
            {
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
                context.Courses.Remove(dbEntry);
                context.SaveChanges();
            }
            return dbEntry;
        }

        public void SavePresentation(Presentation presentation)
        {
            if (presentation.PresentationId == 0)
            {
                context.Presentations.Add(presentation);
            }
            else
            {
                Presentation dbEntry = context.Presentations
                    .FirstOrDefault(p => p.PresentationId == presentation.PresentationId);
                if (dbEntry != null)
                {
                    dbEntry.Title = presentation.Title;
                    dbEntry.DateAdded = presentation.DateAdded;
                    dbEntry.DateModified = presentation.DateModified;
                    dbEntry.EndDate = presentation.EndDate;
                    dbEntry.Description = presentation.Description;
                    dbEntry.Status = presentation.Status;
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
    }
}

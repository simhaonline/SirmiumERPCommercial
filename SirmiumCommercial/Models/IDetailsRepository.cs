using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models
{
    public interface IDetailsRepository
    {
        IQueryable<UserDetails> UserDetails { get; }

        IQueryable<Group> Groups { get; }

        IQueryable<Course> Courses { get; }

        IQueryable<Presentation> Presentations { get; }

        IQueryable<Representation> Representations { get; }

        void SaveCourse(Course course);

        Course DeleteCourse(int courseId);

        void SavePresentation(Presentation presentation);

        Presentation DeletePresentation(int presentationId);
    }
}

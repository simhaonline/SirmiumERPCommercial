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
    }
}

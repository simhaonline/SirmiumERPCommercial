using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext (DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Group> Groups { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Presentation> Presentations { get; set; }
        public DbSet<Representation> Representations { get; set; }
        public DbSet<CourseUsers> CourseUsers { get; set; }
        public DbSet<GroupUsers> GroupUsers { get; set; }
        public DbSet<Video> Videos { get; set; }
    }
}

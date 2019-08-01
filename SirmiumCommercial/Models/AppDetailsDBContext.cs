using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models
{
    public class AppDetailsDbContext : DbContext
    {
        public AppDetailsDbContext(DbContextOptions<AppDetailsDbContext> options)
            : base(options) { }

        public DbSet<UserDetails> UsersDetails { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Presentation> Presentations { get; set; }
        public DbSet<Representation> Representations { get; set; }
    }
}

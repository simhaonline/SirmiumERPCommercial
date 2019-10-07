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
        public DbSet<Comment> Comments { get; set; } 
        public DbSet<Likes> Likes { get; set; }
        public DbSet<Dislikes> Dislikes { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupChatUsers> GroupChatUsers { get; set; }
        public DbSet<GroupChatMessage> GroupChatMessages { get; set; }
        public DbSet<GroupMessageView> GroupMessageViews { get; set; }
    }
}

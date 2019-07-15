using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models
{
    public static class CourseSeedData
    {
        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            AppDetailsDBContext context = app.ApplicationServices
                .GetRequiredService<AppDetailsDBContext>();
            context.Database.Migrate();
            if (!context.Courses.Any())
            {
                context.Courses.AddRange(
                    new Course
                    {
                        Title = "Cours1",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now
                    },
                    new Course
                    {
                        Title = "Cours2",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now
                    },
                    new Course
                    {
                        Title = "Cours3",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now
                    }
                );
                context.SaveChanges();
            }
        }
    }
}

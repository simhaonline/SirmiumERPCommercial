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
                        Title = "Course1",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        AwardIcon = "fa fa-trophy text-danger",
                        Status = "Public"
                    },
                    new Course
                    {
                        Title = "Course2",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        AwardIcon = "pe-7s-medal text-warning",
                        Status = "Public"
                    },
                    new Course
                    {
                        Title = "Course3",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        AwardIcon = "pe pe-7s-cup text-info",
                        Status = "Public"
                    }
                );
                context.SaveChanges();
            }
        }
    }
}

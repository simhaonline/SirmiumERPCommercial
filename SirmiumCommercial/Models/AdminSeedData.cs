using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SirmiumCommercial.Models
{
    public static class AdminSeedData
    {
        private const string adminUserName = "Admin1";
        private const string adminPassword = "Admin123";
        private const string status = "Active";

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            UserManager<AppUser> userManager = app.ApplicationServices
                .GetRequiredService<UserManager<AppUser>>();

            AppUser user = await userManager.FindByNameAsync(adminUserName);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = adminUserName,
                    RegistrationDate = DateTime.Now,
                    Status = status
                };
                _ = await userManager.CreateAsync(user, adminPassword);
                _ = await userManager.AddToRoleAsync(user, "Admin");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                if (!context.UsersDetails.Any())
                {
                    context.UsersDetails.Add(new UserDetails
                    {
                        User = user
                    });
                    context.SaveChanges();
                }
            }
        }
    }  
}

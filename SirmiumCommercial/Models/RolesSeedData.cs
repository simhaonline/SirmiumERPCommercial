using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace SirmiumCommercial.Models
{
    public static class RolesSeedData
    {
        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            RoleManager<IdentityRole> roleManager = app.ApplicationServices
                .GetRequiredService<RoleManager<IdentityRole>>();

            IdentityRole role = await roleManager.FindByNameAsync("Admin");
            if (role == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            role = await roleManager.FindByNameAsync("Manager");
            if (role == null)
            {
                await roleManager.CreateAsync(new IdentityRole("Manager"));
            }

            role = await roleManager.FindByNameAsync("User");
            if (role == null)
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }
        }
    }
}

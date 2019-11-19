using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SirmiumCommercial.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SirmiumCommercial.Hubs;

namespace SirmiumCommercial
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    Configuration["Data:SirmiumCommercialData:ConnectionString"]));
            services.AddTransient<IAppDataRepository, EFAppDataRepository>();

            services.AddIdentity<AppUser, IdentityRole>(options => {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
                }).AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
            services.AddApplicationInsightsTelemetry(Configuration);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddSignalR();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: null,
                    template: "{Controller=Home}/{Action=Index}/{id?}");
                routes.MapRoute(
                    name: null,
                    template: "{Controller=Main}/{Action=Index}/{id?}");
                routes.MapRoute(
                    name: null,
                    template: "{Controller=Admin}/{Action=Index}/{id?}");
                routes.MapRoute(
                    name: null,
                    template: "{Controller}/{Action}/{id?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<VideoHub>("/videoHub");
                routes.MapHub<CommentsHub>("/commentsHub");
                routes.MapHub<ChatHub>("/chatHub");
                routes.MapHub<NotificationHub>("/notificationHub");
            });

            AppSeedData.EnsurePopulated(app);
        }
    }
}

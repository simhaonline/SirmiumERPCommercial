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
    public class AppSeedData
    {
        private const string adminUserName = "SirmiumERPCommercial";
        private const string adminPassword = "Admin123";
        private const string status = "Active";

        public static async void EnsurePopulated(IApplicationBuilder app)
        {
            //Add roles
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

            UserManager<AppUser> userManager = app.ApplicationServices
                .GetRequiredService<UserManager<AppUser>>();

            //Head admin & SirmiumCommercial Info Courses
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
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
                context.Database.Migrate();
                //SirmiumCommercial default courses
                context.Courses.AddRange(
                    new Course
                    {
                        Title = "Welcome to SirmiumERPComercial!",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        AwardIcon = "fa fa-smile-o text-warning",
                        Status = "Public",
                        CreatedBy = user
                    },
                    new Course
                    {
                        Title = "How to Watch, Participate, Respond",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        AwardIcon = "pe-7s-film text-info",
                        Status = "Public",
                        CreatedBy = user
                    }
                );
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("marko123");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "marko123",
                    FirstName = "Marko",
                    LastName = "Peric",
                    CompanyName = "CompanyTMP1",
                    Email = "marko123@gmail.com",
                    PhoneNumber = "+38165111112",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "marko123");
                _ = await userManager.AddToRoleAsync(user, "Admin");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("darko");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "darko",
                    FirstName = "Darko",
                    LastName = "Markovic",
                    CompanyName = "CompanyTMP1",
                    Email = "darko.markovic@gmail.com",
                    PhoneNumber = "+38165115112",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "darko123");
                _ = await userManager.AddToRoleAsync(user, "Manager");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
                context.Database.Migrate();
                context.Courses.AddRange(
                    new Course
                    {
                        Title = "1. kurs",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(60),
                        AwardIcon = "fa fa-crown text-warning",
                        Status = "Public",
                        CreatedBy = user,
                        Description = "Prvi kurs, prve kompanije..."
                    },
                    new Course
                    {
                        Title = "2. kurs",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(350),
                        AwardIcon = "pe pe-7s-gleam text-danger",
                        Status = "Public",
                        CreatedBy = user,
                        Description = "Drugi kurs, prve kompanije..."
                    }
                );
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("AlexJ");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "AlexJ",
                    FirstName = "Aleksandra",
                    LastName = "Jokic",
                    CompanyName = "CompanyTMP1",
                    Email = "darko.markovic@gmail.com",
                    PhoneNumber = "+381648115112",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "AlexJ123");
                _ = await userManager.AddToRoleAsync(user, "Manager");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Nenad");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Nenad",
                    FirstName = "Nenad",
                    LastName = "Milanovic",
                    CompanyName = "CompanyTMP1",
                    Email = "nenadm@gmail.com",
                    PhoneNumber = "+38165115172",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "nenad123");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Admin2Co1");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Admin2Co1",
                    FirstName = "Ana",
                    LastName = "Markovic",
                    CompanyName = "CompanyTMP1",
                    Email = "ana.admin@gmail.com",
                    PhoneNumber = "+38165115112",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Admin2Co1");
                _ = await userManager.AddToRoleAsync(user, "Admin");
                await userManager.AddToRoleAsync(user, "User");
                await userManager.AddToRoleAsync(user, "Manager");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
                context.Database.Migrate();
                context.Courses.AddRange(
                    new Course
                    {
                        Title = "3. kurs",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        AwardIcon = "fa fa-medal text-success",
                        Status = "Public",
                        CreatedBy = user,
                        Description = "Treci kurs, prve kompanije..."
                    },
                    new Course
                    {
                        Title = "4. kurs",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(20),
                        AwardIcon = "fa fa-trophy text-primary-2",
                        Status = "Public",
                        CreatedBy = user,
                        Description = "Cetvrti kurs, prve kompanije..."
                    }
                );
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Dejan");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Dejan",
                    FirstName = "Dejan",
                    LastName = "Petrovic",
                    CompanyName = "CompanyTMP1",
                    Email = "dejan.petrovic23@gmail.com",
                    PhoneNumber = "+38163585172",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "dejan123");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Jovana45");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Jovana45",
                    FirstName = "Jovana",
                    LastName = "Jovanovic",
                    CompanyName = "CompanyTMP1",
                    Email = "jovanovic.jovana@gmail.com",
                    PhoneNumber = "+38163697172",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Jovana45");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("stefanDjordjevic");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "stefanDjordjevic",
                    FirstName = "Stefan",
                    LastName = "Djordjevic",
                    CompanyName = "CompanyTMP1",
                    Email = "stefan6789@gmail.com",
                    PhoneNumber = "+38164932272",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "stefanDjordjevic");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Aleksandar99");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Aleksandar99",
                    FirstName = "Aleksandar99",
                    LastName = "Jakovljevic",
                    CompanyName = "CompanyTMP1",
                    Email = "jakovljevica3@gmail.com",
                    PhoneNumber = "+38165998522",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Aleksandar99");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Uros111");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Uros111",
                    FirstName = "Uros",
                    LastName = "Aleksic",
                    CompanyName = "CompanyTMP1",
                    Email = "uros9981@gmail.com",
                    PhoneNumber = "+381643495788",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Uros111");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Dejan2");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Dejan2",
                    FirstName = "Dejan",
                    LastName = "Maric",
                    CompanyName = "CompanyTMP1",
                    Email = "dejan873@gmail.com",
                    PhoneNumber = "+38164223172",
                    RegistrationDate = DateTime.Now,
                    Status = "Inactive"
                };
                _ = await userManager.CreateAsync(user, "dejan123");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Ivana321");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Ivana321",
                    FirstName = "Ivana",
                    LastName = "Todorovic",
                    CompanyName = "CompanyTMP1",
                    Email = "todorovici@gmail.com",
                    PhoneNumber = "+38163456272",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Ivana321");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("NemanjaNikolic");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "NemanjaNikolic",
                    FirstName = "Nemanja",
                    LastName = "Nikolic",
                    CompanyName = "CompanyTMP1",
                    Email = "nikolicnemanja@gmail.com",
                    PhoneNumber = "+381602712658",
                    RegistrationDate = DateTime.Now,
                    Status = "Inactive"
                };
                _ = await userManager.CreateAsync(user, "NemanjaNikolic");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Teodora987");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Teodora987",
                    FirstName = "Teodora",
                    LastName = "Peric",
                    CompanyName = "CompanyTMP1",
                    Email = "peric987@gmail.com",
                    PhoneNumber = "+38163522342",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Teodora987");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("TamaraN");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "TamaraN",
                    FirstName = "Tamara",
                    LastName = "Neskovic",
                    CompanyName = "CompanyTMP1",
                    Email = "tamaraneskovic54@gmail.com",
                    PhoneNumber = "+38163581112",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "TamaraN");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }
           
            //2nd company
            user = await userManager.FindByNameAsync("Admin1Co2");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Admin1Co2",
                    FirstName = "Predrag",
                    LastName = "Maric",
                    CompanyName = "CompanyTMP2",
                    Email = "maric2654@gmail.com",
                    PhoneNumber = "+381622715984",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Admin1Co2");
                _ = await userManager.AddToRoleAsync(user, "Admin");
                _ = await userManager.AddToRoleAsync(user, "User");
                _ = await userManager.AddToRoleAsync(user, "Manager");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
                context.Database.Migrate();
                context.Courses.AddRange(
                    new Course
                    {
                        Title = "1. kurs",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        AwardIcon = "pe pe-7s-light text-warning",
                        Status = "Public",
                        CreatedBy = user,
                        Description = "Prvi kurs, druge kompanije..."
                    },
                    new Course
                    {
                        Title = "2. kurs",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(150),
                        AwardIcon = "fa fa-atom text-info",
                        Status = "Public",
                        CreatedBy = user,
                        Description = "Drugi kurs, druge kompanije..."
                    }
                );
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Goran34");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Goran34",
                    FirstName = "Goran",
                    LastName = "Nikolic",
                    CompanyName = "CompanyTMP2",
                    Email = "nikolic.dragan56@gmail.com",
                    PhoneNumber = "+381602745225",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Goran34");
                _ = await userManager.AddToRoleAsync(user, "Manager");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
                context.Database.Migrate();
                context.Courses.AddRange(
                    new Course
                    {
                        Title = "3. kurs",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        AwardIcon = "fa fa-chess-pawn text-primary-2",
                        Status = "Public",
                        CreatedBy = user,
                        Description = "Treci kurs, druge kompanije..."
                    },
                    new Course
                    {
                        Title = "4. kurs",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        EndDate = DateTime.Now.AddDays(150),
                        AwardIcon = "fa fa-graduation-cap text-error",
                        Status = "Public",
                        CreatedBy = user,
                        Description = "Cetvrti kurs, druge kompanije..."
                    }
                );
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Jovan34");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Jovan34",
                    FirstName = "Jovan",
                    LastName = "Mitic",
                    CompanyName = "CompanyTMP2",
                    Email = "jovan.mitic56@gmail.com",
                    PhoneNumber = "+381633455897",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Jovan123");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("NikolaJ7");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "NikolaJ7",
                    FirstName = "Nikola",
                    LastName = "Jankovic",
                    CompanyName = "CompanyTMP2",
                    Email = "nikola99999@gmail.com",
                    PhoneNumber = "+381668219451",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "NikolaJ7");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Zeljka1510");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Zeljka1510",
                    FirstName = "Zeljka",
                    LastName = "Teodorovic",
                    CompanyName = "CompanyTMP2",
                    Email = "zeljka1510@gmail.com",
                    PhoneNumber = "+381630058921",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Zeljka1510");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Dusan949");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Dusan949",
                    FirstName = "Dusan",
                    LastName = "Mitic",
                    CompanyName = "CompanyTMP2",
                    Email = "dusan945@gmail.com",
                    PhoneNumber = "+381602225984",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Dusan949");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Jovic8");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Jovic8",
                    FirstName = "Nikola",
                    LastName = "Jovic",
                    CompanyName = "CompanyTMP2",
                    Email = "nikola.jovic@gmail.com",
                    PhoneNumber = "+381632882584",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Jovic8123");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("PesicJanko");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "PesicJanko",
                    FirstName = "Janko",
                    LastName = "Pesic",
                    CompanyName = "CompanyTMP2",
                    Email = "pesic23janko@gmail.com",
                    PhoneNumber = "+381622766514",
                    RegistrationDate = DateTime.Now,
                    Status = "Inactive"
                };
                _ = await userManager.CreateAsync(user, "PesicJanko");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            //3rd company

            user = await userManager.FindByNameAsync("Admin1Co3");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Admin1Co3",
                    FirstName = "Nebojsa",
                    LastName = "Martic",
                    CompanyName = "CompanyTMP3",
                    Email = "marticnebojsa@gmail.com",
                    PhoneNumber = "+381639715551",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Admin1Co3");
                _ = await userManager.AddToRoleAsync(user, "Admin");
                _ = await userManager.AddToRoleAsync(user, "User");
                _ = await userManager.AddToRoleAsync(user, "Manager");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
                context.Database.Migrate();
                context.Courses.AddRange(
                    new Course
                    {
                        Title = "1. kurs",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        AwardIcon = "fas fa-heart text-danger",
                        Status = "Public",
                        CreatedBy = user,
                        Description = "Prvi kurs, Trece kompanije..."
                    },
                    new Course
                    {
                        Title = "2. kurs",
                        DateAdded = DateTime.Now,
                        DateModified = DateTime.Now,
                        AwardIcon = "fa fa-medal text-warning",
                        Status = "Public",
                        CreatedBy = user,
                        Description = "Drugi kurs, trece kompanije..."
                    }
                );
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Milan23");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Milan23",
                    FirstName = "Milan",
                    LastName = "Petrovic",
                    CompanyName = "CompanyTMP3",
                    Email = "milan23@gmail.com",
                    PhoneNumber = "+3816237855423",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Milan23");
                _ = await userManager.AddToRoleAsync(user, "Manager");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("IvanTosic");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "IvanTosic",
                    FirstName = "Ivan",
                    LastName = "Tosic",
                    CompanyName = "CompanyTMP3",
                    Email = "tosiccc8@gmail.com",
                    PhoneNumber = "+381619995551",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "IvanTosic");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("Majaaa55");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "Majaaa55",
                    FirstName = "Maja",
                    LastName = "Kostic",
                    CompanyName = "CompanyTMP3",
                    Email = "majaaa558@gmail.com",
                    PhoneNumber = "+381619995441",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "Majaaa55");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }

            user = await userManager.FindByNameAsync("JovanRistic");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "JovanRistic",
                    FirstName = "Jovan",
                    LastName = "Ristic",
                    CompanyName = "CompanyTMP3",
                    Email = "ristic.jovan438@gmail.com",
                    PhoneNumber = "+381616793211",
                    RegistrationDate = DateTime.Now,
                    Status = "Active"
                };
                _ = await userManager.CreateAsync(user, "JovanRistic");
                _ = await userManager.AddToRoleAsync(user, "User");
                AppDetailsDbContext context = app.ApplicationServices
               .GetRequiredService<AppDetailsDbContext>();
                context.Database.Migrate();
                context.UsersDetails.Add(new UserDetails
                {
                    User = user
                });
                context.SaveChanges();
            }
        }
    }
}

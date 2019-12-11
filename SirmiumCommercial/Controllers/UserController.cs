using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using System.IO;

namespace SirmiumCommercial.Controllers
{
    public class UserController : Controller
    {
        private UserManager<AppUser> userManager;
        private IAppDataRepository repository;
        private IHostingEnvironment hostingEnvironment;

        public UserController(UserManager<AppUser> userMgr,
                IAppDataRepository repo, IHostingEnvironment hosting)
        {
            userManager = userMgr;
            repository = repo;
            hostingEnvironment = hosting;
        }

        public async Task<IActionResult> Index(string id, string sort = "Date Modified",
            string order = "desc", string contentType = "All")
        {
            ViewData["Id"] = id;
            ViewData["Sort"] = sort;
            ViewData["Order"] = order;
            ViewData["ContentType"] = contentType;

            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                List<HomeViewModel> models = new List<HomeViewModel>();

                //Courses
                foreach (Course course in repository.Courses
                    .Where(c => (c.CreatedBy.CompanyName == user.CompanyName
                    || c.CreatedBy.CompanyName == null) && c.Status == "Public"))
                {
                    AppUser createdBy = repository.Courses
                            .Where(c => c.CreatedBy != null
                            && c.CourseId == course.CourseId).Select(c => c.CreatedBy)
                            .FirstOrDefault();

                    //users on courese
                    List<AppUser> usersOnCourse = new List<AppUser>();
                    foreach(string uId in repository.CourseUsers
                        .Where(cu => cu.CourseId == course.CourseId)
                        .Select(cu => cu.AppUserId))
                    {
                        usersOnCourse.Add(userManager.Users
                            .FirstOrDefault(u => u.Id == uId));
                    }

                    //course video likes
                    
                    //course video dislikes
                    HomeViewModel content = new HomeViewModel
                    {
                        ContentId = course.CourseId,
                        CreatedBy = createdBy,
                        Title = course.Title,
                        DateAdded = course.DateAdded,
                        DateModified = course.DateModified,
                        EndDate = course.EndDate,
                        AwardIcon = course.AwardIcon,
                        Video = repository.Videos
                            .FirstOrDefault(v => v.Id == course.VideoId),
                        Likes = VideoLikes(course.VideoId),
                        Dislikes = VideoDislikes(course.VideoId),
                        ContentType = "course",
                        CourseAverageRating = 
                            AvgCourseRating(course.Presentations.AsQueryable()),
                        UsersOnCourse = usersOnCourse.AsQueryable()
                    };
                    models.Add(content);

                    //Presenttions
                    foreach (Presentation presentation in course.Presentations)
                    {
                        AppUser presentCreatedBy = repository.Presentations
                            .Where(p => p.CreatedBy != null
                            && p.PresentationId == presentation.PresentationId)
                            .Select(c => c.CreatedBy).FirstOrDefault();
                        content = new HomeViewModel
                        {
                            CourseId = course.CourseId,
                            ContentId = presentation.PresentationId,
                            CreatedBy = presentCreatedBy,
                            Title = presentation.Title,
                            DateAdded = presentation.DateAdded,
                            DateModified = presentation.DateModified,
                            EndDate = course.EndDate,
                            AwardIcon = "text-primary-2 fa fa-puzzle-piece",
                            Video = repository.Videos
                                .FirstOrDefault(v => v.Id == presentation.VideoId),
                            Likes = VideoLikes(presentation.VideoId),
                            Dislikes = VideoDislikes(presentation.VideoId),
                            ContentType = "presentation",
                            PresentationAverageRating = 
                                AvgPresentationRating(repository.Presentations
                                .FirstOrDefault(p => p.PresentationId == presentation.PresentationId)
                                .Representations.AsQueryable()),
                            UsersOnCourse = usersOnCourse.AsQueryable()
                        };
                        models.Add(content);


                        if (repository.CourseUsers
                            .FirstOrDefault(cu => cu.AppUserId == user.Id &&
                                cu.CourseId == course.CourseId) != null
                           || await userManager.IsInRoleAsync(user, "Admin")
                           || await userManager.IsInRoleAsync(user, "Manager"))
                        {
                            //Representation
                            foreach (Representation representation in repository.Presentations
                                .FirstOrDefault(p => p.PresentationId == presentation.PresentationId)
                                .Representations)
                            {
                                AppUser representCreatedBy = repository.Representations
                                    .Where(r => r.CreatedBy != null
                                        && r.RepresentationId == representation.RepresentationId)
                                    .Select(c => c.CreatedBy).FirstOrDefault();

                                content = new HomeViewModel
                                {
                                    CourseId = course.CourseId,
                                    ContentId = representation.RepresentationId,
                                    CreatedBy = representCreatedBy,
                                    Title = representation.Title,
                                    DateAdded = representation.DateAdded,
                                    EndDate = course.EndDate,
                                    DateModified = representation.DateAdded,
                                    Video = repository.Videos
                                        .FirstOrDefault(v => v.Id == representation.VideoId),
                                    Likes = VideoLikes(representation.VideoId),
                                    Dislikes = VideoDislikes(representation.VideoId),
                                    ContentType = "representation",
                                    RepresentationRating = representation.Rating
                                };
                                models.Add(content);
                            }
                        }
                    }
                }

                if (contentType == "All")
                {
                    switch (sort)
                    {
                        case "End Date":
                            return View(new HomeContent
                            {
                                Content = (order == "asc") ?
                                models.AsQueryable().OrderBy(c => c.EndDate) :
                                models.AsQueryable().OrderByDescending(c => c.EndDate)
                            });
                        case "Date Added":
                            return View(new HomeContent
                            {
                                Content = (order == "asc") ?
                                models.AsQueryable().OrderBy(c => c.DateAdded) :
                                models.AsQueryable().OrderByDescending(c => c.DateAdded)
                            });
                        case "Title":
                            return View(new HomeContent
                            {
                                Content = (order == "asc") ?
                                models.AsQueryable().OrderBy(c => c.Title) :
                                models.AsQueryable().OrderByDescending(c => c.Title)
                            });
                        default:
                            return View(new HomeContent
                            {
                                Content = (order == "asc") ?
                                models.AsQueryable().OrderBy(c => c.DateModified) :
                                models.AsQueryable().OrderByDescending(c => c.DateModified)
                            });
                    }
                }
                else
                {
                    switch (sort)
                    {
                        case "End Date":
                            return View(new HomeContent
                            {
                                Content = (order == "asc") ?
                                models.AsQueryable().
                                Where(c => c.ContentType == contentType).
                                OrderBy(c => c.EndDate) :
                                models.AsQueryable().
                                Where(c => c.ContentType == contentType).
                                OrderByDescending(c => c.EndDate)
                            });
                        case "Date Added":
                            return View(new HomeContent
                            {
                                Content = (order == "asc") ?
                                models.AsQueryable().
                                Where(c => c.ContentType == contentType).
                                OrderBy(c => c.DateAdded) :
                                models.AsQueryable().
                                Where(c => c.ContentType == contentType).
                                OrderByDescending(c => c.DateAdded)
                            });
                        case "Title":
                            return View(new HomeContent
                            {
                                Content = (order == "asc") ?
                                models.AsQueryable().
                                Where(c => c.ContentType == contentType).
                                OrderBy(c => c.Title) :
                                models.AsQueryable().
                                Where(c => c.ContentType == contentType).
                                OrderByDescending(c => c.Title)
                            });
                        default:
                            return View(new HomeContent
                            {
                                Content = (order == "asc") ?
                                models.AsQueryable().
                                Where(c => c.ContentType == contentType).
                                OrderBy(c => c.DateModified) :
                                models.AsQueryable().
                                Where(c => c.ContentType == contentType).
                                OrderByDescending(c => c.DateModified)
                            });
                    }
                }
            }
            return View();
        }

        public IActionResult Notifications()
        {
            return View();
        }

        public IActionResult Messages()
        {
            return View();
        }

        public async Task<IActionResult> EditProfile(string id)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                ViewData["Id"] = user.Id;
                return View(new ProfileModel
                {
                    appUser = user
                }); 
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(ModelState);
        }

        public async Task<IActionResult> EditMyProfile (ProfileModel model)
        {
            AppUser user = await userManager.FindByIdAsync(model.UserId);

            if(user != null)
            {
                string username = (model.UserName == null || model.UserName.Trim() == "") ?
                    user.UserName : model.UserName;
                string firstName = (model.FirstName == null || model.FirstName.Trim() == "") ?
                    user.FirstName : model.FirstName;
                string lastName = (model.LastName == null || model.LastName.Trim() == "") ?
                    user.LastName : model.LastName;
                string email = (model.Email == null || model.Email.Trim() == "") ?
                    user.Email : model.Email;
                string phone = (model.PhoneNumber == null || model.PhoneNumber.Trim() == "") ?
                    user.PhoneNumber : model.PhoneNumber;

                await userManager.SetUserNameAsync(user, username);
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Email = email;
                user.PhoneNumber = phone;
                user.UpdatedAt = DateTime.Now;
                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    TempData["EditMsg"] = "Success";
                    return RedirectToAction("EditProfile", new { id = user.Id });
                }
                else
                {
                    ResultError(result);
                    ViewData["Id"] = user.Id;
                    model.appUser = user;
                    return View("EditProfile", model);
                }
                
            }
            else
            {
                return RedirectToAction("UserNotFound", "Error");
            }
        }

        public async Task<IActionResult> Settings(string id)
        {
            ViewData["Id"] = id;

            AppUser user = await userManager.FindByIdAsync(id);

            return View(new SettingsViewModel { User = user });
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordEditProfile(ProfileModel model)
        {
            ViewData["Id"] = model.UserId;

            AppUser user = await userManager.FindByIdAsync(model.UserId);

            if (user != null)
            {
                if (ModelState.IsValid == false)
                {
                    return RedirectToAction("EditProfile", new { id = user.Id });
                }

                IdentityResult result = await userManager.ChangePasswordAsync(user,
                    model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    user.UpdatedAt = DateTime.Now;
                    result = await userManager.UpdateAsync(user);
                    return RedirectToAction("EditProfile", new { id = user.Id });
                }
                else
                {
                    ResultError(result);
                    ViewData["Id"] = user.Id;
                    return View("EditProfile", new ProfileModel
                                                {
                                                    appUser = user
                                                });
                }
            }
            else
            {
                return RedirectToAction("UserNotFound", "Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(SettingsViewModel model)
        {
            ViewData["Id"] = model.UserId;

            AppUser user = await userManager.FindByIdAsync(model.UserId);

            if (user != null)
            {
                if(ModelState.IsValid == false)
                {
                    return RedirectToAction("Settings", new { id = user.Id });
                }

                IdentityResult result = await userManager.ChangePasswordAsync(user, 
                    model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    user.UpdatedAt = DateTime.Now;
                    result = await userManager.UpdateAsync(user);
                    TempData["SettingsMsg"] = "Success";
                    return RedirectToAction("Settings", new { id = user.Id });
                }
                else
                {
                    ResultError(result);
                    ViewData["Id"] = user.Id;
                    model.User = user;
                    return View("Settings", model);
                }
            }
            else
            {
                return RedirectToAction("UserNotFound", "Error");
            }
        }

        public async Task<IActionResult> UserProfile(string id, string userId)
        {
            ViewData["Id"] = id;

            AppUser user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                //averge rating
                int rating = 0;
                int representationsNumber = 0;
                foreach(Representation representation in repository.Representations
                    .Where(r => r.CreatedBy.Id == user.Id && r.Rating != 0))
                {
                    rating += representation.Rating;
                    representationsNumber++;
                }
                double avgRating = rating > 0 ? rating*1.0 / representationsNumber*1.0 : 0;

                //number of representations
                representationsNumber = repository.Representations
                    .Where(r => r.CreatedBy.Id == user.Id).Count();

                //number of views
                int totalViews = repository.Videos
                    .Where(v => v.CreatedBy == user.Id).Count();

                //started courses
                List<Course> startedCourses = new List<Course>();
                foreach(Course course in repository.Courses)
                {
                    if(repository.CourseUsers
                        .FirstOrDefault(u => u.AppUserId == user.Id && 
                            u.CourseId == course.CourseId) != null)
                    {
                        startedCourses.Add(course);
                    }
                }

                //finished courses
                List<Course> finishedCourses = new List<Course>();
                foreach (Course course in startedCourses.AsQueryable())
                {
                    int presNumb = course.Presentations.Count();
                    int userRep = 0;
                    foreach (Presentation presentation in course.Presentations)
                    {
                        if (presentation.Representations != null)
                        {
                            if (UserRepresentationExists(presentation.PresentationId, user.Id))
                            {
                                userRep++;
                            }
                        }
                    }

                    if (presNumb > 0 && userRep == presNumb)
                    {
                        finishedCourses.Add(course);
                    }
                }

                //---User timeline---
                List<HomeViewModel> models = new List<HomeViewModel>();
                //Courses
                foreach (Course course in repository.Courses
                    .Where(c => c.Status == "Public"))
                {
                    if (course.CreatedBy.Id == user.Id)
                    {
                        //users on courese
                        List<AppUser> usersOnCourse = new List<AppUser>();
                        foreach (string uId in repository.CourseUsers
                            .Where(cu => cu.CourseId == course.CourseId)
                            .Select(cu => cu.AppUserId))
                        {
                            usersOnCourse.Add(userManager.Users
                                .FirstOrDefault(u => u.Id == uId));
                        }

                        HomeViewModel content = new HomeViewModel
                        {
                            ContentId = course.CourseId,
                            CreatedBy = course.CreatedBy,
                            Title = course.Title,
                            DateAdded = course.DateAdded,
                            DateModified = course.DateModified,
                            EndDate = course.EndDate,
                            AwardIcon = course.AwardIcon,
                            Video = repository.Videos
                                .FirstOrDefault(v => v.Id == course.VideoId),
                            Likes = VideoLikes(course.VideoId),
                            Dislikes = VideoDislikes(course.VideoId),
                            ContentType = "course",
                            CourseAverageRating =
                                AvgCourseRating(course.Presentations.AsQueryable()),
                            UsersOnCourse = usersOnCourse.AsQueryable()
                        };
                        models.Add(content);

                        //Presenttions
                        foreach (Presentation presentation in course.Presentations)
                        {
                            content = new HomeViewModel
                            {
                                CourseId = course.CourseId,
                                ContentId = presentation.PresentationId,
                                CreatedBy = presentation.CreatedBy,
                                Title = presentation.Title,
                                DateAdded = presentation.DateAdded,
                                DateModified = presentation.DateModified,
                                EndDate = course.EndDate,
                                AwardIcon = "text-primary-2 fa fa-puzzle-piece",
                                Video = repository.Videos
                                    .FirstOrDefault(v => v.Id == presentation.VideoId),
                                Likes = VideoLikes(presentation.VideoId),
                                Dislikes = VideoDislikes(presentation.VideoId),
                                ContentType = "presentation",
                                PresentationAverageRating =
                                    AvgPresentationRating(repository.Presentations
                                    .FirstOrDefault(p => p.PresentationId == presentation.PresentationId)
                                    .Representations.AsQueryable()),
                                UsersOnCourse = usersOnCourse.AsQueryable()
                            };
                            models.Add(content);
                        }
                    }

                    //Representation
                    foreach (Presentation presentation in course.Presentations)
                    {
                        HomeViewModel content =
                            UserRepresentations(userId, presentation.PresentationId, course.CourseId);

                        if (content != null)
                        {
                            models.Add(content);
                        }
                    }
                }
               
                ViewData["Title"] = user.FirstName + " " +
                    user.LastName + " Profile";

                return View(new UserProfileViewModel {
                    User = user,
                    AverageRating = avgRating,
                    Views = totalViews,
                    TotalRepresentations = representationsNumber,
                    StartedCourses = startedCourses.AsQueryable(),
                    FinishedCourses = finishedCourses.AsQueryable(),
                    UserTimeline = new HomeContent { Content = models.AsQueryable()
                        .OrderByDescending(c => c.DateModified) }
                });
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(ModelState);
        }

        private double AvgCourseRating (IQueryable<Presentation> presentations)
        {
            double sum = 0;
            double count = 0;
            foreach (Presentation p in presentations)
            {
                double rating = AvgPresentationRating( repository.Presentations
                                .FirstOrDefault(pr => pr.PresentationId == p.PresentationId)
                                .Representations.AsQueryable());
                sum += (rating != 0) ? rating : 0;
                count += (rating != 0) ? 1 : 0;
            }
            return (sum != 0) ? sum / count : 0;
        }

        private double AvgPresentationRating (IQueryable<Representation> representations)
        {
            double sum = 0;
            double count = 0;
            foreach (Representation r in representations)
            {
                sum += (r.Rating != 0) ? r.Rating : 0;
                count += (r.Rating != 0) ? 1 : 0;
            }
            return (sum != 0) ? sum / count : 0;
        }

        private IQueryable<AppUser> VideoLikes (int videoId)
        {
            List<AppUser> likes = new List<AppUser>();

            foreach (Likes like in repository.Likes
                     .Where(l => l.For == "Video" && l.ForId == videoId))
            {
                likes.Add(userManager.Users
                    .FirstOrDefault(u => u.Id == like.UserId));
            }

            return likes.AsQueryable();
        }

        private IQueryable<AppUser> VideoDislikes (int videoId)
        {
            List<AppUser> dislikes = new List<AppUser>();

            foreach (Dislikes dislike in repository.Dislikes
                     .Where(d => d.For == "Video" && d.ForId == videoId))
            {
                dislikes.Add(userManager.Users
                    .FirstOrDefault(u => u.Id == dislike.UserId));
            }

            return dislikes.AsQueryable();
        }

        private bool UserRepresentationExists (int presentationId, string userId)
        {
            Presentation presentation = repository.Presentations
                .FirstOrDefault(p => p.PresentationId == presentationId);

            foreach (Representation representation in presentation.Representations)
            {
                if (representation.CreatedBy != null)
                {
                    if (representation.CreatedBy.Id == userId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private HomeViewModel UserRepresentations (string userId, int presentationId, int courseId)
        {
            Presentation presentation = repository.Presentations
                .FirstOrDefault(p => p.PresentationId == presentationId);
            Course course = repository.Courses.FirstOrDefault(c => c.CourseId == courseId);

            foreach (Representation representation in presentation.Representations)
            {
                if (representation.CreatedBy != null)
                {
                    if (representation.CreatedBy.Id == userId)
                    {
                        HomeViewModel content = new HomeViewModel
                        {
                            CourseId = courseId,
                            ContentId = representation.RepresentationId,
                            CreatedBy = representation.CreatedBy,
                            Title = representation.Title,
                            DateAdded = representation.DateAdded,
                            EndDate = course.EndDate,
                            DateModified = representation.DateAdded,
                            Video = repository.Videos
                                .FirstOrDefault(v => v.Id == representation.VideoId),
                            Likes = VideoLikes(representation.VideoId),
                            Dislikes = VideoDislikes(representation.VideoId),
                            ContentType = "representation",
                            RepresentationRating = representation.Rating
                        };
                        return content;
                    }
                }
            }

            return null;
        }


        private void ResultError(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }
    }
}

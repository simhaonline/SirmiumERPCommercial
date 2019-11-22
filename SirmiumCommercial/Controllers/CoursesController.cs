using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Controllers
{
    public class CoursesController : Controller
    {
        private UserManager<AppUser> userManager;
        private IAppDataRepository repository;
        private IHostingEnvironment hostingEnvironment;

        public CoursesController(UserManager<AppUser> userMgr,
            IAppDataRepository repo, IHostingEnvironment hosting)
        {
            userManager = userMgr;
            repository = repo;
            hostingEnvironment = hosting;
        }

        public async Task<IActionResult> MyCourses(string id, string sort, string order)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "My Courses";
            ViewData["CurrentAction"] = "MyCourses";
            ViewData["Sort"] = sort;
            ViewData["Order"] = order;


            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                //AllCourses List
                List<AllCourses> allCourses = new List<AllCourses>();

                //courses
                List<Course> courses = new List<Course>();
                foreach (CourseUsers cu in repository.CourseUsers
                    .Where(c => c.AppUserId == user.Id))
                {
                    courses.Add(repository.Courses
                        .FirstOrDefault(c => c.CourseId == cu.CourseId));
                }

                foreach (Course course in courses
                         .Where(c => c.Status == "Public"
                                && (c.CreatedBy.CompanyName == null
                                || c.CreatedBy.CompanyName == user.CompanyName)))
                {
                    //users on course
                    List<AppUser> usersOnCourse = new List<AppUser>();
                    foreach (string userId in repository.CourseUsers
                        .Where(cu => cu.CourseId == course.CourseId)
                        .Select(u => u.AppUserId))
                    {
                        usersOnCourse.Add(userManager.Users
                            .FirstOrDefault(u => u.Id == userId));
                    }

                    AllCourses c = new AllCourses
                    {
                        Course = course,
                        UsersOnCourse = usersOnCourse.AsQueryable(),
                        Video = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId),
                        Likes = VideoLikes(course.VideoId),
                        Dislikes = VideoDislikes(course.VideoId),
                        CourseAverageRating = AvgCourseRating(course.Presentations.AsQueryable())
                    };
                    allCourses.Add(c);
                }

                switch (sort)
                {
                    case "End Date":
                        return View("AllCourses", new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.EndDate).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.EndDate).AsQueryable()
                        });
                    case "Date Added":
                        return View("AllCourses", new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.DateAdded).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.DateAdded).AsQueryable()
                        });
                    default:
                        return View("AllCourses", new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.Title).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.Title).AsQueryable()
                        });
                }
            }
            return RedirectToAction("Error", "Error404");
        }

        public async Task<IActionResult> AllCourses (string id, string sort, string order)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "All Courses";
            ViewData["CurrentAction"] = "AllCourses";
            ViewData["Sort"] = sort;
            ViewData["Order"] = order;

            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                //AllCourses List
                List<AllCourses> allCourses = new List<AllCourses>();

                foreach (Course course in repository.Courses
                         .Where(c => c.Status == "Public"
                                && (c.CreatedBy.CompanyName == null
                                || c.CreatedBy.CompanyName == user.CompanyName)))
                {
                    //users on course
                    List<AppUser> usersOnCourse = new List<AppUser>();
                    foreach (string userId in repository.CourseUsers
                        .Where(cu => cu.CourseId == course.CourseId)
                        .Select(u => u.AppUserId))
                    {
                        usersOnCourse.Add(userManager.Users
                            .FirstOrDefault(u => u.Id == userId));
                    }

                    AllCourses c = new AllCourses
                    {
                        Course = course,
                        UsersOnCourse = usersOnCourse.AsQueryable(),
                        Video = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId),
                        Likes = VideoLikes(course.VideoId),
                        Dislikes = VideoDislikes(course.VideoId),
                        CourseAverageRating = AvgCourseRating(course.Presentations.AsQueryable())
                    };
                    allCourses.Add(c);
                }

                switch (sort)
                {
                    case "End Date":
                        return View(new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.EndDate).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.EndDate).AsQueryable()
                        });
                    case "Date Added":
                        return View(new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.DateAdded).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.DateAdded).AsQueryable()
                        });
                    default:
                        return View(new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.Title).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.Title).AsQueryable()
                        });
                }
            }
            return RedirectToAction("Error", "Error404");
        }

        public IActionResult CourseDetails(string id, int courseId)
        {
            ViewData["Id"] = id;
            Course course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == courseId);

            //Users on course
            IQueryable<AppUser> users = UsersOnCourse(course);

            //Course, presentations and representations videos
            IQueryable<Video> videos = CprVideos(course);

            return View(new CourseViewModel
            {
                Course = course,
                User = repository.Courses.Where(c => c.CreatedBy != null
                    && c.CourseId == courseId).Select(u => u.CreatedBy)
                    .FirstOrDefault(),
                UsersOnCourse = users,
                Videos = videos
            });
        }

        public IActionResult CourseDetailsRepresentations(string id, int courseId)
        {
            ViewData["Id"] = id;
            Course course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == courseId);

            //Users on course
            IQueryable<AppUser> users = UsersOnCourse(course);

            //Course, presentations and representations videos
            IQueryable<Video> videos = CprVideos(course);

            return View(new CourseViewModel
            {
                Course = course,
                User = repository.Courses.Where(c => c.CreatedBy != null
                    && c.CourseId == courseId).Select(u => u.CreatedBy)
                    .FirstOrDefault(),
                UsersOnCourse = users,
                Videos = videos
            });
        }

        public IActionResult CourseDetailsUsers(string id, int courseId)
        {
            ViewData["Id"] = id;
            Course course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == courseId);

            //Users on course
            IQueryable<AppUser> users = UsersOnCourse(course);

            //Course, presentations and representations videos
            IQueryable<Video> videos = CprVideos(course);

            return View(new CourseViewModel
            {
                Course = course,
                User = repository.Courses.Where(c => c.CreatedBy != null
                    && c.CourseId == courseId).Select(u => u.CreatedBy)
                    .FirstOrDefault(),
                UsersOnCourse = users,
                Videos = videos
            });
        }

        public IActionResult CourseDetailsComments(string id, int courseId)
        {
            ViewData["Id"] = id;
            Course course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == courseId);

            //created by
            AppUser createdBy = repository.Courses.Where(c => c.CreatedBy != null
                    && c.CourseId == courseId).Select(u => u.CreatedBy)
                    .FirstOrDefault();

            //Users on course
            IQueryable<AppUser> users = UsersOnCourse(course);

            //Course, presentations and representations videos
            IQueryable<Video> videos = CprVideos(course);

            //Comments
            IQueryable<Comment> comments = repository.Comments
                .Where(c => c.For == "Course" && c.ForId == course.CourseId);

            return View(new CourseViewModel
            {
                Course = course,
                User = createdBy,
                UsersOnCourse = users,
                Videos = videos,
                Comments = comments,
                CommentUserInfo = userManager.Users
            });
        }

        public IActionResult Participate(string id, int courseId)
        {
            ViewData["Id"] = id;

            if (repository.CourseUsers.FirstOrDefault(cu => cu.AppUserId == id
                    && cu.CourseId == courseId) != null)
            {
                //TempData["sccMsgCourse"] = "You have successfully joined this course.";
                return RedirectToAction("CourseDetails", new { id, courseId });
            }
            else
            {
                //TempData["errMsgCourse"] = "Sorry, something went wrong!";
                return RedirectToAction("CourseDetails", new { id, courseId });
            }
        }

        public IQueryable<AppUser> UsersOnCourse (Course course)
        {
            IQueryable<CourseUsers> courseUsers = repository.CourseUsers
                .Where(c => c.CourseId == course.CourseId);

            List<AppUser> users = new List<AppUser>();
            foreach (CourseUsers cu in courseUsers)
            {
                users.Add(userManager.Users
                    .FirstOrDefault(u => u.Id == cu.AppUserId));
            }

            return users.AsQueryable();
        }

        public IQueryable<Video> CprVideos (Course course)
        {
            List<Video> videos = new List<Video>();

            //course video
            videos.Add(repository.Videos.FirstOrDefault(v => v.Id == course.VideoId));

            foreach (Presentation p in course.Presentations)
            {
                //presentation video
                videos.Add(repository.Videos.
                    FirstOrDefault(v => v.Id == p.VideoId));

                foreach (Representation r in p.Representations)
                {
                    //representation video
                    videos.Add(repository.Videos.
                        FirstOrDefault(v => v.Id == r.VideoId));
                }
            }

            return videos.AsQueryable();
        }

        public ViewResult NewRepresentation(string id, int courseId, int presentationId)
        {
            ViewData["Id"] = id;

            string placeholder = repository.Courses.
                FirstOrDefault(c => c.CourseId == courseId).Title + "_" +
                repository.Presentations
                .FirstOrDefault(p => p.PresentationId == presentationId).Title + "_" +
                userManager.Users.FirstOrDefault(u => u.Id == id);

            return View(new NewRepresentation {
                CourseId = courseId,
                PresentationId = presentationId,
                TitlePlaceholder = placeholder
            });
        }

        [HttpPost]
        public async Task<IActionResult> NewRepresentation(NewRepresentation model)
        {
            AppUser user = await userManager.FindByIdAsync(model.UserId);

            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    //create new representation
                    string representationTitle = model.TitlePlaceholder.Replace("_", " ");
                    if (model.RepresentationTitle != null)
                    {
                        representationTitle = (model.RepresentationTitle.Trim() == "") ?
                            model.TitlePlaceholder.Replace("_", " ") :
                            model.RepresentationTitle;
                    }

                    Presentation presentation = repository.Presentations
                        .FirstOrDefault(p => p.PresentationId == model.PresentationId);
                    Representation representation = new Representation
                    {
                        CreatedBy = user,
                        Title = representationTitle,
                        DateAdded = DateTime.Now,
                        Status = "Public"
                    };
                    presentation.Representations.Add(representation);
                    repository.SavePresentation(presentation);

                    //save representation video
                    representation = repository.Presentations
                        .FirstOrDefault(p => p.PresentationId == model.PresentationId)
                        .Representations.FirstOrDefault(r => r.CreatedBy == user);

                    int videoId = 0;

                    string base64 = model.videoUrl.Substring(model.videoUrl.IndexOf(',') + 1);
                    byte[] data = Convert.FromBase64String(base64);

                    //Create video directory
                    string videoTitle = model.TitlePlaceholder;
                    if (model.VideoTitle != null)
                    {
                        videoTitle = (model.VideoTitle.Trim() == "") ?
                            model.TitlePlaceholder :
                            model.VideoTitle.Replace(" ", "_");
                    }

                    var dirPath = Path.Combine(hostingEnvironment.WebRootPath,
                        $@"UsersData\{model.UserId}\Representations");
                    System.IO.Directory.CreateDirectory(dirPath);
                    var fileName = $@"{videoTitle}.mp4";
                    var filePath = Path.Combine(dirPath, fileName);

                    //save video
                    Video video = new Video
                    {
                        Title = $"{videoTitle.Replace("_", " ")}",
                        CreatedBy = model.UserId,
                        Status = "Public",
                        For = "Representation",
                        ForId = representation.RepresentationId,
                        DateAdded = DateTime.Now,
                        VideoPath = $@"/UsersData/{model.UserId}/Representations/{videoTitle}.mp4"
                    };
                    repository.SaveVideo(video);

                    videoId = repository.Videos
                        .FirstOrDefault(v => v.For == "Representation" &&
                                    v.ForId == representation.RepresentationId).Id;

                    using (var videoFile = new FileStream(filePath, FileMode.Create))
                    {
                        videoFile.Write(data, 0, data.Length);
                        videoFile.Flush();
                    }

                    representation.VideoId = videoId;
                    repository.SaveRepresentation(representation);

                    TempData["sccMsgCourse"] = "'" + representation.Title + "' has been saved!";
                }
                else
                {
                    TempData["errMsgCourse"] = "Sorry, something went wrong!";
                }
            }
            return RedirectToAction("CourseDetails", new
            {
                id = model.UserId,
                courseId = model.CourseId
            });
        }

        private IQueryable<AppUser> VideoLikes(int videoId)
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

        private IQueryable<AppUser> VideoDislikes(int videoId)
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

        private double AvgPresentationRating(IQueryable<Representation> representations)
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

        private double AvgCourseRating(IQueryable<Presentation> presentations)
        {
            double sum = 0;
            double count = 0;
            foreach (Presentation p in presentations)
            {
                double rating = AvgPresentationRating(repository.Presentations
                                .FirstOrDefault(pr => pr.PresentationId == p.PresentationId)
                                .Representations.AsQueryable());
                sum += (rating != 0) ? rating : 0;
                count += (rating != 0) ? 1 : 0;
            }
            return (sum != 0) ? sum / count : 0;
        }
    }
}

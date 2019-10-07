using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public CoursesController(UserManager<AppUser> userMgr,
            IAppDataRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public async Task<IActionResult> MyCourses(string id, string sort, string order)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "My Courses";
            ViewData["Sort"] = sort;
            ViewData["Order"] = order;
            AppUser user = await userManager.FindByIdAsync(id);
            IQueryable<CourseUsers> courseUsers = repository.CourseUsers
                .Where(c => c.AppUserId == user.Id);
            List<Course> courses = new List<Course>();
            foreach(CourseUsers cu in courseUsers)
            {
                courses.Add(repository.Courses
                    .FirstOrDefault(c => c.CourseId == cu.CourseId));
            }
            if (user != null)
            {
                switch (sort)
                {
                    case "End Date":
                        return View("AllCourses", new AllCourse
                        {
                            Courses = (order == "asc") ?
                            courses.AsQueryable()
                            .OrderBy(c => c.EndDate) :
                            courses.AsQueryable()
                            .OrderByDescending(c => c.EndDate),
                            Users = repository.CourseUsers,
                            Videos = repository.Videos
                        });
                    case "Date Added":
                        return View("AllCourses", new AllCourse
                        {
                            Courses = (order == "asc") ?
                            courses.AsQueryable()
                            .OrderBy(c => c.DateAdded) :
                            courses.AsQueryable()
                            .OrderByDescending(c => c.DateAdded),
                            Users = repository.CourseUsers,
                            Videos = repository.Videos
                        });
                    default:
                        return View("AllCourses", new AllCourse
                        {
                            Courses = (order == "asc") ?
                            courses.AsQueryable()
                            .OrderBy(c => c.Title) :
                            courses.AsQueryable()
                            .OrderByDescending(c => c.Title),
                            Users = repository.CourseUsers,
                            Videos = repository.Videos
                        });
                }
            }
            return View();
        }

        public async Task<IActionResult> AllCourses(string id, string sort, string order)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "All Courses";
            ViewData["Sort"] = sort;
            ViewData["Order"] = order;
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                switch (sort)
                {
                    case "End Date":
                        return View(new AllCourse
                        {
                            Courses = (order == "asc") ?
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == null
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderBy(c => c.EndDate) :
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == null
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderByDescending(c => c.EndDate),
                            Users = repository.CourseUsers,
                            Videos = repository.Videos
                        });
                    case "Date Added":
                        return View(new AllCourse
                        {
                            Courses = (order == "asc") ?
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == null
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderBy(c => c.DateAdded) :
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == null
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderByDescending(c => c.DateAdded),
                            Users = repository.CourseUsers,
                            Videos = repository.Videos
                        });
                    default:
                        return View(new AllCourse
                        {
                            Courses = (order == "asc") ?
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == null
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderBy(c => c.Title) :
                            repository.Courses.Where(c => c.Status == "Public"
                            && (c.CreatedBy.CompanyName == null
                            || c.CreatedBy.CompanyName == user.CompanyName))
                            .OrderByDescending(c => c.Title),
                            Users = repository.CourseUsers,
                            Videos = repository.Videos
                        });
                }
            }
            return View();
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

        public async Task<IActionResult> Participate(string id, int courseId)
        {
            ViewData["Id"] = id;

            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                Course course = repository.Courses
                    .FirstOrDefault(c => c.CourseId == courseId);
                
                if(course != null)
                {
                    repository.AddUserToCourse(id, courseId);
                    TempData["sccMsgCourse"] = "You have successfully joined this course.";
                    return RedirectToAction("CourseDetails", new { id, courseId });
                }
                else
                {
                    TempData["errMsgCourse"] = "Sorry, something went wrong!";
                    return RedirectToAction("CourseDetails", new { id, courseId });
                }
            }
            else
            {
                TempData["errMsgCourse"] = "Sorry, something went wrong!";
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
    }
}

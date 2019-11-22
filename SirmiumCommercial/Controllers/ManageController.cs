using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Controllers
{
    public class ManageController : Controller
    {
        private UserManager<AppUser> userManager;
        private IAppDataRepository repository;

        public ManageController(UserManager<AppUser> userMgr,
            IAppDataRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public async Task<IActionResult> Index(string id, string status, 
            string sort, string order)
        {
            status = status ?? "Public";
            ViewData["Id"] = id;
            ViewData["Status"] = status;
            ViewData["Sort"] = sort ?? "Date Added";
            ViewData["Order"] = order ?? "desc";

            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                //AllCourses List
                List<AllCourses> allCourses = new List<AllCourses>();

                if (status == "All")
                {
                    foreach (Course course in repository.Courses
                             .Where(c => c.CreatedBy == user))
                    {
                        AllCourses c = new AllCourses
                        {
                            Course = course,
                            Video = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId)
                        };
                        allCourses.Add(c);
                    }
                }
                //status == Public || status == Private
                else
                {
                    foreach (Course course in repository.Courses
                             .Where(c => c.CreatedBy == user
                                && c.Status == status))
                    {
                        AllCourses c = new AllCourses
                        {
                            Course = course,
                            Video = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId)
                        };
                        allCourses.Add(c);
                    }
                }

                switch (sort)
                {
                    case "Date Modified":
                        return View(new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.DateModified).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.DateModified).AsQueryable()
                        });
                    case "End Date":
                        return View(new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.EndDate).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.EndDate).AsQueryable()
                        });
                    case "Title":
                        return View(new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.Title).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.Title).AsQueryable()
                        });
                    default:
                        return View(new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.DateAdded).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.DateAdded).AsQueryable()
                        });
                }
            }
            return RedirectToAction("Error", "Error404");
        }

        public async Task<IActionResult> NewCourse(string id)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "Create Course";
            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                return View(new NewEditCourse {
                    Course = new Course(),
                    NoEndDate = false
                });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewCourse(string id, NewEditCourse model)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "Create Course";
            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    model.Course.Status = "Private";
                    model.Course.CreatedBy = user;
                    model.Course.DateAdded = DateTime.Now;
                    model.Course.DateModified = DateTime.Now;
                    model.Course.Status = "Private";
                    if (model.NoEndDate)
                    {
                        model.Course.EndDate = DateTime.MinValue;
                    }
                    repository.SaveCourse(model.Course);
                    TempData["messageCM"] = "'" + model.Course.Title + " 'has been saved!";
                    return RedirectToAction("CourseManage", new
                    {
                        id,
                        model.Course.CourseId
                    });
                }
            }
            return View();
        }

        public IActionResult CourseManage(string id, int courseId)
        {
            ViewData["Id"] = id;
            return View(new CourseViewModel {
                Course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == courseId),
                User = repository.Courses.Where(c => c.CreatedBy != null
                    && c.CourseId == courseId).Select(u => u.CreatedBy)
                    .FirstOrDefault()
            });
        }
        
        public IActionResult DeleteCourse(string id, int courseId)
        {
            ViewData["Id"] = id;

            //deleting all presentations
            Course course = repository.Courses
                .FirstOrDefault(c => c.CourseId == courseId);
            while(course.Presentations.Count() > 0)
            {
                Presentation p = course.Presentations.FirstOrDefault();
                Presentation deletedP = repository.DeletePresentation(p.PresentationId);
            }

            Course deletedCourse = repository.DeleteCourse(courseId);
            if (deletedCourse != null)
            {
                TempData["messageIn"] = $"Course: '{deletedCourse.Title}' has been deleted!";
            }
            return RedirectToAction("Index", new{id, status = "Private",
                                                sort = "Title", order = "asc"});
        }

        public ViewResult EditCourse(string id, int courseId)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "Edit Course";
            return View(new NewEditCourse
            {
                    Course = repository.Courses.Where(c => c.CourseId == courseId)
                                        .FirstOrDefault(),
                    NoEndDate = false
            });
        }

        [HttpPost]
        public IActionResult EditCourse(string id, NewEditCourse model)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "Edit Course";
            if (ModelState.IsValid)
            {
                Course course = repository.Courses
                    .Where(c => c.CourseId == model.Course.CourseId)
                    .FirstOrDefault();
                course.AwardIcon = model.Course.AwardIcon;
                course.Description = model.Course.Description;
                course.EndDate = model.Course.EndDate;
                course.Title = model.Course.Title;
                course.DateModified = DateTime.Now;
                if (model.NoEndDate)
                {
                    course.EndDate = DateTime.MinValue;
                }
                repository.SaveCourse(course);
                TempData["messageCM"] = "'" + course.Title + " 'has been saved!";
                return RedirectToAction("CourseManage", new
                {
                    id,
                    model.Course.CourseId
                });
            }
            return View();
        }

        public IActionResult ChangeCourseStatus(string id, int courseId)
        {
            ViewData["Id"] = id;
            Course course = repository.Courses.Where(c => c.CourseId == courseId)
                                       .FirstOrDefault();
            switch (course.Status)
            {
                case "Private":
                    course.Status = "Public";
                    course.DateAdded = DateTime.Now;
                    course.DateModified = DateTime.Now;
                    repository.SaveCourse(course);
                    return RedirectToAction("Index", new
                    {
                        id,
                        status = "Public",
                        sort = "Date Added",
                        order = "asc"
                    });
                default:
                    course.Status = "Private";
                    course.DateModified = DateTime.Now;
                    repository.SaveCourse(course);
                    return RedirectToAction("CourseManage", new { id, courseId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> NewPresentation(string id, NewEditPresentation model)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    Course course = repository.Courses
                        .Where(c => c.CourseId == model.CourseId)
                        .FirstOrDefault();
                    Presentation presentation = model.Presentation;
                    presentation.CreatedBy = user;
                    presentation.DateAdded = DateTime.Now;
                    presentation.DateModified = DateTime.Now;
                    course.Presentations.Add(presentation);
                    repository.SaveCourse(course);
                    TempData["succMsgCM"] = "'" + presentation.Title + " 'has been saved!";
                }
                else
                {
                    TempData["errMsgCM"] = "Sorry, something went wrong!";
                }
            }
            return RedirectToAction("CourseManage", new
            {
                id,
                courseId = model.CourseId
            });
        }

        public IActionResult EditPresentation(string id, NewEditPresentation model)
        {
            ViewData["Id"] = id;

            if (ModelState.IsValid)
            {
                Presentation presentation = repository.Presentations
                .FirstOrDefault(p => p.PresentationId == model.Presentation.PresentationId);
                presentation.Title = model.Presentation.Title;
                presentation.Part = model.Presentation.Part;
                presentation.Description = model.Presentation.Description;
                presentation.DateModified = DateTime.Now;
                repository.SavePresentation(presentation);
                TempData["succMsgCM"] = "The changes have been saved.";
            }
            else
            {
                TempData["errMsgCM"] = "Sorry, something went wrong!";
            }
            return RedirectToAction("CourseManage",
                new { id, courseId = model.CourseId });
        }

        public IActionResult DeletePresentation(string id, int courseId, int presentationId)
        {
            ViewData["Id"] = id;
            Presentation deletedPre = repository.DeletePresentation(presentationId);
            if (deletedPre != null)
            {
                TempData["messageCM"] = $"Presentation: '{deletedPre.Title}' has been deleted!";
            }
            else
            {
                TempData["errMsgCM"] = "Sorry, something went wrong!";

            }
            return RedirectToAction("CourseManage",
                new { id, courseId});
        }
    }
}

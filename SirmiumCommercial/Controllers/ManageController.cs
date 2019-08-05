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
        private IDetailsRepository repository;

        public ManageController(UserManager<AppUser> userMgr,
            IDetailsRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public IActionResult Index(string id, string status, string sort, string order)
        {
            ViewData["Id"] = id;
            ViewData["Status"] = status;
            ViewData["Sort"] = sort;
            ViewData["Order"] = order;
            if (status == null)
            {
                ViewData["Status"] = "Public";
                ViewData["Sort"] = "Title";
                ViewData["Order"] = "asc";
                return View(repository.Courses.Where(c => c.Status == "Public")
                        .OrderBy(c => c.Title));
            }
            else if (status == "All")
            {
                switch (sort)
                {
                    case "End Date":
                        return View((order == "asc") ?
                                repository.Courses.OrderBy(c => c.EndDate) :
                                repository.Courses.OrderByDescending(c => c.EndDate));
                    case "Date Added":
                        return View((order == "asc") ?
                                repository.Courses.OrderBy(c => c.DateAdded) :
                                repository.Courses.OrderByDescending(c => c.DateAdded));
                    default:
                        return View((order == "asc") ?
                                repository.Courses.OrderBy(c => c.Title) :
                                repository.Courses.OrderByDescending(c => c.Title));
                }
            }
            else
            {
                switch (sort)
                {
                    case "End Date":
                        return View((order == "asc") ?
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderBy(c => c.EndDate) :
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderByDescending(c => c.EndDate));
                    case "Date Added":
                            return View((order == "asc") ?
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderBy(c => c.DateAdded) :
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderByDescending(c => c.DateAdded));
                    default:
                        return View((order == "asc") ?
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderBy(c => c.Title) :
                                repository.Courses.Where(c => c.Status == status)
                                    .OrderByDescending(c => c.Title));
                }
            }
            //TODO: if status == private
            //      select only course where course.createdBy.id == asp-route-id    
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

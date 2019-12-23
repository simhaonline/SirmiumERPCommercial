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
    public class FilesController : Controller
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;
        public int MaxFilesPerCourse = 3;
        public int MaxFilesPerSearchPage = 6;

        public FilesController(IAppDataRepository repo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public async Task<ViewResult> MyFiles(string id)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "My Files";

            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == id);

            List<FilesViewModel> files = new List<FilesViewModel>();
            foreach (Course course in repository.Courses
                .Where(c => c.CreatedBy.Id == user.Id))
            {
                FilesViewModel viewModel = new FilesViewModel { Course = course };
                if ((repository.CourseUsers.FirstOrDefault(cu => cu.CourseId == course.CourseId
                    && cu.AppUserId == user.Id) != null) ||
                    await userManager.IsInRoleAsync(user, "Admin") ||
                    await userManager.IsInRoleAsync(user, "Manager"))
                {
                    files.Add(GetCourseFiles(viewModel, course, 1));
                }
            }

            return View("Files", files.AsQueryable());
        }
        
        public async Task<ViewResult> SharedFiles(string id)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "Files";

            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == id);

            List<FilesViewModel> files = new List<FilesViewModel>();
            foreach (Course course in repository.Courses
                .Where(c => c.CreatedBy.CompanyName == user.CompanyName ||
                        c.CreatedBy.CompanyName == ""))
            {
                FilesViewModel viewModel = new FilesViewModel { Course = course };
                if ((repository.CourseUsers.FirstOrDefault(cu => cu.CourseId == course.CourseId
                    && cu.AppUserId == user.Id) != null) ||
                    await userManager.IsInRoleAsync(user, "Admin") ||
                    await userManager.IsInRoleAsync(user, "Manager"))
                {
                    files.Add(GetCourseFiles(viewModel, course, 1));
                }
            }

            return View("Files", files.AsQueryable());
        }

        public ActionResult CourseFiles(int courseId, int currentPage)
        {
            Course course = repository.Courses.FirstOrDefault(c => c.CourseId == courseId);
            FilesViewModel viewModel = new FilesViewModel { Course = course };

            return PartialView("CourseFiles", GetCourseFiles(viewModel, course,
                currentPage));
        }

        [HttpPost]
        public IActionResult SearchFiles (FileSearchViewModel model)
        {
            return RedirectToAction("SearchFiles", new { id = model.UserId, keyWord = model.KeyWord });
        }

        public async Task<ViewResult> SearchFiles (string id, string keyWord, int currentPage = 1)
        {
            ViewData["Id"] = id;
            ViewData["KeyWord"] = keyWord;

            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == id);

            List<PresentationFiles> presentationFiles = new List<PresentationFiles>();

            foreach (Course course in repository.Courses
                .Where(c => c.CreatedBy.CompanyName == user.CompanyName ||
                        c.CreatedBy.CompanyName == ""))
            {
                FilesViewModel viewModel = new FilesViewModel { Course = course };
                if ((repository.CourseUsers.FirstOrDefault(cu => cu.CourseId == course.CourseId
                    && cu.AppUserId == user.Id) != null) ||
                    await userManager.IsInRoleAsync(user, "Admin") ||
                    await userManager.IsInRoleAsync(user, "Manager"))
                {
                    foreach (Presentation presentation in course.Presentations)
                    {
                        foreach (PresentationFiles presFiles in repository.PresentationFiles
                            .Where(f => f.PresentationId == presentation.PresentationId))
                        {
                            string fileTitle = presFiles.Title.ToLower();
                            if (fileTitle.IndexOf(keyWord.Trim().ToLower()) >= 0)
                            {
                                presentationFiles.Add(presFiles);
                            }
                        }
                    }
                }
            }

            return View("FilesSearch", new FileSearchViewModel {
                TotalFiles = presentationFiles.Count(),
                Files = presentationFiles.AsQueryable().OrderBy(f => f.Title)
                    .Skip((currentPage - 1) * MaxFilesPerSearchPage)
                    .Take(MaxFilesPerSearchPage),
                PageInfo = new FilesPerCourseInfo
                {
                    CurrentPage = currentPage,
                    FilesPerPage = MaxFilesPerSearchPage,
                    TotalFiles = presentationFiles.Count()
                }
            });
        }

        private FilesViewModel GetCourseFiles (FilesViewModel model, Course course,
            int currentPage)
        {
            List<PresentationFiles> presentationFiles = new List<PresentationFiles>();
            foreach (Presentation presentation in course.Presentations)
            {
                if (repository.PresentationFiles
                    .Any(f => f.PresentationId == presentation.PresentationId))
                {
                    presentationFiles.AddRange(repository.PresentationFiles
                        .Where(f => f.PresentationId == presentation.PresentationId));
                }
            }
            model.Files = presentationFiles.AsQueryable()
                .Skip((currentPage - 1) * MaxFilesPerCourse)
                .Take(MaxFilesPerCourse);

            model.CoursePageInfo = new FilesPerCourseInfo
            {
                CurrentPage = currentPage,
                FilesPerPage = MaxFilesPerCourse,
                TotalFiles = presentationFiles.Count()
            };

            return model;
        }
    }
}

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

        public FilesController(IAppDataRepository repo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        /*public async Task<ViewResult> MyFiles(string id)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "My Files";

            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                List<FilesViewModel> files = new List<FilesViewModel>();
                foreach (Video video in repository.Videos.Where(v => v.CreatedBy == id))
                {
                    files.Add(new FilesViewModel
                    {
                        Video = video,
                        CreatedBy = user
                    });
                }

                return View("Files", files.AsQueryable());
            }
            else
            {
                //error
                return View("Error");
            }
        }
        */
        public async Task<ViewResult> SharedFiles(string id)
        {
            ViewData["Id"] = id;
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
                    List<PresentationFiles> presentationFiles = new List<PresentationFiles>();
                    foreach(Presentation presentation in course.Presentations)
                    {
                        if (repository.PresentationFiles
                            .Any(f => f.PresentationId == presentation.PresentationId))
                        {
                            presentationFiles.AddRange(repository.PresentationFiles
                                .Where(f => f.PresentationId == presentation.PresentationId));
                        }
                    }
                    viewModel.Files = presentationFiles.AsQueryable();
                    files.Add(viewModel);
                }
            }

            return View("Files", files.AsQueryable());
        }
    }
}

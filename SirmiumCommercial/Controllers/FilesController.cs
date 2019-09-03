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

        public async Task<ViewResult> MyFiles(string id)
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

        public ViewResult SharedFiles(string id)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "Shared Files";

            List<FilesViewModel> files = new List<FilesViewModel>();
            foreach (Video video in repository.Videos
                .Where(v => v.Status == "Public"))
            {
                files.Add(new FilesViewModel
                {
                    Video = video,
                    CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == video.CreatedBy)
                });
            }

            return View("Files", files.AsQueryable());
        }
    }
}

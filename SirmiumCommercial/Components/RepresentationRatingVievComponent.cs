using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace SirmiumCommercial.Components
{
    public class RepresentationRatingViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public RepresentationRatingViewComponent(IAppDataRepository repo, 
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(string videoFor, int videoForId)
        {
            int rating = -1;

            if (videoFor == "Representation")
            {
                Representation representation = repository.Representations
                    .FirstOrDefault(r => r.RepresentationId == videoForId);

                return View(representation.Rating);
            }

            return View(rating);
        }
    }
}

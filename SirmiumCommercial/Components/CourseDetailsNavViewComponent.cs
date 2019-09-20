using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Components
{
    public class CourseDetailsNavViewComponent : ViewComponent
    {
        private IAppDataRepository repository;

        public CourseDetailsNavViewComponent(IAppDataRepository repo)
        {
            repository = repo;
        }

        public IViewComponentResult Invoke(int id)
        {
            return View(id);
        }
    }
}

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
    public class FileSearchViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string id)
        {
            return View(new FileSearchViewModel
            {
                UserId = id
            });
        }
    }
}

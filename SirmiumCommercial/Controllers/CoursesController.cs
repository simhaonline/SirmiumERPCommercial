using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SirmiumCommercial.Controllers
{
    public class CoursesController : Controller
    {
        public IActionResult MyCourses()
        {
            return View();
        }

        public IActionResult AllCourses()
        {
            return View();
        }

        public IActionResult NewCourses()
        {
            return View();
        }
    }
}

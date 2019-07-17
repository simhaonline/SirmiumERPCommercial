using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;

namespace SirmiumCommercial.Controllers
{
    public class ManageController : Controller
    {
        private IDetailsRepository repository;

        public ManageController (IDetailsRepository repo)
        {
            repository = repo;
        }

        public IActionResult Index(string id)
        {
            ViewData["Id"] = id;
            return View(repository.Courses);
        }

        public IActionResult NewCourse(string id)
        {
            ViewData["Id"] = id;
            return View();
        }

        public IActionResult CourseManage(string id, int courseId)
        {
            ViewData["Id"] = id;
            return View(repository.Courses
                .FirstOrDefault(c => c.CourseId == courseId));
        }
        
    }
}

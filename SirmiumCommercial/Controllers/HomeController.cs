using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SirmiumCommercial.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult SendEmail()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Payment()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }
    }
}

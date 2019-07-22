using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SirmiumCommercial.Controllers
{
    public class CompanyController : Controller
    {
        public IActionResult AllUsers(string id)
        {
            ViewData["Id"] = id;
            return View();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace SirmiumCommercial.Controllers
{
    public class MainController : Controller
    {
        [Authorize]
        public ViewResult Index()
        {
            return View();
        }
    }
}

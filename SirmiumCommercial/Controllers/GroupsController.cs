using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SirmiumCommercial.Controllers
{
    public class GroupsController : Controller
    {

        public IActionResult Groups()
        {
            return View();
        }
    }
}

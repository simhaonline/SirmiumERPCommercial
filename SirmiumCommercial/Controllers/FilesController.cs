﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SirmiumCommercial.Controllers
{
    public class FilesController : Controller
    {
        public IActionResult MyFiles()
        {
            return View();
        }

        public IActionResult SharedFiles()
        {
            return View();
        }
    }
}
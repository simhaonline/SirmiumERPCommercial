using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Controllers
{
    public class CommentController : Controller
    {
        private IAppDataRepository repository;

        public CommentController(IAppDataRepository repo)
        {
            repository = repo;
        }

        [HttpPost]
        public IActionResult NewComment(Comment model, string returnUrl)
        {
            model.DateAdded = DateTime.Now;
            repository.SaveComment(model);
            return Redirect(returnUrl);
        }
    }
}

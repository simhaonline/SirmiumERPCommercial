using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Controllers
{
    public class CommentController : Controller
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public CommentController(IAppDataRepository repo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public ActionResult Comments(string userId, string For, int forId)
        {
            ViewData["Id"] = userId;
            ViewData["For"] = For;
            ViewData["ForId"] = forId;
            //comments
            IQueryable<Comment> comments = repository.Comments
                    .Where(c => c.For == For && c.ForId == forId);

            //users 
            IQueryable<AppUser> users = userManager.Users;

            return PartialView("AllComments", new CommentViewModel {
                Comments = comments,
                Users = users
            });
            
        }

        public ViewResult Error (string id, string msg)
        {
            ViewData["Id"] = id;
            return View(msg);
        }

        [HttpPost]
        public ActionResult NewComment(Comment model, string returnUrl)
        {
            model.DateAdded = DateTime.Now;
            repository.SaveComment(model);

            return Redirect(returnUrl);
        }

        public IActionResult DeleteComment(int commentId, string returnUrl)
        {
            Comment comment = repository.Comments
                .FirstOrDefault(c => c.Id == commentId);

            if (comment != null)
            {
                Comment result = repository.DeleteComment(comment.Id);

                return Redirect(returnUrl);
            }
            else
            {
                //TODO: errror
                return RedirectToAction("Error", "Error", new { err = "error name" });
            }
        }
    }
}

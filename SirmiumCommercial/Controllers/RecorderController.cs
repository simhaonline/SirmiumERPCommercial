using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Reflection.Metadata;

namespace SirmiumCommercial.Controllers
{
    public class RecorderController : Controller
    {
        private UserManager<AppUser> userManager;
        private IAppDataRepository repository;
        private IHostingEnvironment hostingEnvironment;

        public RecorderController(UserManager<AppUser> userMgr,
                IAppDataRepository repo, IHostingEnvironment hosting)
        {
            userManager = userMgr;
            repository = repo;
            hostingEnvironment = hosting;
        }

        public ViewResult Index(string id, int videoId)
        {
            ViewData["Id"] = id;

            Video video = repository.Videos
                .FirstOrDefault(v => v.Id == videoId);

            if(video != null)
            {
                //likes
                IQueryable<Likes> likes = repository.Likes
                    .Where(l => l.For == "Video" && l.ForId == video.Id);

                //dislikes
                IQueryable<Dislikes> dislikes = repository.Dislikes
                    .Where(d => d.For == "Video" && d.ForId == video.Id);

                //comments
                IQueryable<Comment> comments = repository.Comments
                    .Where(c => c.ForId == video.Id);

                //users 
                IQueryable<AppUser> users = userManager.Users;

                ViewData["Title"] = video.Title;

                return View(new PlayerViewModel
                {
                    Video = video,
                    Likes = likes,
                    Dislikes = dislikes,
                    Comments = comments,
                    Users = users
                });
            }
            else
            {
                return View("Error", "Error");
            }
        }

        public ActionResult LikesDislikes(string userId, string For, int forId, string spanId)
        {
            ViewData["Id"] = userId;

            return PartialView("LikesDislikesCount", new LikesDislikes
            {
                For = For,
                ForId = forId,
                spanId = spanId,
                Likes = repository.Likes.Where(l => l.For == For && l.ForId == forId),
                Dislikes = repository.Dislikes.Where(d => d.For == For && d.ForId == forId)
            });
        }

        public ActionResult AddLike(string userId, string For, int forId, string spanId)
        {
            ViewData["Id"] = userId;

            Likes like = new Likes
            {
                For = For,
                ForId = forId,
                UserId = userId
            };
            repository.AddLike(like);

            return PartialView("LikesDislikesCount", new LikesDislikes
            {
                For = For,
                ForId = forId,
                spanId = spanId,
                Likes = repository.Likes.Where(l => l.For == For && l.ForId == forId),
                Dislikes = repository.Dislikes.Where(d => d.For == For && d.ForId == forId)
            });
        }

        public ActionResult RemoveLike(string userId, string For, int forId, string spanId)
        {
            ViewData["Id"] = userId;

            Likes like = repository.Likes
                .FirstOrDefault(l => l.For == For && l.ForId == forId && l.UserId == userId );
            repository.DeleteLike(like.Id);

            return PartialView("LikesDislikesCount", new LikesDislikes
            {
                For = For,
                ForId = forId,
                spanId = spanId,
                Likes = repository.Likes.Where(l => l.For == For && l.ForId == forId),
                Dislikes = repository.Dislikes.Where(d => d.For == For && d.ForId == forId)
            });
        }

        public ActionResult AddDislike(string userId, string For, int forId, string spanId)
        {
            ViewData["Id"] = userId;

            Dislikes dislike = new Dislikes
            {
                For = For,
                ForId = forId,
                UserId = userId
            };
            repository.AddDislike(dislike);

            return PartialView("LikesDislikesCount", new LikesDislikes
            {
                For = For,
                ForId = forId,
                spanId = spanId,
                Likes = repository.Likes.Where(l => l.For == For && l.ForId == forId),
                Dislikes = repository.Dislikes.Where(d => d.For == For && d.ForId == forId)
            });
        }

        public ActionResult RemoveDislike(string userId, string For, int forId, string spanId)
        {
            ViewData["Id"] = userId;

            Dislikes dislike = repository.Dislikes
                .FirstOrDefault(d => d.For == For && d.ForId == forId && d.UserId == userId);
            repository.DeleteDislike(dislike.Id);

            return PartialView("LikesDislikesCount", new LikesDislikes
            {
                For = For,
                ForId = forId,
                spanId = spanId,
                Likes = repository.Likes.Where(l => l.For == For && l.ForId == forId),
                Dislikes = repository.Dislikes.Where(d => d.For == For && d.ForId == forId)
            });
        }

        public ActionResult VideoTotalViews(int videoId)
        {
            Video video = repository.Videos
                .FirstOrDefault(v => v.Id == videoId);

            return PartialView("VideoTotalViews", video.Views);
        }

        public ActionResult NewView(int videoId)
        {
            Video video = repository.Videos
                .FirstOrDefault(v => v.Id == videoId);
            int totalViews = video.Views + 1;
            video.Views = totalViews;
            repository.SaveVideo(video);

            return PartialView("VideoTotalViews", totalViews);
        }

        [HttpPost]
        public async Task<IActionResult> SaveProfilePhoto(ChangeProfilePhoto model)
        {
            AppUser user = await userManager.FindByIdAsync(model.userId);

            if (user != null)
            {
                ViewData["Id"] = user.Id;

                string base64 = model.photoUrl.Substring(model.photoUrl.IndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(base64);

                //Create user directory
                string dirPath = Path.Combine(hostingEnvironment.WebRootPath, $@"UsersData\{user.Id}");
                System.IO.Directory.CreateDirectory(dirPath);

                string fileName = "profilePhoto.jpeg";
                string filePath = Path.Combine(dirPath, fileName);

                using (var imageFile = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.Write(data, 0, data.Length);
                    imageFile.Flush();
                }

                string photoPath = user.Id + @"\profilePhoto.jpeg";
                user.ProfilePhotoUrl = photoPath;
                IdentityResult result = await userManager.UpdateAsync(user);

                return RedirectToAction("MyProfile", "User", new { id = user.Id });
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }

            return View(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> UploadProfilePhoto(string id, UploadPhoto model)
        {
            ViewData["Id"] = id;

            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null && model.ProfilePhoto != null)
            {
                //Create user directory
                string dirPath = Path.Combine(hostingEnvironment.WebRootPath, $@"UsersData\{user.Id}");
                System.IO.Directory.CreateDirectory(dirPath);

                string fileName = "profilePhoto.jpeg";
                string filePath = Path.Combine(dirPath, fileName);


                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.ProfilePhoto.CopyTo(stream);
                }

                //add photo path to AppUser/ProfilePhotoUrl
                string photoPath = user.Id + @"\profilePhoto.jpeg";
                ViewData["msg"] = photoPath;
                user.ProfilePhotoUrl = photoPath;
                IdentityResult result = await userManager.UpdateAsync(user);

                return RedirectToAction("MyProfile", "User", new { id = user.Id });
            }
            else
            {
                ModelState.AddModelError("", "No photo");
            }

            return View(ModelState);
        }

        /*public ViewResult Video(string id)
        {
            ViewData["Id"] = id;
            return View(new VideoModel
            {
                UserId = id
            });
        }*/

        [HttpPost]
        public async Task<IActionResult> SaveVideo(VideoModel model)
        {
            AppUser user = await userManager.FindByIdAsync(model.UserId);


            if (user != null)
            {
                ViewData["Id"] = user.Id;
                int videoId = 0;

                string base64 = model.videoUrl.Substring(model.videoUrl.IndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(base64);

                //Create video directory
                var dirPath = "";
                var fileName = "";
                var filePath = "";
                switch (model.For)
                {
                    case "Course":
                        Course course = repository.Courses
                            .FirstOrDefault(c => c.CourseId == model.ForId);

                        dirPath = Path.Combine(hostingEnvironment.WebRootPath, 
                            $@"UsersData\{model.UserId}\Courses\{course.CourseId}");
                        System.IO.Directory.CreateDirectory(dirPath);
                        fileName = $@"{course.Title}.mp4";
                        filePath = Path.Combine(dirPath, fileName);

                        //save course video
                        Video video = new Video
                        {
                            Title = $"{course.Title}Video",
                            CreatedBy = model.UserId,
                            Status = "Public",
                            For = "Course",
                            ForId = course.CourseId,
                            DateAdded = DateTime.Now,
                            VideoPath = $@"/UsersData/{model.UserId}/Courses/{course.CourseId}/{course.Title}.mp4"
                        };
                        repository.SaveVideo(video);
                        course.VideoId = repository.Videos
                            .FirstOrDefault(v => v.ForId == course.CourseId).Id;
                        repository.SaveCourse(course);

                        videoId = course.VideoId;

                        break;

                    case "Presentation":
                        Presentation presentation = repository.Presentations
                            .FirstOrDefault(p => p.PresentationId == model.ForId);

                        dirPath = Path.Combine(hostingEnvironment.WebRootPath, 
                            $@"UsersData\{user.Id}\Presentations\");
                        System.IO.Directory.CreateDirectory(dirPath);
                        fileName = $@"{presentation.PresentationId}.mp4";
                        filePath = Path.Combine(dirPath, fileName);

                        //save presentation video 
                        Video video2 = new Video
                        {
                            Title = $"{presentation.Title}Video",
                            CreatedBy = model.UserId,
                            Status = "Public",
                            For = "Presentation",
                            ForId = presentation.PresentationId,
                            DateAdded = DateTime.Now,
                            VideoPath = $@"/UsersData/{user.Id}/Presentations/{presentation.PresentationId}.mp4"
                        };
                        repository.SaveVideo(video2);
                        presentation.VideoId = repository.Videos
                            .FirstOrDefault(v => v.ForId == presentation.PresentationId).Id;
                        repository.SavePresentation(presentation);
                        videoId = presentation.VideoId;

                        break;

                    case "Representation":
                        Representation representation = repository.Representations
                            .FirstOrDefault(r => r.RepresentationId == model.ForId);
                        dirPath = Path.Combine(hostingEnvironment.WebRootPath, 
                            $@"UsersData\{user.Id}\Representations");
                        System.IO.Directory.CreateDirectory(dirPath);
                        fileName = $@"{representation.RepresentationId}.mp4";
                        filePath = Path.Combine(dirPath, fileName);

                        //save representation vide path
                        Video video3 = new Video
                        {
                            Title = $"{representation.Title}Video",
                            CreatedBy = model.UserId,
                            Status = "Public",
                            For = "Representation",
                            ForId = representation.RepresentationId,
                            DateAdded = DateTime.Now,
                            VideoPath = $@"/UsersData/{user.Id}/Representations/{representation.RepresentationId}.mp4"
                        };
                        repository.SaveVideo(video3);
                        representation.VideoId = repository.Videos
                            .FirstOrDefault(v => v.Id == representation.RepresentationId).Id;
                        repository.SaveRepresentation(representation);
                        videoId = representation.VideoId;

                        break;
                }

                using (var videoFile = new FileStream(filePath, FileMode.Create))
                 {
                     videoFile.Write(data, 0, data.Length);
                     videoFile.Flush();
                 }

                return RedirectToAction("Index", new { id = user.Id, videoId});
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }

            return Redirect(model.returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> NewPracticeVideo(VideoModel model)
        {
            AppUser user = await userManager.FindByIdAsync(model.UserId);

            if (user != null)
            {
                ViewData["Id"] = user.Id;
                int videoId = 0;

                string base64 = model.videoUrl.Substring(model.videoUrl.IndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(base64);

                //Create video directory
                var videoTitle = (model.Title == "" || model.Title == null) ?
                    $"Video{DateTime.Now.Day}{DateTime.Now.Month}{DateTime.Now.Year}" +
                    $"{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}" :
                    model.Title;

                var dirPath = Path.Combine(hostingEnvironment.WebRootPath,
                    $@"UsersData\{model.UserId}\Video");
                System.IO.Directory.CreateDirectory(dirPath);
                var fileName = $@"{videoTitle}.mp4";
                var filePath = Path.Combine(dirPath, fileName);

                //save video
                DateTime currentDate = DateTime.Now;
                Video video = new Video
                {
                    Title = $"{videoTitle}",
                    CreatedBy = model.UserId,
                    Status = "Private",
                    For = "Practice",
                    DateAdded = currentDate,
                    VideoPath = $@"/UsersData/{model.UserId}/Video/{videoTitle}.mp4"
                };
                repository.SaveVideo(video);

                videoId = repository.Videos
                    .FirstOrDefault(v => v.Title == videoTitle &&
                                    v.DateAdded == currentDate).Id;

                using (var videoFile = new FileStream(filePath, FileMode.Create))
                {
                    videoFile.Write(data, 0, data.Length);
                    videoFile.Flush();
                }

                return RedirectToAction("Index", new { id = user.Id, videoId });
            }
            else
            {
                return Redirect("Error");
            }
        }

        public IActionResult DeletePrivateVideo (string id, int videoId)
        {
            ViewData["Id"] = id;

            Video video = repository.Videos.FirstOrDefault(v => v.Id == videoId);
            if(video != null)
            {
                repository.DeleteVideo(videoId);
                TempData["succMsgMG"] = $"Video: '{video.Title}' has been deleted!";
            }
            else
            {
                TempData["errMsgMG"] = "Sorry, something went wrong!";
            }
            return RedirectToAction("MyGallery", "Video", new { id = id });
        }

        [HttpPost]
        public IActionResult RepresentationRating(RepreVideoComponentViewModel model)
        {
            Representation representation = repository.Representations
                .FirstOrDefault(r => r.RepresentationId == model.representationId);

            representation.Rating = model.Rating;
            repository.SaveRepresentation(representation);
            repository.NewNotification(model.userId, "RepresentationRating", "Video", representation.VideoId);

            return RedirectToAction("Index", new { id = model.userId, videoId = representation.VideoId });
        }
    }
}

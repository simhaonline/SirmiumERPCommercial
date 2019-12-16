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
using System.Net.Mail;

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

        public async Task<ViewResult> Index(string id, int videoId)
        {
            ViewData["Id"] = id;

            AppUser user = userManager.Users
                .FirstOrDefault(u => u.Id == id);
            Video video = repository.Videos
                .FirstOrDefault(v => v.Id == videoId);

            if(video != null)
            {
                if((repository.VideoShareds
                    .FirstOrDefault(v => v.VideoId == videoId && v.UserId == id) == null
                    && await userManager.IsInRoleAsync(user, "Admin") == false
                    && await userManager.IsInRoleAsync(user, "Manager") == false
                    && video.CreatedBy != user.Id) && video.For == "Representation")
                {
                    return View("Index2");
                }

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

                //files if(for == representation || for == presentation)
                List<PresentationFiles> files = new List<PresentationFiles>();
                if(video.For == "Presentation" || video.For == "Representation")
                {

                    int presentationId = 0;
                    if (video.For == "Presentation")
                    {
                        presentationId = video.ForId;
                    }
                    else
                    {
                        //video.for == representation
                        foreach(Presentation presentation in repository.Presentations)
                        {
                            if(presentation.Representations
                                .FirstOrDefault(r => r.RepresentationId == video.ForId) != null)
                            {
                                presentationId = presentation.PresentationId;
                                break;
                            }
                        }
                    }

                    files.AddRange(repository.PresentationFiles
                        .Where(f => f.PresentationId == presentationId));
                }

                ViewData["Title"] = video.Title;

                return View(new PlayerViewModel
                {
                    Video = video,
                    Likes = likes,
                    Dislikes = dislikes,
                    Comments = comments,
                    Users = users,
                    Files = files.AsQueryable()
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
                user.UpdatedAt = DateTime.Now;
                user.ProfilePhotoUrl = photoPath;
                IdentityResult result = await userManager.UpdateAsync(user);

                return RedirectToAction("UserProfile", "User", new { id = user.Id, userId = user.Id  });
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
                user.UpdatedAt = DateTime.Now;
                user.ProfilePhotoUrl = photoPath;
                IdentityResult result = await userManager.UpdateAsync(user);

                return RedirectToAction("UserProfile", "User", new { id = user.Id, userId = user.Id });
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

        public IActionResult RepresentationRating(string userId, int representationId)
        {
            Representation representation = repository.Representations
                .FirstOrDefault(r => r.RepresentationId == representationId);

            return RedirectToAction("Index", new { id = userId, videoId = representation.VideoId });
        }

        public async Task<ViewResult> VideoSettings (string id, int videoId)
        {
            ViewData["Id"] = id;

            AppUser videoCreatedBy = userManager.Users.FirstOrDefault(u => u.Id == id);
            Video video = repository.Videos.FirstOrDefault(v => v.Id == videoId);

            List<AppUser> usersShared = new List<AppUser>();
            List<AppUser> userNotShared = new List<AppUser>();

            foreach(AppUser appUser in userManager.Users
                .Where(u => u.CompanyName == videoCreatedBy.CompanyName
                        && u.Id != videoCreatedBy.Id))
            {
                if(repository.VideoShareds
                    .FirstOrDefault(v => v.VideoId == video.Id && v.UserId == appUser.Id) != null)
                {
                    usersShared.Add(appUser);
                }
                else if (await userManager.IsInRoleAsync(appUser, "Admin"))
                {
                    usersShared.Add(appUser);
                }
                else if (await userManager.IsInRoleAsync(appUser, "Manager"))
                {
                    usersShared.Add(appUser);
                }
                else
                {
                    userNotShared.Add(appUser);
                }
            }

            return View(new VideoSettingsViewModel
            {
                UserId = id,
                VideoId = video.Id,
                VideoTitle = video.Title,
                CanSeeVideo = usersShared.AsQueryable(),
                AllUsers = userNotShared.AsQueryable()
            });
        }

        [HttpPost]
        public IActionResult ChangeVideoTitle (VideoSettingsViewModel model)
        {
            Video video = repository.Videos.FirstOrDefault(v => v.Id == model.VideoId);

            if(model.VideoTitle != null && model.VideoTitle != "" 
                && model.VideoTitle.Trim() != video.Title)
            {
                video.Title = model.VideoTitle;
                video.UpdatedAt = DateTime.Now;
                repository.SaveVideo(video);
            }
            return RedirectToAction("VideoSettings", new { id = model.UserId, videoId = model.VideoId });
        }

        [HttpPost]
        public async Task<IActionResult> ShareVideo(VideoSettingsViewModel model)
        {
            ViewData["Id"] = model.UserId;

            AppUser user = await userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                Video video = repository.Videos.FirstOrDefault(v => v.Id == model.VideoId);

                if (model.UsersIds != null && model.UsersIds.Trim() != "")
                {
                    //list of users id
                    string userIdList = model.UsersIds.Trim();
                    while (userIdList.Trim().Length > 0)
                    {
                        int index = userIdList.IndexOf(";");
                        if (index != -1)
                        {
                            string newUserId = userIdList.Substring(0, index);

                            repository.SaveVideoShared(video.Id, newUserId);

                            userIdList = userIdList.Replace(newUserId + ";", "");
                        }
                    }
                }
                return RedirectToAction("VideoSettings", new { id = model.UserId, videoId = model.VideoId });
            }

            return RedirectToAction("Error", "UserNotFound");
        }

        public async Task<IActionResult> ShareVideoAllUsers(string id, int videoId)
        {
            Video video = repository.Videos
                .FirstOrDefault(v => v.Id == videoId);
            AppUser user = userManager.Users
                .FirstOrDefault(u => u.Id == id);

            List<AppUser> users = new List<AppUser>();
            foreach (AppUser appUser in userManager.Users
                .Where(u => u.CompanyName == user.CompanyName && u.Id != user.Id))
            {
                if (await userManager.IsInRoleAsync(appUser, "Admin") == false
                    && await userManager.IsInRoleAsync(appUser, "Manager") == false
                    && repository.VideoShareds
                    .FirstOrDefault(v => v.VideoId == videoId && v.UserId == appUser.Id) == null)
                {
                    users.Add(appUser);
                }
            }

            foreach (AppUser appUser in users)
            {
                repository.SaveVideoShared(video.Id, appUser.Id);
            }

            return RedirectToAction("VideoSettings", new { id = id, videoId = videoId });
        }

        public IActionResult RemoveAccess(string id, string userId,
            int videoId)
        {
            repository.DeleteVideoShared(videoId, userId);
            return RedirectToAction("VideoSettings", new { id = id, videoId = videoId });
        }

        public IActionResult RemoveAccessToAll(string id, int videoId)
        {
            List<AppUser> users = new List<AppUser>();
            foreach (VideoShared shared in repository.VideoShareds
                .Where(v => v.VideoId == videoId))
            {
                AppUser appUser = userManager.Users
                    .FirstOrDefault(u => u.Id == shared.UserId);
                users.Add(appUser);
            }

            foreach (AppUser user in users)
            {
                repository.DeleteVideoShared(videoId, user.Id);
            }

            return RedirectToAction("VideoSettings", new { id = id, videoId = videoId });
        }

    }
}

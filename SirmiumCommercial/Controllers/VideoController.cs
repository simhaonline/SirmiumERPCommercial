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
    public class VideoController : Controller
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public VideoController(IAppDataRepository repo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public async Task<ViewResult> MyGallery(string id)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "My Gallery";

            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                //top 10
                List<VideoInfo> top10 = new List<VideoInfo>();
                //course videos
                List<VideoInfo> courseVideos = new List<VideoInfo>();
                //presentation videos
                List<VideoInfo> presentationVideos = new List<VideoInfo>();
                //representation videos
                List<VideoInfo> representationVideos = new List<VideoInfo>();

                foreach (Video video in repository.Videos
                    .Where(v => v.CreatedBy == user.Id && v.Status != "Private")
                    .OrderByDescending(v => v.DateAdded))
                {
                    VideoInfo vI = new VideoInfo
                    {
                        Video = video,
                        CreatedBy = user,
                        Likes = repository.Likes
                        .Where(l => l.For == "Video" && l.ForId == video.Id).Count(),
                        Dislikes = repository.Dislikes
                        .Where(d => d.For == "Video" && d.ForId == video.Id).Count(),
                        Comments = repository.Comments
                        .Where(c => c.For == "Video" && c.ForId == video.Id).Count()

                    };

                    top10.Add(vI);

                    switch (video.For)
                    {
                        case "Course":
                            courseVideos.Add(vI);
                            break;
                        case "Presentation":
                            presentationVideos.Add(vI);
                            break;
                        case "Representation":
                            representationVideos.Add(vI);
                            break;
                    }
                }

                //most popular
                List<VideoInfo> mostPopular = new List<VideoInfo>();
                foreach(Video video in repository.Videos
                    .Where(v => v.CreatedBy == user.Id && v.Status != "Private")
                    .OrderByDescending(v => v.Views).Take(16))
                {
                    VideoInfo vI = new VideoInfo
                    {
                        Video = video,
                        CreatedBy = user,
                        Likes = repository.Likes
                        .Where(l => l.For == "Video" && l.ForId == video.Id).Count(),
                        Dislikes = repository.Dislikes
                        .Where(d => d.For == "Video" && d.ForId == video.Id).Count(),
                        Comments = repository.Comments
                        .Where(c => c.For == "Video" && c.ForId == video.Id).Count()

                    };
                    mostPopular.Add(vI);
                }

                //private videos
                List<VideoInfo> privateVideos = new List<VideoInfo>();
                foreach (Video video in repository.Videos
                    .Where(v => v.CreatedBy == user.Id && v.Status == "Private")
                    .OrderBy(v => v.DateAdded))
                {
                    VideoInfo vI = new VideoInfo
                    {
                        Video = video,
                        CreatedBy = user

                    };
                    privateVideos.Add(vI);
                }

                return View("VideoGallery", new GalleryViewModel {
                    Top10 = top10.AsQueryable()
                        .OrderByDescending(v => v.Likes - v.Dislikes).Take(10),
                    MostPopular = mostPopular.AsQueryable(),
                    Private = privateVideos.AsQueryable(),
                    CourseVideos = courseVideos.AsQueryable(),
                    PresentationVideos = presentationVideos.AsQueryable(),
                    RepresentationVideos = representationVideos.AsQueryable()
                });
            }
            else
            {
                //error
                return View("Error");
            }
        }

        public async Task<ViewResult> VideoGallery(string id)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "Video Gallery";

            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                //top 10
                List<VideoInfo> top10 = new List<VideoInfo>();
                //course videos
                List<VideoInfo> courseVideos = new List<VideoInfo>();
                //presentation videos
                List<VideoInfo> presentationVideos = new List<VideoInfo>();
                //representation videos
                List<VideoInfo> representationVideos = new List<VideoInfo>();

                foreach (Video video in repository.Videos
                    .Where(v => v.Status != "Private")
                    .OrderByDescending(v => v.DateAdded))
                {
                    AppUser createdBy = userManager.Users
                        .FirstOrDefault(u => u.Id == video.CreatedBy);

                    if (createdBy.CompanyName == user.CompanyName
                        || createdBy.CompanyName == "")
                    {
                        VideoInfo vI = new VideoInfo
                        {
                            Video = video,
                            CreatedBy = createdBy,
                            Likes = repository.Likes
                            .Where(l => l.For == "Video" && l.ForId == video.Id).Count(),
                            Dislikes = repository.Dislikes
                            .Where(d => d.For == "Video" && d.ForId == video.Id).Count(),
                            Comments = repository.Comments
                            .Where(c => c.For == "Video" && c.ForId == video.Id).Count()

                        };

                        top10.Add(vI);

                        switch (video.For)
                        {
                            case "Course":
                                courseVideos.Add(vI);
                                break;
                            case "Presentation":
                                presentationVideos.Add(vI);
                                break;
                            case "Representation":
                                representationVideos.Add(vI);
                                break;
                        }
                    }
                }

                //most popular
                List<VideoInfo> mostPopular = new List<VideoInfo>();
                foreach (Video video in repository.Videos
                    .Where(v => v.Status != "Private"))
                {
                    AppUser createdBy = userManager.Users
                        .FirstOrDefault(u => u.Id == video.CreatedBy);

                    if (createdBy.CompanyName == user.CompanyName
                        || createdBy.CompanyName == "")
                    {
                        VideoInfo vI = new VideoInfo
                        {
                            Video = video,
                            CreatedBy = createdBy,
                            Likes = repository.Likes
                        .Where(l => l.For == "Video" && l.ForId == video.Id).Count(),
                            Dislikes = repository.Dislikes
                        .Where(d => d.For == "Video" && d.ForId == video.Id).Count(),
                            Comments = repository.Comments
                        .Where(c => c.For == "Video" && c.ForId == video.Id).Count()

                        };
                        mostPopular.Add(vI);
                    }
                }

                List<VideoInfo> privateVideos = new List<VideoInfo>();

                return View(new GalleryViewModel
                {
                    Top10 = top10.AsQueryable()
                        .OrderByDescending(v => v.Likes - v.Dislikes).Take(10),
                    MostPopular = mostPopular.AsQueryable()
                        .OrderByDescending(v => v.Video.Views).Take(16),
                    Private = privateVideos.AsQueryable(),
                    CourseVideos = courseVideos.AsQueryable(),
                    PresentationVideos = presentationVideos.AsQueryable(),
                    RepresentationVideos = representationVideos.AsQueryable()
                });
            }
            else
            {
                //error
                return View("Error");
            }
        }

        public ViewResult NewVideo (string id)
        {
            ViewData["Id"] = id;

            return View();
        }
    }
}

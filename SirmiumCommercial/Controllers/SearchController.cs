using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using System.IO;

namespace SirmiumCommercial.Controllers
{
    public class SearchController : Controller
    {
        private UserManager<AppUser> userManager;
        private IAppDataRepository repository;
        private IHostingEnvironment hostingEnvironment;

        public int MaxSearchPageUsers = 4;
        public int MaxSearchPageCourse = 8;
        public int MaxSearchPagePresentation = 8;
        public int MaxSearchPageRepresentation = 4;
        public int MaxSearchPageVideo = 4;
        public int MaxSearchPageFile = 4;

        public SearchController(UserManager<AppUser> userMgr,
                IAppDataRepository repo, IHostingEnvironment hosting)
        {
            userManager = userMgr;
            repository = repo;
            hostingEnvironment = hosting;
        }

        [HttpPost]
        public async Task<ViewResult> Search(string id, string KeyWord)
        {
            ViewData["Id"] = id;

            AppUser currentUser = userManager.Users.FirstOrDefault(u => u.Id == id);

            //users
            List<AppUser> users = new List<AppUser>();
            foreach(AppUser user in userManager.Users.Where(u => u.CompanyName == currentUser.CompanyName))
            {
                if (SearchWordExists(user.FirstName, KeyWord))
                {
                    users.Add(user);
                }
                else if (SearchWordExists(user.LastName, KeyWord))
                {
                    users.Add(user);
                }
                else if (SearchWordExists(user.UserName, KeyWord))
                {
                    users.Add(user);
                }
            }
            SearchAllUsers allUsers = new SearchAllUsers
            {
                AllUsers = users.AsQueryable().OrderBy(u => u.LastName)
                    .Take(MaxSearchPageUsers),
                UserPageInfo = new SearchUserPageInfo
                {
                    CurrentPage = 1,
                    TotalUsers = users.AsQueryable().Count(),
                    UsersPerPage = MaxSearchPageUsers
                }
            };

            //courses for ALL USERS
            //& presentations
            List<SearchCourse> courses = new List<SearchCourse>();
            List<SearchPresentation> presentations = new List<SearchPresentation>();
            List<SearchRepresentation> representations = new List<SearchRepresentation>();
            List<SearchVideo> videos = new List<SearchVideo>();
            List<SearchFiles> files = new List<SearchFiles>();
            foreach(Course course in repository.Courses.Where(c => c.Status == "Public"
                        && (c.CreatedBy.CompanyName == currentUser.CompanyName ||
                            c.CreatedBy.CompanyName == null)))
            {
                if (SearchWordExists(course.Title, KeyWord))
                {
                    SearchCourse searchCourse = new SearchCourse
                    {
                        Course = course,
                        CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                    };
                    courses.Add(searchCourse);
                }

                //course video
                if (repository.Videos.Any(v => v.Id == course.VideoId))
                {
                    Video courseVideo = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId);
                    if (SearchWordExists(courseVideo.Title, KeyWord))
                    {
                        string info = $"<span class='text-muted'>For: <i class='{course.AwardIcon}'></i>" +
                            $"<strong>{course.Title}</strong></span>";
                        SearchVideo searchVideoC = new SearchVideo
                        {
                            Video = courseVideo,
                            CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == courseVideo.CreatedBy),
                            VideoForInfo = info
                        };
                        videos.Add(searchVideoC);
                    }
                }

                foreach (Presentation presentation in course.Presentations)
                {
                    if (SearchWordExists(presentation.Title, KeyWord))
                    {
                        SearchPresentation searchPresentation = new SearchPresentation
                        {
                            Presentation = presentation,
                            Course = course,
                            CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                        };
                        presentations.Add(searchPresentation);
                    }

                    //presentation video
                    if (repository.Videos.Any(v => v.Id == presentation.VideoId))
                    {
                        Video presentationVideo = repository.Videos.FirstOrDefault(v => v.Id == presentation.VideoId);
                        if (SearchWordExists(presentationVideo.Title, KeyWord))
                        {
                            string info = $"<span class='text-muted'>For: <strong>{presentation.Title}</strong> <i class='{course.AwardIcon}'></i>" +
                                $"<strong>{course.Title}</strong></span>";
                            SearchVideo searchVideoP = new SearchVideo
                            {
                                Video = presentationVideo,
                                CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == presentationVideo.CreatedBy),
                                VideoForInfo = info
                            };
                            videos.Add(searchVideoP);
                        }
                    }

                    //presentation files
                    foreach (PresentationFiles file1 in repository.PresentationFiles
                        .Where(f => f.PresentationId == presentation.PresentationId))
                    {
                        if (SearchWordExists(file1.Title, KeyWord))
                        {
                            string info = $"<span class='text-muted'>For: <strong>{presentation.Title}</strong> <i class='{course.AwardIcon}'></i>" +
                                $"<strong>{course.Title}</strong></span>";
                            SearchFiles searchFile1 = new SearchFiles
                            {
                                File = file1,
                                CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == presentation.CreatedBy.Id),
                                FileForInfo = info
                            };
                            files.Add(searchFile1);
                        }
                    }

                    foreach (Representation representation in presentation.Representations)
                    {
                        if (SearchWordExists(representation.Title, KeyWord))
                        {
                            SearchRepresentation searchRepresentation = new SearchRepresentation
                            {
                                Representation = representation,
                                PresentationTitle = presentation.Title,
                                Course = course,
                                Video = repository.Videos.FirstOrDefault(v => v.Id == representation.VideoId),
                                CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == representation.CreatedBy.Id)
                            };
                            representations.Add(searchRepresentation);
                        }

                        //representation video
                        if (repository.Videos.Any(v => v.Id == representation.VideoId))
                        {
                            Video representationVideo = repository.Videos.FirstOrDefault(v => v.Id == representation.VideoId);
                            if (SearchWordExists(representationVideo.Title, KeyWord))
                            {
                                if(repository.VideoShareds.Any(v => v.UserId == currentUser.Id &&
                                    v.VideoId == representationVideo.Id) ||
                                     await userManager.IsInRoleAsync(currentUser, "Admin") == true
                                     || await userManager.IsInRoleAsync(currentUser, "Manager") == true
                                     || representationVideo.CreatedBy == currentUser.Id)
                                {
                                    string info = $"<span class='text-muted'>For: <strong>{representation.Title}</strong> <strong>{presentation.Title}</strong> <i class='{course.AwardIcon}'></i>" +
                                        $"<strong>{course.Title}</strong></span>";
                                    SearchVideo searchVideoR = new SearchVideo
                                    {
                                        Video = representationVideo,
                                        CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == representationVideo.CreatedBy),
                                        VideoForInfo = info
                                    };
                                    videos.Add(searchVideoR);
                                }
                            }
                        }
                    }
                }
            }

            //courses where currentUser == course.CreatedBy && status == "Private"
            //& presentations
            foreach(Course course in repository.Courses.Where(c => c.Status == "Private"
                        && (c.CreatedBy.CompanyName == currentUser.CompanyName ||
                            c.CreatedBy.CompanyName == null)))
            {
                if(course.CreatedBy != null)
                {
                    if(course.CreatedBy.Id == currentUser.Id)
                    {
                        if (SearchWordExists(course.Title, KeyWord))
                        {
                            SearchCourse searchCourse = new SearchCourse
                            {
                                Course = course,
                                CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                            };
                            courses.Add(searchCourse);
                        }

                        //course video
                        if (repository.Videos.Any(v => v.Id == course.VideoId))
                        {
                            Video courseVideo = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId);
                            if (SearchWordExists(courseVideo.Title, KeyWord))
                            {
                                string info = $"<span class='text-muted'>For: <i class='{course.AwardIcon}'></i>" +
                                    $"<strong>{course.Title}</strong></span>";
                                SearchVideo searchVideoC = new SearchVideo
                                {
                                    Video = courseVideo,
                                    CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == courseVideo.CreatedBy),
                                    VideoForInfo = info
                                };
                                videos.Add(searchVideoC);
                            }
                        }

                        foreach (Presentation presentation in course.Presentations)
                        {
                            if (SearchWordExists(presentation.Title, KeyWord))
                            {
                                SearchPresentation searchPresentation = new SearchPresentation
                                {
                                    Presentation = presentation,
                                    Course = course,
                                    CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                                };
                                presentations.Add(searchPresentation);
                            }

                            //presentation video
                            if (repository.Videos.Any(v => v.Id == presentation.VideoId))
                            {
                                Video presentationVideo = repository.Videos.FirstOrDefault(v => v.Id == presentation.VideoId);
                                if (SearchWordExists(presentationVideo.Title, KeyWord))
                                {
                                    string info = $"<span class='text-muted'>For: <strong>{presentation.Title}</strong> <i class='{course.AwardIcon}'></i>" +
                                        $"<strong>{course.Title}</strong></span>";
                                    SearchVideo searchVideoP = new SearchVideo
                                    {
                                        Video = presentationVideo,
                                        CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == presentationVideo.CreatedBy),
                                        VideoForInfo = info
                                    };
                                    videos.Add(searchVideoP);
                                }
                            }

                            //presentation files
                            foreach (PresentationFiles file2 in repository.PresentationFiles
                                .Where(f => f.PresentationId == presentation.PresentationId))
                            {
                                if (SearchWordExists(file2.Title, KeyWord))
                                {
                                    string info = $"<span class='text-muted'>For: <strong>{presentation.Title}</strong> <i class='{course.AwardIcon}'></i>" +
                                        $"<strong>{course.Title}</strong></span>";
                                    SearchFiles searchFile2 = new SearchFiles
                                    {
                                        File = file2,
                                        CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == presentation.CreatedBy.Id),
                                        FileForInfo = info
                                    };
                                    files.Add(searchFile2);
                                }
                            }
                        }
                    }
                }
            }
            SearchAllCourses allCourses = new SearchAllCourses
            {
                AllCourses = courses.AsQueryable().OrderBy(c => c.Course.Title)
                    .Take(MaxSearchPageCourse),
                CoursePageInfo = new SearchCoursePageInfo
                {
                    CurrentPage = 1,
                    TotalCourses = courses.AsQueryable().Count(),
                    CoursesPerPage = MaxSearchPageCourse
                }
            };
            SearchAllPresentation allPresentation = new SearchAllPresentation
            {
                AllPresentations = presentations.AsQueryable()
                    .OrderBy(p => p.Presentation.Title)
                    .Take(MaxSearchPagePresentation),
                PresentationPageInfo = new SearchPresentationPageInfo
                {
                    CurrentPage = 1,
                    TotalPresentations = presentations.AsQueryable().Count(),
                    PresentationsPerPage = MaxSearchPagePresentation
                }
            };
            SearchAllRepresentation allRepresentation = new SearchAllRepresentation
            {
                AllRepresentation = representations.AsQueryable()
                    .OrderBy(r => r.Representation.Title)
                    .Take(MaxSearchPageRepresentation),
                RepresentationPageInfo = new SearchRepresentationPageInfo
                {
                    CurrentPage = 1,
                    TotalRepresentation = representations.AsQueryable().Count(),
                    RepresentationPerPage = MaxSearchPageRepresentation
                }
            };
            SearchAllVideos allVideos = new SearchAllVideos
            {
                AllVideos = videos.AsQueryable()
                    .OrderBy(v => v.Video.Title)
                    .Take(MaxSearchPageVideo),
                VideoPageInfo = new SearchVideoPageInfo
                {
                    CurrentPage = 1,
                    TotalVideos = videos.Count(),
                    VideosPerPage = MaxSearchPageVideo
                }
            };
            SearchAllFiles allFiles = new SearchAllFiles
            {
                AllFiles = files.AsQueryable()
                    .OrderBy(f => f.File.Title)
                    .Take(MaxSearchPageFile),
                FilePageInfo = new SearchFilesPageInfo
                {
                    CurrentPage = 1,
                    TotalFiles = files.Count(),
                    FilesPerPage = MaxSearchPageFile
                }
            };

            return View("Search", new SearchViewModel
            {
                KeyWord = KeyWord,
                Users = allUsers,
                Courses = allCourses,
                Presentations = allPresentation,
                Representations = allRepresentation,
                Videos = allVideos,
                Files = allFiles
            });
        }

        public ActionResult SearchUsersPartial(string userId, string keyWord, int currentPage)
        {
            AppUser currentUser = userManager.Users.FirstOrDefault(u => u.Id == userId);

            List<AppUser> users = new List<AppUser>();
            foreach (AppUser user in userManager.Users.Where(u => u.CompanyName == currentUser.CompanyName))
            {
                if (SearchWordExists(user.FirstName, keyWord))
                {
                    users.Add(user);
                }
                else if (SearchWordExists(user.LastName, keyWord))
                {
                    users.Add(user);
                }
                else if (SearchWordExists(user.UserName, keyWord))
                {
                    users.Add(user);
                }
            }

            SearchAllUsers allUsers = new SearchAllUsers
            {
                AllUsers = users.AsQueryable()
                    .OrderBy(u => u.LastName)
                    .Skip((currentPage - 1) * MaxSearchPageUsers)
                    .Take(MaxSearchPageUsers),
                UserPageInfo = new SearchUserPageInfo
                {
                    CurrentPage = currentPage,
                    TotalUsers = users.AsQueryable().Count(),
                    UsersPerPage = MaxSearchPageUsers
                }
            };

            return PartialView("SearchUserPartial", allUsers);
        }

        public ActionResult SearchCoursesPartial(string userId, string keyWord, int currentPage)
        {
            AppUser currentUser = userManager.Users.FirstOrDefault(u => u.Id == userId);

            List<SearchCourse> courses = new List<SearchCourse>();

            foreach (Course course in repository.Courses.Where(c =>
                c.CreatedBy.CompanyName == currentUser.CompanyName ||
                             c.CreatedBy.CompanyName == null))
            {
                if (course.CreatedBy != null && course.Status == "Private")
                {
                    if (course.CreatedBy.Id == currentUser.Id)
                    {
                        if (SearchWordExists(course.Title, keyWord))
                        {
                            SearchCourse searchCourse = new SearchCourse
                            {
                                Course = course,
                                CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                            };
                            courses.Add(searchCourse);
                        }
                    }
                }
                else if (course.Status == "Public")
                {
                    if (SearchWordExists(course.Title, keyWord))
                    {
                        SearchCourse searchCourse = new SearchCourse
                        {
                            Course = course,
                            CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                        };
                        courses.Add(searchCourse);
                    }
                }
            }

            SearchAllCourses allCourses = new SearchAllCourses
            {
                AllCourses = courses.AsQueryable()
                    .OrderBy(c => c.Course.Title)
                    .Skip((currentPage - 1) * MaxSearchPageCourse)
                    .Take(MaxSearchPageCourse),
                CoursePageInfo = new SearchCoursePageInfo
                {
                    CurrentPage = currentPage,
                    TotalCourses = courses.AsQueryable().Count(),
                    CoursesPerPage = MaxSearchPageCourse
                }
            };

            return PartialView("SearchCoursesPartial", allCourses);
        }

        public ActionResult SearchPresentationsPartial(string userId, string keyWord, int currentPage)
        {
            AppUser currentUser = userManager.Users.FirstOrDefault(u => u.Id == userId);

            List<SearchPresentation> presentations = new List<SearchPresentation>();

            foreach (Course course in repository.Courses.Where(c =>
                c.CreatedBy.CompanyName == currentUser.CompanyName ||
                             c.CreatedBy.CompanyName == null))
            {
                if (course.CreatedBy != null && course.Status == "Private")
                {
                    if (course.CreatedBy.Id == currentUser.Id)
                    {
                        foreach (Presentation presentation in course.Presentations)
                        {
                            if (SearchWordExists(presentation.Title, keyWord))
                            {
                                SearchPresentation searchPresentation = new SearchPresentation
                                {
                                    Presentation = presentation,
                                    Course = course,
                                    CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                                };
                                presentations.Add(searchPresentation);
                            }
                        }
                    }
                }
                else if (course.Status == "Public")
                {
                    foreach (Presentation presentation in course.Presentations)
                    {
                        if (SearchWordExists(presentation.Title, keyWord))
                        {
                            SearchPresentation searchPresentation = new SearchPresentation
                            {
                                Presentation = presentation,
                                Course = course,
                                CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                            };
                            presentations.Add(searchPresentation);
                        }
                    }
                }
            }

            SearchAllPresentation allPresentations = new SearchAllPresentation
            {
                AllPresentations = presentations.AsQueryable()
                    .OrderBy(p => p.Presentation.Title)
                    .Skip((currentPage - 1) * MaxSearchPagePresentation)
                    .Take(MaxSearchPagePresentation),
                PresentationPageInfo = new SearchPresentationPageInfo
                {
                    CurrentPage = currentPage,
                    TotalPresentations = presentations.Count(),
                    PresentationsPerPage = MaxSearchPagePresentation
                }
            };

            return PartialView("SearchPresentationsPartial", allPresentations);
        }

        public ActionResult SearchRepresentationsPartial(string userId, string keyWord, int currentPage)
        {
            AppUser currentUser = userManager.Users.FirstOrDefault(u => u.Id == userId);

            List<SearchRepresentation> representations = new List<SearchRepresentation>();
            foreach (Course course in repository.Courses.Where(c => c.Status == "Public"
                         && (c.CreatedBy.CompanyName == currentUser.CompanyName ||
                             c.CreatedBy.CompanyName == null)))
            {
                foreach (Presentation presentation in course.Presentations)
                {
                    foreach (Representation representation in presentation.Representations)
                    {
                        Representation repTmp = repository.Representations
                            .FirstOrDefault(r => r.RepresentationId == representation.RepresentationId);
                        if (SearchWordExists(representation.Title, keyWord))
                        {
                            SearchRepresentation searchRepresentation = new SearchRepresentation
                            {
                                Representation = repTmp,
                                PresentationTitle = presentation.Title,
                                Course = course,
                                Video = repository.Videos.FirstOrDefault(v => v.Id == repTmp.VideoId),
                                CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == repTmp.CreatedBy.Id)
                            };
                            representations.Add(searchRepresentation);
                        }
                    }
                }
            }

            SearchAllRepresentation allRepresentations = new SearchAllRepresentation
            {
                AllRepresentation = representations.AsQueryable()
                    .OrderBy(r => r.Representation.Title)
                    .Skip((currentPage - 1) * MaxSearchPageRepresentation)
                    .Take(MaxSearchPageRepresentation),
                RepresentationPageInfo = new SearchRepresentationPageInfo
                {
                    CurrentPage = currentPage,
                    TotalRepresentation = representations.Count(),
                    RepresentationPerPage = MaxSearchPageRepresentation
                }
            };

            return PartialView("SearchRepresentationsPartial", allRepresentations);
        }

        public async Task<ActionResult> SearchVideoPartial(string userId, string keyWord, int currentPage)
        {
            AppUser currentUser = userManager.Users.FirstOrDefault(u => u.Id == userId);

            List<SearchVideo> videos = new List<SearchVideo>();
            foreach (Course course in repository.Courses.Where(c => c.Status == "Public"
                         && (c.CreatedBy.CompanyName == currentUser.CompanyName ||
                             c.CreatedBy.CompanyName == null)))
            {
                //course video
                if (repository.Videos.Any(v => v.Id == course.VideoId))
                {
                    Video courseVideo = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId);
                    if (SearchWordExists(courseVideo.Title, keyWord))
                    {
                        string info = $"<span class='text-muted'>For: <i class='{course.AwardIcon}'></i>" +
                            $"<strong>{course.Title}</strong></span>";
                        SearchVideo searchVideoC = new SearchVideo
                        {
                            Video = courseVideo,
                            CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == courseVideo.CreatedBy),
                            VideoForInfo = info
                        };
                        videos.Add(searchVideoC);
                    }
                }

                foreach (Presentation presentation in course.Presentations)
                {
                    //presentation video
                    if (repository.Videos.Any(v => v.Id == presentation.VideoId))
                    {
                        Video presentationVideo = repository.Videos.FirstOrDefault(v => v.Id == presentation.VideoId);
                        if (SearchWordExists(presentationVideo.Title, keyWord))
                        {
                            string info = $"<span class='text-muted'>For: <strong>{presentation.Title}</strong> <i class='{course.AwardIcon}'></i>" +
                                $"<strong>{course.Title}</strong></span>";
                            SearchVideo searchVideoP = new SearchVideo
                            {
                                Video = presentationVideo,
                                CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == presentationVideo.CreatedBy),
                                VideoForInfo = info
                            };
                            videos.Add(searchVideoP);
                        }
                    }

                    foreach (Representation representation in presentation.Representations)
                    {
                        Representation repTmp = repository.Representations
                            .FirstOrDefault(r => r.RepresentationId == representation.RepresentationId);
                        //representation video
                        if (repository.Videos.Any(v => v.Id == representation.VideoId))
                        {
                            Video representationVideo = repository.Videos.FirstOrDefault(v => v.Id == representation.VideoId);
                            if (SearchWordExists(representationVideo.Title, keyWord))
                            {
                                if (repository.VideoShareds.Any(v => v.UserId == currentUser.Id &&
                                     v.VideoId == representationVideo.Id) ||
                                     await userManager.IsInRoleAsync(currentUser, "Admin") == true
                                     || await userManager.IsInRoleAsync(currentUser, "Manager") == true
                                     || representationVideo.CreatedBy == currentUser.Id)
                                {
                                    string info = $"<span class='text-muted'>For: <strong>{representation.Title}</strong> <strong>{presentation.Title}</strong> <i class='{course.AwardIcon}'></i>" +
                                        $"<strong>{course.Title}</strong></span>";
                                    SearchVideo searchVideoR = new SearchVideo
                                    {
                                        Video = representationVideo,
                                        CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == representationVideo.CreatedBy),
                                        VideoForInfo = info
                                    };
                                    videos.Add(searchVideoR);
                                }
                            }
                        }
                    }
                }
            }

            //courses where currentUser == course.CreatedBy && status == "Private"
            //& presentations
            foreach (Course course in repository.Courses.Where(c => c.Status == "Private"
                         && (c.CreatedBy.CompanyName == currentUser.CompanyName ||
                             c.CreatedBy.CompanyName == null)))
            {
                if (course.CreatedBy != null)
                {
                    if (course.CreatedBy.Id == currentUser.Id)
                    {
                        //course video
                        if (repository.Videos.Any(v => v.Id == course.VideoId))
                        {
                            Video courseVideo = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId);
                            if (SearchWordExists(courseVideo.Title, keyWord))
                            {
                                string info = $"<span class='text-muted'>For: <i class='{course.AwardIcon}'></i>" +
                                    $"<strong>{course.Title}</strong></span>";
                                SearchVideo searchVideoC = new SearchVideo
                                {
                                    Video = courseVideo,
                                    CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == courseVideo.CreatedBy),
                                    VideoForInfo = info
                                };
                                videos.Add(searchVideoC);
                            }
                        }

                        foreach (Presentation presentation in course.Presentations)
                        {
                            //presentation video
                            if (repository.Videos.Any(v => v.Id == presentation.VideoId))
                            {
                                Video presentationVideo = repository.Videos.FirstOrDefault(v => v.Id == presentation.VideoId);
                                if (SearchWordExists(presentationVideo.Title, keyWord))
                                {
                                    string info = $"<span class='text-muted'>For: <strong>{presentation.Title}</strong> <i class='{course.AwardIcon}'></i>" +
                                        $"<strong>{course.Title}</strong></span>";
                                    SearchVideo searchVideoP = new SearchVideo
                                    {
                                        Video = presentationVideo,
                                        CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == presentationVideo.CreatedBy),
                                        VideoForInfo = info
                                    };
                                    videos.Add(searchVideoP);
                                }
                            }
                        }
                    }
                }
            }

            SearchAllVideos allVideos = new SearchAllVideos
            {
                AllVideos = videos.AsQueryable()
                    .OrderBy(v => v.Video.Title)
                    .Skip((currentPage - 1) * MaxSearchPageVideo)
                    .Take(MaxSearchPageVideo),
                VideoPageInfo = new SearchVideoPageInfo
                {
                    CurrentPage = currentPage,
                    TotalVideos = videos.Count(),
                    VideosPerPage = MaxSearchPageVideo
                }
            };

            return PartialView("SearchVideoPartial", allVideos);
        }


        public ActionResult SearchFilePartial(string userId, string keyWord, int currentPage)
        {
            AppUser currentUser = userManager.Users.FirstOrDefault(u => u.Id == userId);

            List<SearchFiles> files = new List<SearchFiles>();
            foreach (Course course in repository.Courses.Where(c => c.Status == "Public"
                         && (c.CreatedBy.CompanyName == currentUser.CompanyName ||
                             c.CreatedBy.CompanyName == null)))
            {
                foreach (Presentation presentation in course.Presentations)
                {
                    //presentation files
                    foreach (PresentationFiles file1 in repository.PresentationFiles
                        .Where(f => f.PresentationId == presentation.PresentationId))
                    {
                        if (SearchWordExists(file1.Title, keyWord))
                        {
                            string info = $"<span class='text-muted'>For: <strong>{presentation.Title}</strong> <i class='{course.AwardIcon}'></i>" +
                                $"<strong>{course.Title}</strong></span>";
                            SearchFiles searchFile1 = new SearchFiles
                            {
                                File = file1,
                                CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == presentation.CreatedBy.Id),
                                FileForInfo = info
                            };
                            files.Add(searchFile1);
                        }
                    }
                }
            }

            //courses where currentUser == course.CreatedBy && status == "Private"
            //& presentations
            foreach (Course course in repository.Courses.Where(c => c.Status == "Private"
                         && (c.CreatedBy.CompanyName == currentUser.CompanyName ||
                             c.CreatedBy.CompanyName == null)))
            {
                if (course.CreatedBy != null)
                {
                    if (course.CreatedBy.Id == currentUser.Id)
                    {
                        foreach (Presentation presentation in course.Presentations)
                        {
                            //presentation files
                            foreach (PresentationFiles file2 in repository.PresentationFiles
                                .Where(f => f.PresentationId == presentation.PresentationId))
                            {
                                if (SearchWordExists(file2.Title, keyWord))
                                {
                                    string info = $"<span class='text-muted'>For: <strong>{presentation.Title}</strong> <i class='{course.AwardIcon}'></i>" +
                                        $"<strong>{course.Title}</strong></span>";
                                    SearchFiles searchFile2 = new SearchFiles
                                    {
                                        File = file2,
                                        CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == presentation.CreatedBy.Id),
                                        FileForInfo = info
                                    };
                                    files.Add(searchFile2);
                                }
                            }
                        }
                    }
                }
            }

            SearchAllFiles allFiles = new SearchAllFiles
            {
                AllFiles = files.AsQueryable()
                    .OrderBy(f => f.File.Title)
                    .Take(MaxSearchPageFile),
                FilePageInfo = new SearchFilesPageInfo
                {
                    CurrentPage = currentPage,
                    TotalFiles = files.Count(),
                    FilesPerPage = MaxSearchPageFile
                }
            };

            return PartialView("SearchFilePartial", allFiles);
        }

        private bool SearchWordExists (string str, string key)
        {
            if (str == null || key == null)
            {
                return false;
            }

            if (str.Trim().ToLower().IndexOf(key.Trim().ToLower()) > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

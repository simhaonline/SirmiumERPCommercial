using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;

namespace SirmiumCommercial.Controllers
{
    public class ManageController : Controller
    {
        private UserManager<AppUser> userManager;
        private IAppDataRepository repository;
        private IHostingEnvironment hostingEnvironment;
        public int MaxFilesPerPage = 2;
        public int MaxPresentationsPerPage = 2;

        public ManageController(UserManager<AppUser> userMgr,
            IAppDataRepository repo, IHostingEnvironment hosting)
        {
            userManager = userMgr;
            repository = repo;
            hostingEnvironment = hosting;
        }

        public async Task<IActionResult> Index(string id, string status, 
            string sort, string order)
        {
            status = status ?? "Public";
            ViewData["Id"] = id;
            ViewData["Status"] = status;
            ViewData["Sort"] = sort ?? "Date Added";
            ViewData["Order"] = order ?? "desc";

            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                //AllCourses List
                List<AllCourses> allCourses = new List<AllCourses>();

                if (status == "All")
                {
                    foreach (Course course in repository.Courses
                             .Where(c => c.CreatedBy == user))
                    {
                        AllCourses c = new AllCourses
                        {
                            Course = course,
                            Video = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId)
                        };
                        allCourses.Add(c);
                    }
                }
                //status == Public || status == Private
                else
                {
                    foreach (Course course in repository.Courses
                             .Where(c => c.CreatedBy == user
                                && c.Status == status))
                    {
                        AllCourses c = new AllCourses
                        {
                            Course = course,
                            Video = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId)
                        };
                        allCourses.Add(c);
                    }
                }

                switch (sort)
                {
                    case "Date Modified":
                        return View(new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.DateModified).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.DateModified).AsQueryable()
                        });
                    case "End Date":
                        return View(new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.EndDate).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.EndDate).AsQueryable()
                        });
                    case "Title":
                        return View(new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.Title).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.Title).AsQueryable()
                        });
                    default:
                        return View(new AllCoursesViewModel
                        {
                            Content = (order == "asc") ?
                                allCourses.OrderBy(c => c.Course.DateAdded).AsQueryable() :
                                allCourses.OrderByDescending(c => c.Course.DateAdded).AsQueryable()
                        });
                }
            }
            return RedirectToAction("Error", "Error404");
        }

        public async Task<IActionResult> NewCourse(string id)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "Create Course";
            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                return View(new NewEditCourse {
                    Course = new Course(),
                    NoEndDate = false
                });
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewCourse(string id, NewEditCourse model)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "Create Course";
            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                if (ModelState.IsValid)
                {
                    model.Course.Status = "Private";
                    model.Course.CreatedBy = user;
                    model.Course.DateAdded = DateTime.Now;
                    model.Course.DateModified = DateTime.Now;
                    model.Course.Status = "Private";
                    if (model.NoEndDate)
                    {
                        model.Course.EndDate = DateTime.MinValue;
                    }
                    repository.SaveCourse(model.Course);
                   
                    return RedirectToAction("NewCourseStep2", new
                    {
                        id,
                        model.Course.CourseId
                    });
                }
            }
            return View();
        }

        public ViewResult NewCourseStep2(string id, int courseId)
        {
            ViewData["Id"] = id;

            Course course = repository.Courses.FirstOrDefault(c => c.CourseId == courseId);

            string videoTitlePlaceholder = course.Title + "_Video";

            return View("NewCourseStep2", new NewCourseStep2ViewModel {
                CourseId = courseId,
                TitlePlaceholder = videoTitlePlaceholder
            });
        }

        [HttpPost]
        public IActionResult NewCourseStep2(NewCourseStep2ViewModel model)
        {
            Course course = repository.Courses.FirstOrDefault(c => c.CourseId == model.CourseId);

            string base64 = model.videoUrl.Substring(model.videoUrl.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(base64);

            string videoTitle = model.TitlePlaceholder;
            if (model.VideoTitle != null)
            {
                videoTitle = (model.VideoTitle.Trim() == "") ?
                    model.TitlePlaceholder.Replace(" ", "_") :
                    model.VideoTitle;
            }

            var dirPath = Path.Combine(hostingEnvironment.WebRootPath,
                            $@"UsersData\{model.UserId}\Courses\{course.CourseId}");
            System.IO.Directory.CreateDirectory(dirPath);
            var fileName = $@"{course.CourseId}.mp4";
            var filePath = Path.Combine(dirPath, fileName);

            //save course video
            Video video = new Video
            {
                Title = videoTitle,
                CreatedBy = model.UserId,
                Status = "Public",
                For = "Course",
                ForId = course.CourseId,
                DateAdded = DateTime.Now,
                VideoPath = $@"/UsersData/{model.UserId}/Courses/{course.CourseId}/{course.CourseId}.mp4"
            };
            repository.SaveVideo(video);
            course.VideoId = repository.Videos
                .FirstOrDefault(v => v.For == "Course" && v.ForId == course.CourseId).Id;
            repository.SaveCourse(course);

            using (var videoFile = new FileStream(filePath, FileMode.Create))
            {
                videoFile.Write(data, 0, data.Length);
                videoFile.Flush();
            }

            TempData["messageCM"] = "'" + course.Title + " 'has been saved!";

            return RedirectToAction("CourseManage", new { id = model.UserId, courseId = model.CourseId });
        }

        public ActionResult NewCourseNoVideo (string id, int courseId)
        {
            Course course = repository.Courses.FirstOrDefault(c => c.CourseId == courseId);

            TempData["messageCM"] = "'" + course.Title + " 'has been saved!";

            return RedirectToAction("CourseManage", new { id, courseId });
        }

        public ActionResult NewCourseAbort(string id, int courseId)
        {
            repository.DeleteCourse(courseId);

            return RedirectToAction("Index", new { id });
        }

        public ActionResult NewCourseVideo (string id, int courseId)
        {
            ViewData["Id"] = id;

            Course course = repository.Courses
                .FirstOrDefault(c => c.CourseId == courseId);


            return View(new NewCourseStep2ViewModel
            {
                CourseId = courseId,
                TitlePlaceholder = course.Title + "_Video"
            });
        }

        [HttpPost]
        public ActionResult NewCourseVideo(NewCourseStep2ViewModel model)
        {
            Course course = repository.Courses
                .FirstOrDefault(c => c.CourseId == model.CourseId);
            Video video = repository.Videos.FirstOrDefault(v => v.For == "Course" && v.ForId == course.CourseId);

            string base64 = model.videoUrl.Substring(model.videoUrl.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(base64);

            string videoTitle = model.TitlePlaceholder;
            if (model.VideoTitle != null)
            {
                videoTitle = (model.VideoTitle.Trim() == "") ?
                    model.TitlePlaceholder.Replace(" ", "_") :
                    model.VideoTitle;
            }

            var dirPath = Path.Combine(hostingEnvironment.WebRootPath,
                            $@"UsersData\{model.UserId}\Courses\{course.CourseId}");
            System.IO.Directory.CreateDirectory(dirPath);
            var fileName = $@"{course.CourseId}.mp4";
            var filePath = Path.Combine(dirPath, fileName);

            //save course video
            if (video != null)
            {
                video.Title = videoTitle;
                video.Views = 0;
                repository.SaveVideo(video);
                repository.SaveCourse(course);

                //delete video likes & dislikes
                repository.DeleteVideoLikes(video.Id);
                repository.DeleteVideoDislikes(video.Id);
            }
            else
            {
                video = new Video
                {
                    Title = videoTitle,
                    CreatedBy = model.UserId,
                    Status = "Public",
                    For = "Course",
                    ForId = course.CourseId,
                    DateAdded = DateTime.Now,
                    VideoPath = $@"/UsersData/{model.UserId}/Courses/{course.CourseId}/{course.CourseId}.mp4"
                };
                repository.SaveVideo(video);
                course.VideoId = repository.Videos
                    .FirstOrDefault(v => v.For == "Course" && v.ForId == course.CourseId).Id;
                repository.SaveCourse(course);
            }

            using (var videoFile = new FileStream(filePath, FileMode.Create))
            {
                videoFile.Write(data, 0, data.Length);
                videoFile.Flush();
            }

            TempData["messageCM"] = "'" + course.Title + " 'has been saved!";

            return RedirectToAction("EditCourse", new { id = model.UserId, courseId = model.CourseId });
        }

        [HttpPost]
        public IActionResult ChangeCourseVideoName(NewEditCourse model)
        {
            Video video = repository.Videos.FirstOrDefault(v => v.Id == model.VideoId);
            Course course = repository.Courses.FirstOrDefault(c => c.CourseId == model.CourseId);

            if (model.VideoTitle != null && model.VideoTitle.Trim() != "" && model.VideoTitle.Trim() != video.Title)
            {
                video.Title = model.VideoTitle;
                repository.SaveVideo(video);
                TempData["messageCM"] = "'" + course.Title + " 'has been saved!";
            }

            return RedirectToAction("EditCourse", new { id = model.UserId, courseId = model.CourseId });
        }

        public IActionResult CourseManage(string id, int courseId)
        {
            ViewData["Id"] = id;

            List<Video> videos = new List<Video>();
            Course course = repository.Courses
                .FirstOrDefault(c => c.CourseId == courseId);
            //course video
            if (course.VideoId != 0)
            {
                videos.Add(repository.Videos.FirstOrDefault(v => v.Id == course.VideoId));
            }

            foreach (Presentation p in course.Presentations)
            {
                //presentation video
                if (p.VideoId != 0)
                {
                    videos.Add(repository.Videos.
                        FirstOrDefault(v => v.Id == p.VideoId));
                }
            }

            return View(new CourseViewModel {
                Course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == courseId),
                User = repository.Courses.Where(c => c.CreatedBy != null
                    && c.CourseId == courseId).Select(u => u.CreatedBy)
                    .FirstOrDefault(),
                Videos = videos.AsQueryable(),
                AllPresentations = new CourseManagePresentations
                {
                    Presentations = course.Presentations.AsQueryable()
                        .OrderBy(p => p.Part)
                        .Take(MaxPresentationsPerPage),
                    CourseId = courseId,
                    Videos = videos.AsQueryable(),
                    UserId = id,
                    PageInfo = new CoursePresentationsPageInfo
                    {
                        CurrentPage = 1,
                        PresentationsPerPage = MaxPresentationsPerPage,
                        TotalPresentations = course.Presentations.Count()
                    }
                }
            });
        }

        public ActionResult CourseManagePresentationsPartial (string id, int courseId, 
            int currentPage)
        {
            Course course = repository.Courses.FirstOrDefault(c => c.CourseId == courseId);

            List<Video> videos = new List<Video>();
            foreach(Presentation p in course.Presentations)
            {
                if(p.VideoId != 0)
                {
                    videos.Add(repository.Videos.FirstOrDefault(v => v.Id == p.VideoId));
                }
            }

            return PartialView("CourseManagePresentationsPartial", new CourseManagePresentations
            {
                Presentations = course.Presentations.AsQueryable()
                    .OrderBy(p => p.Part)
                    .Skip((currentPage - 1) * MaxPresentationsPerPage)
                    .Take(MaxPresentationsPerPage),
                Videos = videos.AsQueryable(),
                CourseId = courseId,
                UserId = id,
                PageInfo = new CoursePresentationsPageInfo
                {
                    CurrentPage = currentPage,
                    PresentationsPerPage = MaxPresentationsPerPage,
                    TotalPresentations = course.Presentations.Count()
                }
            });
        }
        
        public IActionResult DeleteCourse(string id, int courseId)
        {
            ViewData["Id"] = id;

            //deleting all presentations
            Course course = repository.Courses
                .FirstOrDefault(c => c.CourseId == courseId);
            while(course.Presentations.Count() > 0)
            {
                Presentation p = course.Presentations.FirstOrDefault();
                Presentation deletedP = repository.DeletePresentation(p.PresentationId);
            }

            Course deletedCourse = repository.DeleteCourse(courseId);
            if (deletedCourse != null)
            {
                TempData["messageIn"] = $"Course: '{deletedCourse.Title}' has been deleted!";
            }
            return RedirectToAction("Index", new{id, status = "Private",
                                                sort = "Title", order = "asc"});
        }

        public ViewResult EditCourse(string id, int courseId)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "Edit Course";

            Course course = repository.Courses.FirstOrDefault(c => c.CourseId == courseId);

            return View(new NewEditCourse
            {
                    Course = repository.Courses.Where(c => c.CourseId == courseId)
                                        .FirstOrDefault(),
                    NoEndDate = ((course.EndDate == DateTime.MinValue) ? true : false),
                    Video = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId)
            });
        }

        [HttpPost]
        public IActionResult EditCourse(string id, NewEditCourse model)
        {
            ViewData["Id"] = id;
            ViewData["Title"] = "Edit Course";

            Course course = repository.Courses
                    .Where(c => c.CourseId == model.CourseId)
                    .FirstOrDefault();
            bool ind = false;

            if (model.NewTitle != null)
            {
                if (course.Title == null)
                {
                    course.Title = model.NewTitle;
                    ind = true;
                }
                else if (model.NewTitle.Trim() != "" && model.NewTitle.Trim() != course.Title)
                {
                    course.Title = model.NewTitle;
                    ind = true;
                }
            }

            if (model.NewNoEndDate && (course.EndDate > DateTime.MinValue))
            {
                course.EndDate = DateTime.MinValue;
                ind = true;
            }

            if(model.NewEndDate != null)
            {
                if(model.NewEndDate != DateTime.MinValue && model.NewEndDate != course.EndDate)
                {
                    course.EndDate = model.NewEndDate;
                    ind = true;
                }
            }

            if (model.NewDescription != null && course.Description != null)
            {
                if (model.NewDescription.Trim() != "" && model.NewDescription.Trim() != course.Description)
                {
                    course.Description = model.NewDescription;
                    ind = true;
                }
            }
            else if (model.NewDescription != null && course.Description == null)
            {
                if (model.NewDescription.Trim() != "")
                {
                    course.Description = model.NewDescription;
                    ind = true;
                }
            }

            if(model.NewAwardIcon != null && course.AwardIcon == null)
            {
                course.AwardIcon = model.NewAwardIcon;
                ind = true;
            }
            if(model.NewAwardIcon != null)
            {
                if(model.NewAwardIcon.Trim() != "" && model.NewAwardIcon.Trim() != course.AwardIcon.Trim())
                {
                    course.AwardIcon = model.NewAwardIcon;
                    ind = true;
                }
            }


            //if something has changed
            if (ind)
            {
                repository.SaveCourse(course);
                TempData["messageCM"] = "'" + course.Title + " 'has been saved!";
            }

            return RedirectToAction("EditCourse", new
            {
                id,
                model.CourseId
            });
        }

        public IActionResult ChangeCourseStatus(string id, int courseId)
        {
            ViewData["Id"] = id;
            Course course = repository.Courses.Where(c => c.CourseId == courseId)
                                       .FirstOrDefault();
            switch (course.Status)
            {
                case "Private":
                    course.Status = "Public";
                    course.DateAdded = DateTime.Now;
                    course.DateModified = DateTime.Now;
                    repository.SaveCourse(course);
                    return RedirectToAction("Index", new
                    {
                        id,
                        status = "Public",
                        sort = "Date Added",
                        order = "asc"
                    });
                default:
                    course.Status = "Private";
                    course.DateModified = DateTime.Now;
                    repository.SaveCourse(course);
                    return RedirectToAction("CourseManage", new { id, courseId });
            }
        }

        public IActionResult NewPresentation (string id, int courseId)
        {
            ViewData["Id"] = id;

            Course course = repository.Courses.FirstOrDefault(c => c.CourseId == courseId);

            if(course != null)
            {
                if (course.CreatedBy.Id == id)
                {
                    return View(new NewPresentationStep1ViewModel
                    {
                        CourseId = course.CourseId,
                        PresentationPart = course.Presentations.Count() + 1
                    });
                }
                else
                {
                    return RedirectToAction("CourseManage", new
                    {
                        id,
                        courseId
                    });
                }
            }
            else
            {
                return RedirectToAction("CourseManage", new
                {
                    id,
                    courseId
                });
            }
        }

        [HttpPost]
        public IActionResult NewPresentation(NewPresentationStep1ViewModel model)
        {
            AppUser appUser = userManager.Users.FirstOrDefault(u => u.Id == model.UserId);
            Course course = repository.Courses
                .Where(c => c.CourseId == model.CourseId)
                .FirstOrDefault();

            string title = (model.Presentation.Title == null ||
                model.Presentation.Title.Trim() == "") ?
                course.Title + "_Presentation_" + model.Presentation.Part :
                model.Presentation.Title;

            Presentation presentation = model.Presentation;
            presentation.CreatedBy = appUser;
            presentation.DateAdded = DateTime.Now;
            presentation.DateModified = DateTime.Now;
            presentation.CreatedAt = DateTime.Now;
            presentation.UpdatedAt = DateTime.Now;

            course.UpdatedAt = DateTime.Now;
            course.Presentations.Add(presentation);
            repository.SaveCourse(course);

            return RedirectToAction("NewPresentationStep2", new {
                id = appUser.Id,
                presentationId = presentation.PresentationId,
                courseId = course.CourseId
            });
        }

        public IActionResult NewPresentationStep2(string id, int presentationId, int courseId)
        {
            ViewData["Id"] = id;

            Presentation presentation = repository.Presentations
                .FirstOrDefault(p => p.PresentationId == presentationId);


            return View(new NewPresentationStep2ViewModel
            {
                CourseId = courseId,
                PresentationId = presentation.PresentationId,
                TitlePlaceholder = presentation.Title + "_Video"
            });
        }

        [HttpPost]
        public IActionResult NewPresentationStep2(NewPresentationStep2ViewModel model)
        {
            AppUser user = userManager.Users
                .FirstOrDefault(u => u.Id == model.UserId);
            Presentation presentation = repository.Presentations
                .FirstOrDefault(p => p.PresentationId == model.PresentationId);

            string base64 = model.videoUrl.Substring(model.videoUrl.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(base64);

            //Create video directory
            string videoTitle = model.TitlePlaceholder;
            if (model.VideoTitle != null)
            {
                videoTitle = (model.VideoTitle.Trim() == "") ?
                    model.TitlePlaceholder.Replace(" ", "_") :
                    model.VideoTitle;
            }

            var dirPath = Path.Combine(hostingEnvironment.WebRootPath,
                            $@"UsersData\{user.Id}\Presentations\");
            System.IO.Directory.CreateDirectory(dirPath);
            var fileName = $@"{presentation.PresentationId}.mp4";
            var filePath = Path.Combine(dirPath, fileName);

            //save presentation video 
            Video video = new Video
            {
                Title = videoTitle,
                CreatedBy = model.UserId,
                Status = "Public",
                For = "Presentation",
                ForId = presentation.PresentationId,
                DateAdded = DateTime.Now,
                VideoPath = $@"/UsersData/{user.Id}/Presentations/{presentation.PresentationId}.mp4"
            };

            repository.SaveVideo(video);
            presentation.VideoId = repository.Videos
                .FirstOrDefault(v => v.For == "Presentation" &&
                v.ForId == presentation.PresentationId).Id;
            presentation.UpdatedAt = DateTime.Now;
            repository.SavePresentation(presentation);


            using (var videoFile = new FileStream(filePath, FileMode.Create))
            {
                videoFile.Write(data, 0, data.Length);
                videoFile.Flush();
            }

            return RedirectToAction("NewPresentationStep3", new
            {
                id = model.UserId,
                presentationId = presentation.PresentationId,
                courseId = model.CourseId
            });
        }

        public IActionResult NewPresentationStep3(string id, int presentationId, int courseId)
        {
            ViewData["Id"] = id;

            Presentation presentation = repository.Presentations
                .FirstOrDefault(p => p.PresentationId == presentationId);

            int filePart = repository.PresentationFiles
                .Where(f => f.PresentationId == presentationId).Count() + 1;

            return View(new NewPresentationStep3ViewModel{
                UserId = id,
                PresentationId = presentationId,
                FileTitle = presentation.Title + "_File_Part_" + filePart,
                CourseId = courseId,
                Part = filePart
            });
        }

        [HttpPost]
        public IActionResult NewPresentationStep3(NewPresentationStep3ViewModel model)
        {
            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == model.UserId);
            Presentation presentation = repository.Presentations
                .FirstOrDefault(p => p.PresentationId == model.PresentationId);
            int filePart = repository.PresentationFiles
                .Where(f => f.PresentationId == model.PresentationId).Count() + 1;

            //Create user directory
            string dirPath = Path.Combine(hostingEnvironment.WebRootPath, $@"UsersData\{user.Id}\Presentations\");
            System.IO.Directory.CreateDirectory(dirPath);

            string fileName = (model.FileTitle == null || model.FileTitle.Trim() == "") ?
                presentation.Title + "_File_Part_" + filePart : model.FileTitle;

            //save presentation file 
            PresentationFiles file = new PresentationFiles
            {
                Title = fileName,
                CreatedById = user.Id,
                Part = model.Part,
                PresentationId = presentation.PresentationId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            repository.SaveFile(file);
            file.FilePath = $@"/UsersData/{user.Id}/Presentations/{file.FileId}.pdf";
            repository.SaveFile(file);

            string filePath = Path.Combine(dirPath, file.FileId.ToString()+".pdf");
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                model.File.CopyTo(stream);
            }

            ViewData["Id"] = user.Id;
            filePart = repository.PresentationFiles
                .Where(f => f.PresentationId == model.PresentationId).Count() + 1;

            return View(new NewPresentationStep3ViewModel
            {
                UserId = user.Id,
                PresentationId = presentation.PresentationId,
                FileTitle = presentation.Title + "_File_Part_" + filePart,
                CourseId = model.CourseId,
                Part = filePart,
                PresentationFiles = repository.PresentationFiles
                .Where(f => f.PresentationId == presentation.PresentationId)
            });
        }

        public IActionResult NewPresentationAbort(string id, int presentationId,
            int courseId)
        {
            repository.DeletePresentation(presentationId);

            return RedirectToAction("CourseManage", new { id, courseId });
        }

        public ViewResult EditPresentation (string id, int presentationId, int courseId)
        {
            ViewData["Id"] = id;

            Presentation presentation = repository.Presentations.FirstOrDefault(p => p.PresentationId == presentationId);
            Video video = repository.Videos.FirstOrDefault(v => v.Id == presentation.VideoId);
            EditPresentationFiles files = new EditPresentationFiles
            {
                Files = repository.PresentationFiles
                    .Where(f => f.PresentationId == presentation.PresentationId)
                    .OrderBy(f => f.Part)
                    .Take(MaxFilesPerPage),
                PresentationId = presentation.PresentationId,
                CourseId = courseId,
                UserId = id,
                PageInfo = new EditPresentationFilesPageInfo
                {
                    CurrentPage = 1,
                    FilesPerPage = MaxFilesPerPage,
                    TotalFiles = repository.PresentationFiles
                        .Where(f => f.PresentationId == presentation.PresentationId)
                        .Count()
                }
            };

            return View(new EditPresentation
            {
                Presentation = presentation,
                Video = video,
                CourseId = courseId,
                Files = files
            });
        }

        public ActionResult EditPresentationFilesPartial (int presentationId, int courseId,
            string userId, int currentPage)
        {
            return PartialView("EditPresentationFilesPartial", new EditPresentationFiles
            {
                Files = repository.PresentationFiles
                    .Where(f => f.PresentationId == presentationId)
                    .OrderBy(f => f.Part)
                    .Skip((currentPage - 1) * MaxFilesPerPage)
                    .Take(MaxFilesPerPage),
                PresentationId = presentationId,
                UserId = userId,
                CourseId = courseId,
                PageInfo = new EditPresentationFilesPageInfo
                {
                    CurrentPage = currentPage,
                    FilesPerPage = MaxFilesPerPage,
                    TotalFiles = repository.PresentationFiles
                        .Where(f => f.PresentationId == presentationId)
                        .Count()
                }
            });
        }

        [HttpPost]
        public IActionResult EditPresentation(EditPresentation model)
        {
            Presentation presentation = repository.Presentations.FirstOrDefault(p => p.PresentationId == model.PresentationId);

            if(model.Title != null)
            {
                if (model.Title.Trim() != "" && model.Title.Trim() != presentation.Title)
                {
                    presentation.Title = model.Title;
                    repository.SavePresentation(presentation);
                }
            }

            if(model.Part != presentation.Part && model.Part > 0)
            {
                presentation.Part = model.Part;
                repository.SavePresentation(presentation);
            }

            if (model.Description != null && presentation.Description != null)
            {
                if (model.Description.Trim() != "" && model.Description.Trim() != presentation.Description)
                {
                    presentation.Description = model.Description;
                    repository.SavePresentation(presentation);
                }
            }
            else if (model.Description != null && presentation.Description == null)
            {
                if (model.Description.Trim() != "")
                {
                    presentation.Description = model.Description;
                    repository.SavePresentation(presentation);
                }
            }

            return RedirectToAction("EditPresentation", new {
                id = model.UserId,
                presentationId = model.PresentationId,
                courseId = model.CourseId
            });
        }

        public ViewResult NewPresentationVideo(string id, int presentationId, int courseId)
        {
            ViewData["Id"] = id;

            Presentation presentation = repository.Presentations
                .FirstOrDefault(p => p.PresentationId == presentationId);


            return View(new NewPresentationStep2ViewModel
            {
                CourseId = courseId,
                PresentationId = presentation.PresentationId,
                TitlePlaceholder = presentation.Title + "_Video"
            });
        }

        [HttpPost]
        public IActionResult NewPresentationVideo(NewPresentationStep2ViewModel model)
        {
            AppUser user = userManager.Users
                .FirstOrDefault(u => u.Id == model.UserId);
            Presentation presentation = repository.Presentations
                .FirstOrDefault(p => p.PresentationId == model.PresentationId);

            string base64 = model.videoUrl.Substring(model.videoUrl.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(base64);

            //Create video directory
            string videoTitle = model.TitlePlaceholder;
            if (model.VideoTitle != null)
            {
                videoTitle = (model.VideoTitle.Trim() == "") ?
                    model.TitlePlaceholder.Replace(" ", "_") :
                    model.VideoTitle;
            }

            var dirPath = Path.Combine(hostingEnvironment.WebRootPath,
                            $@"UsersData\{user.Id}\Presentations\");
            System.IO.Directory.CreateDirectory(dirPath);
            var fileName = $@"{presentation.PresentationId}.mp4";
            var filePath = Path.Combine(dirPath, fileName);

            //save presentation video 
            if(presentation.VideoId != 0)
            {
                Video presVideo = repository.Videos.FirstOrDefault(v => v.Id == presentation.VideoId);
                presVideo.Title = videoTitle;
                presVideo.Views = 0;
                repository.SaveVideo(presVideo);

                //delete video likes & dislikes
                repository.DeleteVideoLikes(presVideo.Id);
                repository.DeleteVideoDislikes(presVideo.Id);
            }
            else
            {
                Video video = new Video
                {
                    Title = videoTitle,
                    CreatedBy = model.UserId,
                    Status = "Public",
                    For = "Presentation",
                    ForId = presentation.PresentationId,
                    DateAdded = DateTime.Now,
                    VideoPath = $@"/UsersData/{user.Id}/Presentations/{presentation.PresentationId}.mp4"
                };

                repository.SaveVideo(video);
                presentation.VideoId = repository.Videos
                    .FirstOrDefault(v => v.For == "Presentation" &&
                    v.ForId == presentation.PresentationId).Id;
                presentation.UpdatedAt = DateTime.Now;
                repository.SavePresentation(presentation);
            }

            using (var videoFile = new FileStream(filePath, FileMode.Create))
            {
                videoFile.Write(data, 0, data.Length);
                videoFile.Flush();
            }

            return RedirectToAction("EditPresentation", new
            {
                id = model.UserId,
                presentationId = presentation.PresentationId,
                courseId = model.CourseId
            });
        }

        public ViewResult ChangePresentationVideo(string id, int presentationId, int courseId)
        {
            ViewData["Id"] = id;

            Presentation presentation = repository.Presentations
                .FirstOrDefault(p => p.PresentationId == presentationId);


            return View(new NewPresentationStep2ViewModel
            {
                CourseId = courseId,
                PresentationId = presentation.PresentationId,
                TitlePlaceholder = presentation.Title + "_Video"
            });
        }

        [HttpPost]
        public IActionResult ChangeVideoName(EditPresentation model)
        {
            Video video = repository.Videos.FirstOrDefault(v => v.Id == model.VideoId);

            if (model.VideoTitle != null && model.VideoTitle.Trim() != "" && model.VideoTitle.Trim() != video.Title)
            {
                video.Title = model.VideoTitle;
                repository.SaveVideo(video);
            }

            return RedirectToAction("EditPresentation", new
            {
                id = model.UserId,
                presentationId = model.PresentationId,
                courseId = model.CourseId
            });
        }

        [HttpPost]
        public IActionResult ChangeFileInfo(EditPresentationFiles model)
        {
            PresentationFiles file = repository.PresentationFiles.FirstOrDefault(f => f.FileId == model.FileId);

            if(file != null)
            {
                if(model.FileTitle != null && model.FileTitle.Trim() != "" && model.FileTitle.Trim() != file.Title)
                {
                    file.Title = model.FileTitle;
                    repository.SaveFile(file);
                }
                else if(model.FilePart > 0 && model.FilePart != file.Part)
                {
                    file.Part = model.FilePart;
                    repository.SaveFile(file);
                }
            }

            return RedirectToAction("EditPresentation", new
            {
                id = model.UserId,
                presentationId = model.PresentationId,
                courseId = model.CourseId
            });
        }

        [HttpPost]
        public IActionResult UploadNewFile(EditPresentation model)
        {
            if (model.NewFile != null)
            {
                AppUser user = userManager.Users.FirstOrDefault(u => u.Id == model.UserId);
                Presentation presentation = repository.Presentations
                    .FirstOrDefault(p => p.PresentationId == model.PresentationId);
                int filePart = repository.PresentationFiles
                    .Where(f => f.PresentationId == model.PresentationId).Count() + 1;

                //Create user directory
                string dirPath = Path.Combine(hostingEnvironment.WebRootPath, $@"UsersData\{user.Id}\Presentations\");
                System.IO.Directory.CreateDirectory(dirPath);

                PresentationFiles file = repository.PresentationFiles.FirstOrDefault(f => f.FileId == model.NewFileId);
                //file exists
                if(file != null)
                {
                    repository.SaveFile(file);
                }
                else
                {
                    string fileName = "";
                    if (model.NewFileTitle == null)
                    {
                        fileName = presentation.Title + "_File_Part_" + filePart;
                    }
                    else
                    {
                        fileName = (model.NewFileTitle.Trim() == "") ?
                            presentation.Title + "_File_Part_" + filePart : model.NewFileTitle;
                    }

                    //save presentation file 
                    file = new PresentationFiles
                    {
                        Title = fileName,
                        CreatedById = user.Id,
                        Part = model.NewFilePart,
                        PresentationId = presentation.PresentationId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };
                    repository.SaveFile(file);
                    file.FilePath = $@"/UsersData/{user.Id}/Presentations/{file.FileId}.pdf";
                    repository.SaveFile(file);
                }

                string filePath = Path.Combine(dirPath, file.FileId.ToString() + ".pdf");
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.NewFile.CopyTo(stream);
                }
            }

            return RedirectToAction("EditPresentation", new
            {
                id = model.UserId,
                presentationId = model.PresentationId,
                courseId = model.CourseId
            });
        }
        
        public IActionResult DeleteFile (string id, int presentationId, int courseId, int fileId)
        {
            PresentationFiles file = repository.PresentationFiles.FirstOrDefault(f => f.FileId == fileId);
            string dirPath = Path.Combine(hostingEnvironment.WebRootPath, $@"UsersData\{id}\Presentations\");
            string filePath = Path.Combine(dirPath, file.FileId.ToString() + ".pdf");

            //delete file if exists
            System.IO.File.Delete(filePath);

            repository.DeleteFile(file.FileId);

            return RedirectToAction("EditPresentation", new { id, presentationId, courseId });
        }

        public IActionResult DeletePresentation(string id, int courseId, int presentationId)
        {
            ViewData["Id"] = id;

            Presentation deletedPre = repository.DeletePresentation(presentationId);
            if (deletedPre != null)
            {
                TempData["messageCM"] = $"Presentation: '{deletedPre.Title}' has been deleted!";
            }
            else
            {
                TempData["errMsgCM"] = "Sorry, something went wrong!";

            }
            return RedirectToAction("CourseManage",
                new { id, courseId});
        }
    }
}

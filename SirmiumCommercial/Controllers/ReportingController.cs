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
    public class ReportingController : Controller
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;
        public int CourseReportsPageSize = 2;
        public int PresentationReportsPageSize = 2;

        public ReportingController (UserManager<AppUser> userMgr, 
            IAppDataRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public IActionResult Index(string id, int currentPage = 1)
        {
            ViewData["Id"] = id;

            AppUser currentUser = userManager.Users.FirstOrDefault(u => u.Id == id);

            List<CourseReportingViewModel> courses = new List<CourseReportingViewModel>();
            foreach(Course course in repository.Courses)
            {
                if (course.CreatedBy != null)
                {
                    if(course.CreatedBy.CompanyName == currentUser.CompanyName)
                    {
                        CourseReportingViewModel course1 = new CourseReportingViewModel
                        {
                            Course = course
                        };

                        //users on course
                        List<UserRepresentationsInfo> usersInfo = new List<UserRepresentationsInfo>();
                        foreach (CourseUsers courseUser in repository.CourseUsers
                            .Where(cu => cu.CourseId == course.CourseId))
                        {
                            double userRating = 0;
                            double userRepres = 0;
                            foreach (Presentation presentation in course.Presentations)
                            {
                                foreach (Representation representation in presentation.Representations)
                                {
                                    if (representation.CreatedBy != null)
                                    {
                                        if (representation.CreatedBy.Id != null ||
                                            representation.CreatedBy.Id != "")
                                        {
                                            if(representation.CreatedBy.Id == courseUser.AppUserId)
                                            {
                                                userRating += (representation.Rating > 0) ? 
                                                    representation.Rating : 0;
                                                userRepres += (representation.Rating > 0) ? 1 : 0;
                                            }
                                        }
                                    }
                                }
                            }
                            double avgRating = (userRepres > 0) ? userRating / userRepres : 0;
                            UserRepresentationsInfo userInfo = new UserRepresentationsInfo
                            {
                                User = userManager.Users.FirstOrDefault(u => u.Id == courseUser.AppUserId),
                                AvgRating = avgRating,
                                TotalRepresentations = (int)userRepres
                            };
                            usersInfo.Add(userInfo);
                        }
                        course1.UsersOnCourse = usersInfo.AsQueryable();
                        courses.Add(course1);
                    }
                }
            }

            return View(new CourseReportsPageViewModel
            {
                ViewModel = courses.AsQueryable()
                    .OrderBy(c => c.Course.Title)
                    .Skip((currentPage - 1) * CourseReportsPageSize)
                    .Take(CourseReportsPageSize),
                PageInfo = new CourseReportsPageInfo
                {
                    CurrentPage = currentPage,
                    CoursesPerPage = CourseReportsPageSize,
                    TotalCourses = courses.Count()
                }
            });
        }

        public IActionResult MyProgress(string id)
        {
            return RedirectToAction("UserProgress", new { id, userId = id });
        }

        public IActionResult PresentationReports(string id, int courseId, int currentPage = 1)
        {
            ViewData["Id"] = id;

            AppUser currentUser = userManager.Users.FirstOrDefault(u => u.Id == id);
            Course course = repository.Courses.FirstOrDefault(c => c.CourseId == courseId);

            List<PresentationReportsInfo> presentationsInfos = new List<PresentationReportsInfo>();
            foreach (Presentation presentation in course.Presentations)
            {
                List<PresentationUserInfo> usersInfos = new List<PresentationUserInfo>();
                foreach (Representation representation in presentation.Representations)
                {
                    AppUser user = RepresentationCreatedBy(representation.RepresentationId);
                    PresentationUserInfo info = new PresentationUserInfo
                    {
                        User = user,
                        Rating = representation.Rating
                    };

                    usersInfos.Add(info);
                }
                presentationsInfos.Add(new PresentationReportsInfo
                {
                    Presentation = presentation,
                    Users = usersInfos.AsQueryable()
                });
            }

            return View(new PresentationPageViewModel
            {
                Course = course,
                Presentations = presentationsInfos.AsQueryable()
                    .OrderBy(p => p.Presentation.Part)
                    .Skip((currentPage - 1) * PresentationReportsPageSize)
                    .Take(PresentationReportsPageSize),
                PageInfo = new PresentationReportsPageInfo
                {
                    CurrentPage = currentPage,
                    PresentationsPerPage = PresentationReportsPageSize,
                    TotalPresentations = presentationsInfos.Count()
                }
            });
        }

        public IActionResult UserProgress(string id, string userId)
        {
            ViewData["Id"] = id;

            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == userId);

            MyProgressViewModel myProgress = new MyProgressViewModel
            {
                User = user
            };
            double myRating = 0;
            double myCompletedPresentations = 0;
            int myViews = 0;
            int myRepresentations = 0;

            List<MyProgressCourseInfo> Courses = new List<MyProgressCourseInfo>();
            foreach (Course course in repository.Courses)
            {
                if (repository.CourseUsers
                    .Any(cu => cu.AppUserId == user.Id && cu.CourseId == course.CourseId))
                {
                    MyProgressCourseInfo courseProgress = new MyProgressCourseInfo
                    {
                        Course = course
                    };
                    double courseRating = 0;
                    double completedPresentations = 0;

                    List<MyProgressPresentationsInfo> presentationsInfo = new List<MyProgressPresentationsInfo>();
                    foreach (Presentation presentation in course.Presentations)
                    {
                        MyProgressPresentationsInfo presentationProgress = new MyProgressPresentationsInfo
                        {
                            Presentation = presentation
                        };
                        Representation repres = UserRepresentation(user.Id, presentation.PresentationId);
                        if (repres != null)
                        {
                            presentationProgress.Rating = repres.Rating;
                            courseRating += repres.Rating;
                            completedPresentations += (courseRating > 0) ? 1.0 : 0;

                            myRepresentations++;
                            myViews += repository.Videos.FirstOrDefault(v => v.Id == repres.VideoId).Views;
                        }
                        presentationsInfo.Add(presentationProgress);
                    }
                    courseProgress.Presentations = presentationsInfo.AsQueryable();
                    courseProgress.AvgRating = (completedPresentations > 0) ?
                        courseRating / completedPresentations * 1.0 : 0;
                    courseProgress.CompletedPresentations = (int)completedPresentations;

                    myRating += courseRating;
                    myCompletedPresentations += completedPresentations;

                    Courses.Add(courseProgress);
                }
            }

            myProgress.Courses = Courses.AsQueryable();
            myProgress.AvgRating = (myCompletedPresentations > 0) ?
                myRating / myCompletedPresentations * 1.0 : 0;
            myProgress.Representations = myRepresentations;
            myProgress.Views = myViews;


            return View(myProgress);
        }

        private Representation UserRepresentation (string id, int presentationId)
        {
            Presentation pres = repository.Presentations.FirstOrDefault(p => p.PresentationId == presentationId);

            foreach (Representation repres in pres.Representations)
            {
                if(repres != null && repres.CreatedBy != null)
                {
                    if (repres.CreatedBy.Id != null)
                    {
                        if (repres.CreatedBy.Id == id)
                        {
                            return repres;
                        }
                    }
                }
            }
            return null;
        }

        private AppUser RepresentationCreatedBy (int representationId)
        {
            return userManager.Users
                .FirstOrDefault(u => u.Id == repository.Representations
                .FirstOrDefault(r => r.RepresentationId == representationId).CreatedBy.Id);
        }
    }
}

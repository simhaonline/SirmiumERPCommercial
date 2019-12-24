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

        public ReportingController (UserManager<AppUser> userMgr, 
            IAppDataRepository repo)
        {
            userManager = userMgr;
            repository = repo;
        }

        public IActionResult Index(string id)
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
                    }
                }
            }

            return View();
        }

        public IActionResult MyProgress(string id)
        {
            ViewData["Id"] = id;

            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == id);

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
                    .Any(cu => cu.AppUserId == id && cu.CourseId == course.CourseId))
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
                        /*Representation repres = presentation.Representations
                            .FirstOrDefault(r => r.CreatedBy.Id == user.Id);*/
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
    }
}

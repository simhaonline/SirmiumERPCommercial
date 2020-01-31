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

        public int MAX_SEARCH_PAGE = 4;

        public SearchController(UserManager<AppUser> userMgr,
                IAppDataRepository repo, IHostingEnvironment hosting)
        {
            userManager = userMgr;
            repository = repo;
            hostingEnvironment = hosting;
        }

        [HttpPost]
        public ViewResult Search(string id, string KeyWord)
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
                AllUsers = users.AsQueryable().OrderBy(u => u.LastName),
                UserPageInfo = new SearchUserPageInfo
                {
                    CurrentPage = 1,
                    TotalUsers = users.AsQueryable().Count(),
                    UsersPerPage = MAX_SEARCH_PAGE
                }
            };

            //courses for ALL USERS
            //& presentations
            List<SearchCourse> courses = new List<SearchCourse>();
            List<SearchPresentation> presentations = new List<SearchPresentation>();
            foreach(Course course in repository.Courses.Where(c => c.Status == "Public"
                        && (c.CreatedBy.CompanyName == currentUser.CompanyName ||
                            c.CreatedBy.CompanyName == null)))
            {
                if (SearchWordExists(course.Title, KeyWord))
                {
                    SearchCourse searchCourse = new SearchCourse
                    {
                        Course = course,
                        CourseVideo = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId),
                        CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                    };
                    courses.Add(searchCourse);
                }

                foreach (Presentation presentation in course.Presentations)
                {
                    if (SearchWordExists(presentation.Title, KeyWord))
                    {
                        SearchPresentation searchPresentation = new SearchPresentation
                        {
                            Presentation = presentation,
                            PresentationVideo = repository.Videos.FirstOrDefault(v => v.Id == presentation.VideoId),
                            CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                        };
                        presentations.Add(searchPresentation);
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
                                CourseVideo = repository.Videos.FirstOrDefault(v => v.Id == course.VideoId),
                                CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                            };
                            courses.Add(searchCourse);
                        }

                        foreach (Presentation presentation in course.Presentations)
                        {
                            if (SearchWordExists(presentation.Title, KeyWord))
                            {
                                SearchPresentation searchPresentation = new SearchPresentation
                                {
                                    Presentation = presentation,
                                    PresentationVideo = repository.Videos.FirstOrDefault(v => v.Id == presentation.VideoId),
                                    CreatedBy = userManager.Users.FirstOrDefault(u => u.Id == course.CreatedBy.Id)
                                };
                                presentations.Add(searchPresentation);
                            }
                        }
                    }
                }
            }
            SearchAllCourses allCourses = new SearchAllCourses
            {
                AllCourses = courses.AsQueryable().OrderBy(c => c.Course.Title),
                CoursePageInfo = new SearchCoursePageInfo
                {
                    CurrentPage = 1,
                    TotalCourses = courses.AsQueryable().Count(),
                    CoursesPerPage = MAX_SEARCH_PAGE
                }
            };
            SearchAllPresentation allPresentation = new SearchAllPresentation
            {
                AllPresentations = presentations.AsQueryable().OrderBy(p => p.Presentation.Title),
                PresentationPageInfo = new SearchPresentationPageInfo
                {
                    CurrentPage = 1,
                    TotalPresentations = presentations.AsQueryable().Count(),
                    PresentationsPerPage = MAX_SEARCH_PAGE
                }
            };

            return View("Search", new SearchViewModel
            {
                Users = allUsers,
                Courses = allCourses,
                Presentations = allPresentation
            });
        }

        private bool SearchWordExists (string str, string key)
        {
            if (str == null)
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

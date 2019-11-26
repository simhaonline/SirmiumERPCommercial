using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models.ViewModels;
using SirmiumCommercial.Models;

namespace SirmiumCommercial.Controllers
{
    public class GroupsController : Controller
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public GroupsController (IAppDataRepository repo, UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public async Task<ViewResult> Groups(string id)
        {
            ViewData["Id"] = id;

            AppUser user = await userManager.FindByIdAsync(id);

            return View(repository.Groups.Where(g => g.CompanyName == user.CompanyName));
        }

        public IActionResult GroupDetails(string id, int groupId)
        {
            ViewData["Id"] = id;

            Group group = repository.Groups
                .FirstOrDefault(g => g.GroupId == groupId);

            if(group != null)
            {
                //users 
                List<AppUser> users = new List<AppUser>();
                foreach(GroupUsers groupUser in repository.GroupUsers
                            .Where(g => g.GroupId == groupId))
                {
                    AppUser user = userManager.Users
                        .FirstOrDefault(u => u.Id == groupUser.AppUserId);
                    users.Add(user);
                }

                //courses
                List<GroupCourseDetails> courses = new List<GroupCourseDetails>();
                foreach(GroupCourses groupCourse in repository.GroupCourses
                            .Where(g => g.GroupId == groupId))
                {
                    Course course = repository.Courses
                        .FirstOrDefault(c => c.CourseId == groupCourse.CourseId);

                    //users on course
                    List<AppUser> courseUsers = new List<AppUser>();
                    foreach (CourseUsers cUser in repository.CourseUsers
                                .Where(c => c.CourseId == course.CourseId))
                    {
                        AppUser user = userManager.Users
                            .FirstOrDefault(u => u.Id == cUser.AppUserId);
                        courseUsers.Add(user);
                    }

                    GroupCourseDetails courseDetails = new GroupCourseDetails
                    {
                        Course = course,
                        CourseUsers = courseUsers.AsQueryable(),
                        Video = repository.Videos
                            .FirstOrDefault(v => v.Id == course.VideoId)
                    };
                    courses.Add(courseDetails);
                }

                return View(new GroupDetailsViewModel
                {
                    Group = group,
                    Users = users.AsQueryable(),
                    Courses = courses.AsQueryable()
                });
            }
            else
            {
                return RedirectToAction("Error", "GroupNotFound");
            }
        }

        public ViewResult NewGroup(string id)
        {
            ViewData["Id"] = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NewGroup(NewGroupViewModel model)
        {
            ViewData["Id"] = model.CreatedById;

            AppUser user = await userManager.FindByIdAsync(model.CreatedById);
            if (user != null)
            {
                Group newGroup = new Group
                {
                    Name = model.Name,
                    Description = model.Description,
                    CreatedBy = user,
                    CompanyName = user.CompanyName
                };

                int groupId = repository.SaveGroup(newGroup);
                repository.AddUserToGroup(user.Id, groupId);

                return RedirectToAction("NewGroupStep2", new { id = model.CreatedById, groupId });
            }

            return RedirectToAction("Error", "UserNotFound");
        }

        public IActionResult NewGroupStep2(string id, int groupId)
        {
            ViewData["Id"] = id;

            Group group = repository.Groups.FirstOrDefault(g => g.GroupId == groupId);

            if (group.CreatedBy.Id == id)
            {
                return View(new NewGroupStep2ViewModel
                {
                    Group = group,
                    Users = userManager.Users.Where(u => u.CompanyName == group.CreatedBy.CompanyName
                            && u.Id != id)
                });
            }
            else
            {
                return RedirectToAction("Error", "AccessDenied");
            }
        }

        [HttpPost]
        public async Task<IActionResult> NewGroupStep2(NewGroupStep2ViewModel model)
        {
            ViewData["Id"] = model.UserId;

            AppUser user = await userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                //group
                Group group = repository.Groups
                    .FirstOrDefault(g => g.GroupId == model.GroupId);

                if (model.UsersIdString != null)
                {
                    //list of users id
                    string userIdList = model.UsersIdString.Trim();
                    while (userIdList.Trim().Length > 0)
                    {
                        int index = userIdList.IndexOf(";");
                        if (index != -1)
                        {
                            string newUserId = userIdList.Substring(0, index);

                            repository.AddUserToGroup(newUserId, group.GroupId);

                            userIdList = userIdList.Replace(newUserId + ";", "");
                        }
                    }

                    return RedirectToAction("NewGroupStep3",
                    new { id = model.UserId, groupId = group.GroupId });
                }
                else
                {
                    return RedirectToAction("NewGroupStep3",
                    new { id = model.UserId, groupId = group.GroupId });
                }
            }

            return RedirectToAction("Error", "UserNotFound");
        }

        public IActionResult NewGroupStep3(string id, int groupId)
        {
            ViewData["Id"] = id;

            AppUser user = userManager.Users
                .FirstOrDefault(u => u.Id == id);

            Group group = repository.Groups.FirstOrDefault(g => g.GroupId == groupId);

            if (group.CreatedBy.Id == id)
            {
                return View(new NewGroupStep3ViewModel
                {
                    Group = group,
                    Courses = repository.Courses
                        .Where(c => c.CreatedBy.CompanyName == user.CompanyName
                                    && c.Status == "Public")
                });
            }
            else
            {
                return RedirectToAction("Error", "AccessDenied");
            }
        }

        [HttpPost]
        public async Task<IActionResult> NewGroupStep3(NewGroupStep3ViewModel model)
        {
            ViewData["Id"] = model.UserId;

            AppUser user = await userManager.FindByIdAsync(model.UserId);
            if (user != null)
            {
                //group
                Group group = repository.Groups
                    .FirstOrDefault(g => g.GroupId == model.GroupId);

                if (model.CoursesIdString != null)
                {
                    //list of users id
                    string courseIdList = model.CoursesIdString.Trim();
                    while (courseIdList.Trim().Length > 0)
                    {
                        int index = courseIdList.IndexOf(";");
                        if (index != -1)
                        {
                            string newCourseId = courseIdList.Substring(0, index);

                            int newCourseIdInt = Int32.Parse(newCourseId.Trim());

                            repository.AddCourseToGroup(newCourseIdInt, group.GroupId);

                            courseIdList = courseIdList.Replace(newCourseId + ";", "");
                        }
                    }
                }
                return RedirectToAction("GroupDetails", new
                {
                    id = group.CreatedBy.Id,
                    groupId = group.GroupId
                });
            }

            return RedirectToAction("Error", "UserNotFound");
        }

        public async Task<IActionResult> DeleteGroup (string id, int groupId)
        {
            AppUser user = await userManager.FindByIdAsync(id);

            if (user != null)
            {
                repository.DeleteGroup(groupId);

                return RedirectToAction("Groups", new { id });
            }
            else
            {
                return RedirectToAction("UserNotFound", "Error");
            }
        }

        public async Task<IActionResult> RemoveUserFromGroup (string adminId, int groupId, 
            string userId)
        {
            AppUser user = await userManager.FindByIdAsync(adminId);

            if (user != null)
            {
                repository.RemoveUserFromGroup(groupId, userId);
                return RedirectToAction("GroupDetails", new { id = adminId, groupId });
            }
            else
            {
                return RedirectToAction("UserNotFound", "Error");
            }
        }

        public async Task<IActionResult> RemoveCourseFromGroup(string adminId, int groupId, 
            int courseId)
        {
            AppUser user = await userManager.FindByIdAsync(adminId);

            if (user != null)
            {
                repository.RemoveCourseFromGroup(groupId, courseId);
                return RedirectToAction("GroupDetails", new { id = adminId, groupId });
            }
            else
            {
                return RedirectToAction("UserNotFound", "Error");
            }
        }

        [HttpPost]
        public IActionResult EditGroup (GroupDetailsViewModel model)
        {
            Group group = repository.Groups
                .FirstOrDefault(g => g.GroupId == model.Group.GroupId);

            group.Name = model.Group.Name;
            group.Description = model.Group.Description;
            repository.SaveGroup(group);

            return RedirectToAction("GroupDetails", new
            {
                id = group.CreatedBy.Id,
                groupId = group.GroupId
            });
        }

        [HttpPost]
        public IActionResult AddUsers(NewGroupStep2ViewModel model)
        {
            //group
            Group group = repository.Groups
                .FirstOrDefault(g => g.GroupId == model.GroupId);

            if (model.UsersIdString != null)
            {
                //list of users id
                string userIdList = model.UsersIdString.Trim();
                while (userIdList.Trim().Length > 0)
                {
                    int index = userIdList.IndexOf(";");
                    if (index != -1)
                    {
                        string newUserId = userIdList.Substring(0, index);

                        repository.AddUserToGroup(newUserId, group.GroupId);

                        userIdList = userIdList.Replace(newUserId + ";", "");
                    }
                }
            }

            return RedirectToAction("GroupDetails", new
            {
                id = group.CreatedBy.Id,
                groupId = group.GroupId
            });
        }

        [HttpPost]
        public IActionResult AddCourses(NewGroupStep3ViewModel model)
        {
            //group
            Group group = repository.Groups
                .FirstOrDefault(g => g.GroupId == model.GroupId);

            if (model.CoursesIdString != null)
            {
                //list of users id
                string courseIdList = model.CoursesIdString.Trim();
                while (courseIdList.Trim().Length > 0)
                {
                    int index = courseIdList.IndexOf(";");
                    if (index != -1)
                    {
                        string newCourseId = courseIdList.Substring(0, index);

                        int newCourseIdInt = Int32.Parse(newCourseId.Trim());

                        repository.AddCourseToGroup(newCourseIdInt, group.GroupId);

                        courseIdList = courseIdList.Replace(newCourseId + ";", "");
                    }
                }
            }
            return RedirectToAction("GroupDetails", new
            {
                id = group.CreatedBy.Id,
                groupId = group.GroupId
            });
        }
    }
}

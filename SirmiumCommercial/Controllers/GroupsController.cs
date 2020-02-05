using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models.ViewModels;
using SirmiumCommercial.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace SirmiumCommercial.Controllers
{
    public class GroupsController : Controller
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;
        private IHostingEnvironment hostingEnvironment;

        public GroupsController (IAppDataRepository repo, UserManager<AppUser> userMgr,
                 IHostingEnvironment hosting)
        {
            repository = repo;
            userManager = userMgr;
            hostingEnvironment = hosting;
        }

        public async Task<ViewResult> Groups(string id, string status = "Public")
        {
            ViewData["Id"] = id;
            ViewData["GroupStatus"] = status;

            AppUser user = await userManager.FindByIdAsync(id);

            if(status == "Private")
            {
                return View(repository.Groups.Where(g => g.CompanyName == user.CompanyName
                    && g.CreatedBy.Id == user.Id && g.Status == "Private" ));
            }
            else if(status == "All")
            {
                return View(repository.Groups.Where(g => 
                    (g.CompanyName == user.CompanyName && g.Status == "Public") ||
                    (g.CreatedBy.Id == user.Id && g.Status == "Private")));
            }
            else
            {
                return View(repository.Groups.Where(g => g.CompanyName == user.CompanyName
                     && g.Status == "Public"));
            }
        }

        public IActionResult GroupDetails(string id, int groupId)
        {
            ViewData["Id"] = id;

            Group group = repository.Groups
                .FirstOrDefault(g => g.GroupId == groupId);

            if(group != null)
            {
                //NoPermission
                if(repository.GroupUsers.Any(g => g.GroupId == groupId && g.AppUserId == id) == false)
                {
                    return RedirectToAction("GroupNoPermission", new { id, groupId });
                }

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
                return RedirectToAction("GroupNoPermission", new { id, groupId });
            }
        }

        public ViewResult GroupNoPermission(string id, int groupId)
        {
            ViewData["Id"] = id;

            Group group = repository.Groups.FirstOrDefault(g => g.GroupId == groupId);
            string msg = "";

            if (group != null)
            {
                msg = "You do not have permission to access this group!";
            }
            else
            {
                msg = "Sorry, this group not exist!";
            }

            ViewData["GroupError"] = msg;

            return View("GroupNoPermission");
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
                string status = (model.Status == true) ? "Public" : "Private";

                Group newGroup = new Group
                {
                    Name = model.Name,
                    Description = model.Description,
                    CreatedBy = user,
                    CompanyName = user.CompanyName,
                    Status = status
                };

                int groupId = repository.SaveGroup(newGroup);
                repository.AddUserToGroup(user.Id, groupId);

                //if photo exists
                if (model.ProfilePhoto != null)
                {
                    AddGroupPhoto(groupId, model.ProfilePhoto);
                }

                //create chat
                if (model.CreateChat == true)
                {
                    GroupChat groupChat = new GroupChat
                    {
                        Title = model.Name,
                        Status = status,
                        CreatedBy = user.Id,
                        ChatPhotoPath = "Groups/" + repository.Groups
                            .FirstOrDefault(g => g.GroupId == newGroup.GroupId).GroupPhotoPath
                    };
                    repository.EditGroupChat(groupChat);

                    //add admin to chat
                    repository.AddUserToGroupChat(groupChat.ChatId, user.Id);

                    newGroup.GroupChatId = groupChat.ChatId;
                    repository.SaveGroup(newGroup);
                }

                return RedirectToAction("NewGroupStep2", new { id = model.CreatedById, groupId });
            }

            return RedirectToAction("Error", "UserNotFound");
        }

        private void AddGroupPhoto(int groupId, IFormFile photo)
        {
            Group group = repository.Groups
                .FirstOrDefault(g => g.GroupId == groupId);

            //Create user directory
            string dirPath = Path.Combine(hostingEnvironment.WebRootPath, $@"UsersData\Groups\{group.GroupId}");
            System.IO.Directory.CreateDirectory(dirPath);

            string fileName = "profilePhoto.jpeg";
            string filePath = Path.Combine(dirPath, fileName);


            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                photo.CopyTo(stream);
            }

            //add photo path to Group/GroupPhotoPath
            string photoPath = group.GroupId + @"\profilePhoto.jpeg";
            group.GroupPhotoPath = photoPath;
            repository.SaveGroup(group);
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

                            //group chat exists
                            if(group.GroupChatId != 0)
                            {
                                repository.AddUserToGroupChat(group.GroupChatId, newUserId);
                            }

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

                //send msg 
                if(group.GroupChatId != 0)
                {
                    GroupChat groupChat = repository.GroupChats
                        .FirstOrDefault(g => g.ChatId == group.GroupChatId);

                    string userName = (user.FirstName == null && user.LastName == null) ?
                        user.UserName : user.FirstName + user.LastName;
                    string content = $"{userName} created a group chat";
                    GroupChatMessage newMsg = new GroupChatMessage
                    {
                        GroupChatId = group.GroupChatId,
                        UserId = user.Id,
                        MessageContent = content,
                        MessageType = "SystemMsg"
                    };

                    repository.NewGroupChatMessage(newMsg, groupChat);
                }

                //new notification
                if(group.Status == "Public")
                {
                    repository.NewNotification(user.Id, "AddUserToGroup", "Group", group.GroupId);
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
            Group group = repository.Groups.FirstOrDefault(g => g.GroupId == groupId);

            if (user != null)
            {
                //delete group chat if exists
                if(group.GroupChatId != 0)
                {
                    repository.DeleteGroupChat(group.GroupChatId);
                }

                //delete notifications
                foreach (Notification notification in repository.Notifications
                    .Where(n => n.For == "Group" && n.ForId == groupId))
                {
                    repository.DeleteNotification(notification.NotificationId);
                }

                //delete photo and group directory
                if(group.GroupPhotoPath != null)
                {
                    string dirPath = Path.Combine(hostingEnvironment.WebRootPath, $@"UsersData\Groups\{group.GroupId}");
                    System.IO.Directory.Delete(dirPath, true);
                }

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
            Group group = repository.Groups.FirstOrDefault(g => g.GroupId == groupId); 

            if (user != null)
            {
                repository.RemoveUserFromGroup(groupId, userId);

                //remove user from chat
                if(group.GroupChatId != 0)
                {
                    if(repository.GroupChatUsers
                        .FirstOrDefault(g => g.GroupChatId == group.GroupChatId && g.UserId == userId) != null)
                    {
                        repository.RemoveUserFromGroupChat(group.GroupChatId, userId);
                    }
                }

                //remove notification for user
                Notification notification = repository.Notifications
                    .FirstOrDefault(n => n.For == "Group" && n.ForId == groupId
                                    && n.Subject == "NewGroupUser" && n.ForUserId == userId);
                if(notification != null)
                {
                    repository.DeleteNotification(notification.NotificationId);
                }

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
            //Created by
            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == group.CreatedBy.Id);

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
                        //notification
                        if (group.Status == "Public")
                        {
                            repository.NewNotification(group.CreatedBy.Id, "AddUserToGroup", "Group", group.GroupId, newUserId);
                        }
                        //group chat exists
                        if (group.GroupChatId != 0)
                        {
                            repository.AddUserToGroupChat(group.GroupChatId, newUserId);

                            GroupChat groupChat = repository.GroupChats
                                .FirstOrDefault(g => g.ChatId == group.GroupChatId);
                            AppUser addedUser = userManager.Users.FirstOrDefault(u => u.Id == newUserId);

                            string userName = (user.FirstName == null && user.LastName == null) ?
                                user.UserName : user.FirstName + user.LastName;
                            string content = $"{userName} added {addedUser}";
                            GroupChatMessage newMsg = new GroupChatMessage
                            {
                                GroupChatId = group.GroupChatId,
                                UserId = user.Id,
                                MessageContent = content,
                                MessageType = "SystemMsg"
                            };

                            repository.NewGroupChatMessage(newMsg, groupChat);
                        }

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

        [HttpPost]
        public IActionResult NewGroupPhoto(GroupUploadPhotoViewModel model)
        {
            Group group = repository.Groups
                .FirstOrDefault(g => g.GroupId == model.GroupId);

            if (model.ProfilePhoto != null)
            {
                AddGroupPhoto(model.GroupId, model.ProfilePhoto);
            }

            return RedirectToAction("GroupDetails", new
            {
                id = group.CreatedBy.Id,
                groupId = group.GroupId
            });
        }

        public IActionResult AbortNewGroup(string userId, int groupId)
        {
            Group group = repository.Groups.FirstOrDefault(g => g.GroupId == groupId);

            //delete chat if exists
            if(group.GroupChatId != 0)
            {
                repository.DeleteGroupChat(group.GroupChatId);
            }

            repository.DeleteGroup(groupId);

            return RedirectToAction("Groups", new { id = userId });
        }

        public IActionResult GroupNewChat (string id, int groupId)
        {
            Group group = repository.Groups.FirstOrDefault(g => g.GroupId == groupId);
            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == id);
            int chatId = group.GroupChatId;

            if (group.GroupChatId == 0)
            {
                GroupChat groupChat = new GroupChat
                {
                    Title = group.Name,
                    Status = group.Status,
                    CreatedBy = user.Id,
                    ChatPhotoPath = "Groups/" + group.GroupPhotoPath
                };
                repository.EditGroupChat(groupChat);

                chatId = groupChat.ChatId;

                //add users to chat
                repository.GroupAddUserToChat(chatId, groupId);

                group.GroupChatId = groupChat.ChatId;
                repository.SaveGroup(group);

                //new msg
                GroupChat chat = repository.GroupChats
                        .FirstOrDefault(g => g.ChatId == group.GroupChatId);

                string userName = (user.FirstName == null && user.LastName == null) ?
                    user.UserName : user.FirstName + user.LastName;
                string content = $"{userName} created a group chat";
                GroupChatMessage newMsg = new GroupChatMessage
                {
                    GroupChatId = chatId,
                    UserId = user.Id,
                    MessageContent = content,
                    MessageType = "SystemMsg"
                };

                repository.NewGroupChatMessage(newMsg, chat);

            }

            return RedirectToAction("GroupChat", "Chat", new { id, groupChatId = chatId });
        }

        public IActionResult GroupChangeStatus (string id, int groupId)
        {
            Group group = repository.Groups.FirstOrDefault(g => g.GroupId == groupId);
            string newStatus = "Private";

            if(group.Status == "Private")
            {
                newStatus = "Public";

                //add notification if not exists
                if(repository.Notifications.Any(n => n.For == "Group" && n.ForId == groupId
                    && n.Subject == "AddUserToGroup") == false)
                {
                    repository.NewNotification(id, "AddUserToGroup", "Group", group.GroupId);
                }
            }

            group.Status = newStatus;
            repository.SaveGroup(group);

            if(group.GroupChatId != 0)
            {
                GroupChat chat = repository.GroupChats.FirstOrDefault(c => c.ChatId == group.GroupChatId);
                chat.Status = newStatus;
                repository.EditGroupChat(chat);
            }

            return RedirectToAction("GroupDetails", new { id, groupId });
        }
    }
}

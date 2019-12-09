using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SirmiumCommercial.Models;
using System.Threading.Tasks;
using SirmiumCommercial.Models.ViewModels;
using SirmiumCommercial.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Mail;
using System.Net;

namespace SirmiumCommercial.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private UserManager<AppUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public AdminController(UserManager<AppUser> userMgr,
            RoleManager<IdentityRole> roleMgr)
        {
            userManager = userMgr;
            roleManager = roleMgr;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string id)
        {
            ViewData["Id"] = id;
            AppUser admin = await userManager.FindByIdAsync(id);
            if (admin != null)
            {
                ViewData["AdminCompany"] = admin.CompanyName;
                if (admin.CompanyName == null)
                {
                    return RedirectToAction("HeadAdminIndex", new { id });
                }
                else
                {
                    return RedirectToAction("Index2", new { id });
                }
            }
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<ViewResult> HeadAdminIndex(string id, string sort, string order)
        {
            ViewData["Id"] = id;
            ViewData["Sort"] = (sort == null) ? "RegistrationDate" : sort;
            ViewData["Order"] = (order == null) ? "desc" : order;

            AppUser admin = await userManager.FindByIdAsync(id);

            switch (sort)
            {
                case "Username":
                    return View(order == "asc" ?
                        userManager.Users.OrderBy(u => u.UserName) :
                        userManager.Users.OrderByDescending(u => u.UserName));
                case "FirstName":
                    return View(order == "asc" ?
                        userManager.Users.OrderBy(u => u.FirstName) :
                        userManager.Users.OrderByDescending(u => u.FirstName));
                case "LastName":
                    return View(order == "asc" ?
                        userManager.Users.OrderBy(u => u.LastName) :
                        userManager.Users.OrderByDescending(u => u.LastName));
                case "Company":
                    return View(order == "asc" ?
                        userManager.Users.OrderBy(u => u.CompanyName) :
                        userManager.Users.OrderByDescending(u => u.CompanyName));
                case "Status":
                    return View(order == "asc" ?
                        userManager.Users.OrderBy(u => u.Status) :
                        userManager.Users.OrderByDescending(u => u.Status));
                default:
                    return View(order == "asc" ?
                        userManager.Users.OrderBy(u => u.RegistrationDate) :
                        userManager.Users.OrderByDescending(u => u.RegistrationDate));
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<ViewResult> Index2(string id, string sort, string order)
        {
            ViewData["Id"] = id;
            ViewData["Sort"] = (sort == null) ? "RegistrationDate" : sort;
            ViewData["Order"] = (order == null) ? "desc" : order;

            AppUser admin = await userManager.FindByIdAsync(id);

            switch(sort){
                case "Username":
                    return View(order == "asc" ?
                        userManager.Users.Where(u => u.CompanyName == admin.CompanyName).OrderBy(u => u.UserName) :
                        userManager.Users.Where(u => u.CompanyName == admin.CompanyName).OrderByDescending(u => u.UserName));
                case "FirstName":
                    return View(order == "asc" ?
                        userManager.Users.Where(u => u.CompanyName == admin.CompanyName).OrderBy(u => u.FirstName) :
                        userManager.Users.Where(u => u.CompanyName == admin.CompanyName).OrderByDescending(u => u.FirstName));
                case "LastName":
                    return View(order == "asc" ?
                        userManager.Users.Where(u => u.CompanyName == admin.CompanyName).OrderBy(u => u.LastName) :
                        userManager.Users.Where(u => u.CompanyName == admin.CompanyName).OrderByDescending(u => u.LastName));
                case "Status":
                    return View(order == "asc" ?
                        userManager.Users.Where(u => u.CompanyName == admin.CompanyName).OrderBy(u => u.Status) :
                        userManager.Users.Where(u => u.CompanyName == admin.CompanyName).OrderByDescending(u => u.Status));
                default:
                    return View(order == "asc" ?
                        userManager.Users.Where(u => u.CompanyName == admin.CompanyName).OrderBy(u => u.RegistrationDate) :
                        userManager.Users.Where(u => u.CompanyName == admin.CompanyName).OrderByDescending(u => u.RegistrationDate));
            }
        }

        [Authorize(Roles = "Admin")]
        public ViewResult Create(string id)
        {
            ViewData["Id"] = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(string id, CreateModel model)
        {
            ViewData["Id"] = id;
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    CompanyName = model.CompanyName,
                    PhoneNumber = model.PhoneNumber,
                    RegistrationDate = DateTime.Now,
                    Status = model.Status
                };
                IdentityResult result
                    = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", id);
                }
                else
                {
                    ResultError(result);
                }
            }
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                return View(user);
            }
            else
            {
                return RedirectToAction("Index", id);
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id, string status)
        {
            AppUser user = await userManager.FindByIdAsync(id);
            if(user != null)
            {
                user.Status = status;
                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", id);
                }
                else
                {
                    ResultError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View(user);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id, string deleteId)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(deleteId);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", id);
                }
                else
                {
                    ResultError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Index", userManager.Users);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatust(string id, string changeId)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(changeId);
            if (user != null)
            {
                switch (user.Status)
                {
                    case "Active":
                        user.Status = "Inactive";
                        break;
                    case "Inactive":
                        user.Status = "Active";
                        SendMail(user.Id);
                        break;
                    default:
                        user.Status = "Active";
                        SendMail(user.Id);
                        break;
                }
                
                IdentityResult result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Details", new { id = id, detailsId = changeId });
                }
                else
                {
                    ResultError(result);
                }
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
            }
            return View("Details", new { id = id, detailsId = changeId });
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Roles(string id)
        {
            ViewData["Id"] = id;
            AppUser admin = await userManager.FindByIdAsync(id);
            if(admin != null)
            {
                if (HttpContext.User.Identity.Name == admin.UserName)
                {
                    if (admin.CompanyName == null)
                    {
                        return View(roleManager.Roles);
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Account", new { returnUrl = "Login" });
                }
            }
            return RedirectToAction("Index", new { id });
        }

        [Authorize(Roles = "Admin")]
        public IActionResult CreateRole(string id)
        {
            ViewData["Id"] = id;
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateRole(string id, [Required]string name)
        {
            ViewData["Id"] = id;
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles", id);
                }
                else
                {
                    ResultError(result);
                }
            }
            return View(name);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(string id, string detailsId)
        {
            ViewData["Id"] = id;
            AppUser user = await userManager.FindByIdAsync(detailsId);
            if (user != null)
            {
                return View(new AdminViewModel
                {
                    User = user,
                    Roles = roleManager.Roles
                });
            }
            else
            {
                return RedirectToAction("Index", id);
            }

        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditRole(AdminViewModel admin)
        {
            AppUser user = await userManager.FindByIdAsync(admin.User.Id);
            IdentityRole role = await roleManager.FindByIdAsync(admin.Role.Id);
            if (user != null)
            {
                if (role != null)
                {
                    IdentityResult result = await userManager.AddToRoleAsync(user, role.Name);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Details");
                    }
                    else
                    {
                        ResultError(result);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "User Not Found");
                }
            }
            else
            {
                ModelState.AddModelError("", "Role Not Found");
            }
            return View("Details");
        }

        [Authorize(Roles = "Admin")]
        public ViewResult InviteByEmail(string id)
        {
            ViewData["Id"] = id;

            return View(new InviteByEmailViewModel { });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult InviteByEmail(InviteByEmailViewModel model)
        {
            if (model.email1 != null && model.email1.Trim().Length > 0)
            {
                SendMailInvite(model.adminId, model.email1);
            }
            if (model.email2 != null && model.email2.Trim().Length > 0)
            {
                SendMailInvite(model.adminId, model.email2);
            }
            if (model.email3 != null && model.email3.Trim().Length > 0)
            {
                SendMailInvite(model.adminId, model.email3);
            }
            if (model.email4 != null && model.email4.Trim().Length > 0)
            {
                SendMailInvite(model.adminId, model.email4);
            }
            if (model.email5 != null && model.email5.Trim().Length > 0)
            {
                SendMailInvite(model.adminId, model.email5);
            }

            return RedirectToAction("Index", new { id = model.adminId });
        }

        private void ResultError(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        //send email to user
        private void SendMail(string userId)
        {
            AppUser user = userManager.Users.FirstOrDefault(u => u.Id == userId);

            try
            {
                String msgBody =
                    "<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"color:#333;background:#fff;padding:0;margin:0;width:100%;font:15px 'Helvetica Neue',Arial,Helvetica\">" +
                    "<tbody>" +
                    "<tr width=\"100%\">" +
                    "<td valign=\"top\" align=\"left\" style=\"background:#f0f0f0;font-size:15px; font-family: 'Helvetica Neue',Arial,Helvetica\">" +
                    "<table style=\"border:none;padding:0 18px;margin:50px auto;width:500px\">" +
                    "<tbody>" +
                    "<tr width=\"100%\" height=\"57\">" +
                    "<td valign=\"top\" align=\"left\" style=\"border-top-left-radius:4px;border-top-right-radius:4px;background:#0079bf;padding:10px;text-align:center; font:700; font-size: 26px; font-family: Impact, Haettenschweiler, 'Arial Narrow Bold', sans-serif; color: #fff;\">" +
                    "SirmiumERPCommercial</td></tr>" +
                    "<tr style=\"width:100%\">" +
                    "<td valign=\"top\" align=\"left\" style=\"background:#fff;padding:18px\">" +
                    "<p style=\"color:#333333;font:14px/1.25em 'Helvetica Neue',Arial,Helvetica;font-weight:bold;line-height:20px;text-align:center;padding-left:56px;padding-right:56px\"> Hey there! Welcome to SirmiumERPCommercial </p>" +
                    $"<p style=\"color:#305496;font:14px/1.25em 'Helvetica Neue',Arial,Helvetica;font-weight:bold;line-height:20px;text-align:center;padding-left:56px;padding-right:56px\"> Your account was activated.</p>" +
                    "</td> </tr>" +
                    "<tr style=\"width:100%\">" +
                    "<td valign=\"top\" align=\"left\" style=\"background:#fff;padding:18px\">" +
                    $"<p style=\"font:15px/1.25em 'Helvetica Neue',Arial,Helvetica;margin-bottom:0;text-align:center\"> <a href='https://localhost:5001/' style=\"border-radius:3px;background:#5aac44;color:#fff;display:block;font-weight:600;font-size:20px;line-height:24px;margin:32px auto 24px;padding:11px 13px;text-decoration:none;width:152px\"> Login now </a> </p>" +
                    "</td> </tr> </tbody> </table>" +
                    "</td> </tr> </tbody> </table>";
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("sirmiumcommercial@gmail.com");
                message.To.Add(new MailAddress(user.Email));
                message.Subject = "Account activated";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = msgBody;
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com"; //for gmail host  
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("sirmiumcommercial@gmail.com", "648768422SM");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(message);
            }
            catch (SmtpException ex)
            {
            }
        }

        //invite by email
        private void SendMailInvite(string adminId, string userEmail)
        {
            AppUser admin = userManager.Users.FirstOrDefault(u => u.Id == adminId);

            try
            {
                String msgBody =
                    "<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" style=\"color:#333;background:#fff;padding:0;margin:0;width:100%;font:15px 'Helvetica Neue',Arial,Helvetica\">" +
                    "<tbody>" +
                    "<tr width=\"100%\">" +
                    "<td valign=\"top\" align=\"left\" style=\"background:#f0f0f0;font-size:15px; font-family: 'Helvetica Neue',Arial,Helvetica\">" +
                    "<table style=\"border:none;padding:0 18px;margin:50px auto;width:500px\">" +
                    "<tbody>" +
                    "<tr width=\"100%\" height=\"57\">" +
                    "<td valign=\"top\" align=\"left\" style=\"border-top-left-radius:4px;border-top-right-radius:4px;background:#0079bf;padding:10px;text-align:center; font:700; font-size: 26px; font-family: Impact, Haettenschweiler, 'Arial Narrow Bold', sans-serif; color: #fff;\">" +
                    "SirmiumERPCommercial</td></tr>" +
                    "<tr style=\"width:100%\">" +
                    "<td valign=\"top\" align=\"left\" style=\"background:#fff;padding:18px\">" +
                    "<p style=\"color:#333333;font:14px/1.25em 'Helvetica Neue',Arial,Helvetica;font-weight:bold;line-height:20px;text-align:center;padding-left:56px;padding-right:56px\"> Hey there! </p>" +
                    $"<p style=\"color:#305496;font:14px/1.25em 'Helvetica Neue',Arial,Helvetica;font-weight:bold;line-height:20px;text-align:center;padding-left:56px;padding-right:56px\"> {admin.FirstName} {admin.LastName} invited you to join the SirmiumERPCommercial</p>" +
                    "</td> </tr>" +
                    "<tr style=\"width:100%\">" +
                    "<td valign=\"top\" align=\"left\" style=\"background:#fff;padding:18px\">" +
                    $"<p style=\"font:15px/1.25em 'Helvetica Neue',Arial,Helvetica;margin-bottom:0;text-align:center\"> <a href='https://localhost:5001/Account/SignUp' style=\"border-radius:3px;background:#5aac44;color:#fff;display:block;font-weight:600;font-size:20px;line-height:24px;margin:32px auto 24px;padding:11px 13px;text-decoration:none;width:152px\"> Register now </a> </p>" +
                    "</td> </tr> </tbody> </table>" +
                    "</td> </tr> </tbody> </table>";
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("sirmiumcommercial@gmail.com");
                message.To.Add(new MailAddress(userEmail));
                message.Subject = "Account activated";
                message.IsBodyHtml = true; //to make message body as html  
                message.Body = msgBody;
                SmtpClient client = new SmtpClient();
                client.Port = 587;
                client.Host = "smtp.gmail.com"; //for gmail host  
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("sirmiumcommercial@gmail.com", "648768422SM");
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(message);
            }
            catch (SmtpException ex)
            {
            }
        }
    }
}

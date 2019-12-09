using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SirmiumCommercial.Models.ViewModels;
using SirmiumCommercial.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Session;
using System.Net.Mail;
using System.Net;

namespace SirmiumCommercial.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<AppUser> userMgr,
            SignInManager<AppUser> signinMgr, RoleManager<IdentityRole> roleMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            roleManager = roleMgr;
        }

        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel detalis, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(detalis.UserName);
                if (user != null)
                {
                    if (user.Status == "Active")
                    {
                        await signInManager.SignOutAsync();
                        Microsoft.AspNetCore.Identity.SignInResult result =
                            await signInManager.PasswordSignInAsync(
                                user, detalis.Password, false, false);
                        if (result.Succeeded)
                        {
                            //Redirect to AdminController, if user-role="Admin"
                            if (await userManager.IsInRoleAsync(user, "Admin"))
                            {
                                return RedirectToAction("Index", "Admin", new { id = user.Id });
                            }
                            //Redirect to CompanyAdminController, if user-role="Manager"
                            else if (await userManager.IsInRoleAsync(user, "Manager")) { 
                            
                                return RedirectToAction("Index", "Manage", new { id = user.Id });
                            }
                            //Redirect to CompanyAdminController, if user-role="User"
                            else //if //(await userManager.IsInRoleAsync(user, "User"))
                            {

                                return RedirectToAction("Index", "User", new { id = user.Id });
                            }
                            /*else
                            {
                                return Redirect(returnUrl ?? "/");
                            }*/
                        }
                        else
                        {
                            ModelState.AddModelError(nameof(LoginModel.UserName),
                                "Invalid password!");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(LoginModel.UserName),
                            "The username '" + user.UserName + "' has not been activated or is blocked!");
                        return View(detalis);
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(LoginModel.UserName),
                        "Invalid username!");
                }
            }
            return View(detalis);
        }

        [AllowAnonymous]
        public ViewResult SignUp()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp(CreateModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.RepeatPassword != model.Password)
                {
                    ModelState.AddModelError("IncorrectPassword", "Please, enter your password again.");
                    return View();
                }
                AppUser user = new AppUser
                {
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    CompanyName = model.CompanyName,
                    PhoneNumber = (model.PhoneNumber != "") ? model.PhoneNumber : "",
                    RegistrationDate = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Remark = model.Remark,
                    Status = "Inactive"
                };
                IdentityResult result
                    = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    foreach(AppUser admin in userManager.Users
                        .Where(u => u.CompanyName == model.CompanyName))
                    {
                        if (await userManager.IsInRoleAsync(admin, "Admin"))
                        {
                            SendMail(admin.Id, user.Id);
                        }
                    }
                    /*TODO: add user to userDetails*/
                    return RedirectToAction("SuccessSignUp");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }

        [AllowAnonymous]
        public ViewResult SuccessSignUp()
        {
            return View();
        }

        public async Task<RedirectResult> Logout(string returnUrl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult FreeTrial()
        {
            return View();
        }

        //send email to admin
        private void SendMail(string adminId, string userId)
        {
            AppUser admin = userManager.Users.FirstOrDefault(u => u.Id == adminId);
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
                    "<p style=\"color:#333333;font:14px/1.25em 'Helvetica Neue',Arial,Helvetica;font-weight:bold;line-height:20px;text-align:center;padding-left:56px;padding-right:56px\"> New User! </p>" +
                    $"<p style=\"color:#305496;font:14px/1.25em 'Helvetica Neue',Arial,Helvetica;font-weight:bold;line-height:20px;text-align:center;padding-left:56px;padding-right:56px\"> {user.FirstName} {user.LastName} just created an account! </p>" +
                    "</td> </tr>" +
                    "<tr style=\"width:100%\">" +
                    "<td valign=\"top\" align=\"left\" style=\"background:#fff;padding:18px\">" +
                    $"<p style=\"font:15px/1.25em 'Helvetica Neue',Arial,Helvetica;margin-bottom:0;text-align:center\"> <a href='https://localhost:5001/Admin/Details/3ca5fbe9-61fa-42c5-bfe4-f76d9d28438e?detailsId={user.Id}' style=\"border-radius:3px;background:#5aac44;color:#fff;display:block;font-weight:600;font-size:20px;line-height:24px;margin:32px auto 24px;padding:11px 13px;text-decoration:none;width:152px\"> Go to user details </a> </p>" +
                    "</td> </tr> </tbody> </table>" +
                    "</td> </tr> </tbody> </table>";
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress("sirmiumcommercial@gmail.com");
                message.To.Add(new MailAddress(admin.Email));
                message.Subject = "New user";
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

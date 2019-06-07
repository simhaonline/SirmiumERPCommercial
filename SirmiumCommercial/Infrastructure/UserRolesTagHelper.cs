using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SirmiumCommercial.Models;

namespace SirmiumCommercial.Infrastructure
{
    [HtmlTargetElement("td", Attributes = "user-role")]
    public class UserRolesTagHelper : TagHelper
    {
        private UserManager<AppUser> userManager;
        private RoleManager<IdentityRole> roleManager;

        public UserRolesTagHelper(UserManager<AppUser> userMgr,
            RoleManager<IdentityRole> roleMgr)
        {
            userManager = userMgr;
            roleManager = roleMgr;
        }

        [HtmlAttributeName("user-role")]
        public string User { get; set; }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            List<string> roles = new List<string>();
            AppUser user = await userManager.FindByIdAsync(User);
            if (user != null)
            {
                foreach (var role in roleManager.Roles)
                {
                    if (role != null && await userManager.IsInRoleAsync(user, role.Name))
                    {
                        roles.Add(role.Name);
                    }
                }
            }
            output.Content.SetContent(roles.Count == 0 ?
                "No Roles" : string.Join(", ", roles));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SirmiumCommercial.Models;
using SirmiumCommercial.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace SirmiumCommercial.Components
{
    public class GroupsUploadPhotoViewComponent : ViewComponent
    {
        private IAppDataRepository repository;

        public GroupsUploadPhotoViewComponent(IAppDataRepository repo)
        {
            repository = repo;
        }

        public IViewComponentResult Invoke(int groupId)
        {
            Group group = repository.Groups.FirstOrDefault(g => g.GroupId == groupId);

            return View(new GroupUploadPhotoViewModel
            {
                GroupId = group.GroupId
            });
        }
    }
}

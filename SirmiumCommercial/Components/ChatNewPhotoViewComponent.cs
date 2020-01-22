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
    public class ChatNewPhotoViewComponent : ViewComponent
    {
        private IAppDataRepository repository;
        private UserManager<AppUser> userManager;

        public ChatNewPhotoViewComponent(IAppDataRepository repo, 
            UserManager<AppUser> userMgr)
        {
            repository = repo;
            userManager = userMgr;
        }

        public IViewComponentResult Invoke(int chatId)
        {
            GroupChat chat = repository.GroupChats.FirstOrDefault(c => c.ChatId == chatId);

            return View(new NewGroupChatPhotoViewModel
            {
                CreatedBy = chat.CreatedBy,
                ChatId = chatId
            });
        }
    }
}

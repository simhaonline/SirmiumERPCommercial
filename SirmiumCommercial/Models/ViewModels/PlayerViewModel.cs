using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class PlayerViewModel
    {
        public Video Video { get; set; }
        public IQueryable<Likes> Likes { get; set; }
        public IQueryable<Dislikes> Dislikes { get; set; }
        public IQueryable<Comment> Comments { get; set; }
        public IQueryable<AppUser> Users { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class LikesDislikes
    {
        public string For { get; set; }
        public int ForId { get; set; }
        public string spanId { get; set; }
        public IQueryable<Likes> Likes { get; set; }
        public IQueryable<Dislikes> Dislikes { get; set; }
    }
}

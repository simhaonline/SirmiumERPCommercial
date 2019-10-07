using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class CompanyViewModel
    {
        public AppUser User { get; set; }
        public int Awards { get; set; }
        public int Posts { get; set; }
        public int Views { get; set; }
    }
}

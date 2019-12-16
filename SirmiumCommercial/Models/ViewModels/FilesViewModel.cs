using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models.ViewModels
{
    public class FilesViewModel
    {
        public Course Course { get; set; }
        public IQueryable<PresentationFiles> Files { get; set; }
    }
}

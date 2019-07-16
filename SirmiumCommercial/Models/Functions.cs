using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models
{
    public class Functions
    {
        public int AddedDate(DateTime added)
        {
            return (DateTime.Now - added).Hours;
        }
    }
}

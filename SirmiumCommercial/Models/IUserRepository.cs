using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SirmiumCommercial.Models
{
    public interface IUserRepository
    {
        IQueryable<UserProfile> Users { get; }
        void SaveUser(UserProfile user);
        void SaveProfilePicture(UserProfile user);
    }
}

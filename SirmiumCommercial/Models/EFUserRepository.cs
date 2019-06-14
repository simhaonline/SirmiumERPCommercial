using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SirmiumCommercial.Models
{
    public class EFUserRepository : IUserRepository
    {
        private AppDbContext dbContext;

        public EFUserRepository (AppDbContext dbCtx)
        {
            dbContext = dbCtx;
        }

        public IQueryable<UserProfile> Users => dbContext.Users;

        public void SaveUser(UserProfile user)
        {
            if (user.Id == 0)
            {
                dbContext.Users.Add(user);
            }
            else
            {
                UserProfile dbEntry = dbContext.Users
                .FirstOrDefault(u => u.UserId == user.UserId);
                if (dbEntry != null)
                {
                    dbEntry.UserId = user.UserId;
                    dbEntry.UserName = user.UserName;
                    dbEntry.FirstName = user.FirstName;
                    dbEntry.LastName = user.LastName;
                    dbEntry.CompanyName = user.CompanyName;
                    dbEntry.PhoneNumber = user.PhoneNumber;
                    dbEntry.ProfilePicture = user.ProfilePicture;
                }
            }
            dbContext.SaveChanges();
        }

        public void SaveProfilePicture(UserProfile user)
        {
             UserProfile dbEntry = dbContext.Users
                .FirstOrDefault(u => u.UserId == user.UserId);
             if (dbEntry != null)
             {
                 dbEntry.ProfilePicture = user.ProfilePicture;
             }
            dbContext.SaveChanges();
        }
    }
}

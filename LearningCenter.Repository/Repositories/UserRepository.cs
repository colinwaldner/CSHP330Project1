using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using LearningCenter.Repository.Interfaces;
using LearningCenter.Repository.Models;

namespace LearningCenter.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private IMemoryCache memoryCache;

        public UserRepository(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        public List<UserModel> Users
        {
            get
            {
                List<UserModel> results = new List<UserModel>();

                if (!memoryCache.TryGetValue("Users", out results))
                {
                    results = DatabaseAccessor.Instance.User.Select(s => new UserModel
                    {
                        Id = s.UserId,
                        Email = s.UserEmail,
                        Password = s.UserPassword,
                        IsAdmin = s.UserIsAdmin
                    }).ToList();

                    memoryCache.Set("Users", results,
                        new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromSeconds(30)));
                }

                return (List<UserModel>)memoryCache.Get("Users");
            }
        }

        public UserModel Add(string email, string password, bool is_admin)
        {
            var user = DatabaseAccessor.Instance.User
                .Add(new Database.Models.User 
                { 
                    UserEmail = email,
                    UserPassword = password,
                    UserIsAdmin = is_admin
                });
            if (user == null)
                return null;

            DatabaseAccessor.Instance.SaveChanges();

            if (memoryCache.TryGetValue("Users", out List<UserModel> temp))
            {
                memoryCache.Remove("Users");
            }

            return new UserModel 
            { 
                Id = user.Entity.UserId,
                Email = user.Entity.UserEmail,
                Password = user.Entity.UserPassword,
                IsAdmin = user.Entity.UserIsAdmin,
            };
        }

        public bool Remove(int user_id)
        {
            var user = DatabaseAccessor.Instance.User.FirstOrDefault(x => x.UserId == user_id);

            if (user == null)
                return false;

            DatabaseAccessor.Instance.User.Remove(user);
            DatabaseAccessor.Instance.SaveChanges();

            if (memoryCache.TryGetValue("Users", out List<UserModel> temp))
            {
                memoryCache.Remove("Users");
            }

            return true;
        }

        public UserModel Update(int user_id, string email, string password, bool is_admin)
        {
            var user = DatabaseAccessor.Instance.User.FirstOrDefault(x => x.UserId == user_id);

            if (user == null)
                return null;

            user.UserEmail = email;
            user.UserPassword = password;
            user.UserIsAdmin = is_admin;

            DatabaseAccessor.Instance.SaveChanges();

            if (memoryCache.TryGetValue("Users", out List<UserModel> temp))
            {
                memoryCache.Remove("Users");
            }

            return new UserModel
            {
                Id = user.UserId,
                Email = user.UserEmail,
                Password = user.UserPassword,
                IsAdmin = user.UserIsAdmin,
            };
        }

        public UserModel User(int user_id)
        {
            return DatabaseAccessor.Instance.User
                .Where(x => x.UserId == user_id)
                .Select(s => new UserModel
                {
                    Id = s.UserId,
                    Email = s.UserEmail,
                    Password = s.UserPassword,
                    IsAdmin = s.UserIsAdmin
                }).FirstOrDefault();
        }
    }
}

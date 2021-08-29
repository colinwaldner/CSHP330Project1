using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using LearningCenter.Repository.Interfaces;
using LearningCenter.Repository.Models;

namespace LearningCenter.Repository.Repositories
{
    public class UserClassRepository : IUserClassRepository
    {
        private IMemoryCache memoryCache;

        public UserClassRepository(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }

        public bool Add(int user_id, int class_id)
        {
            var user = DatabaseAccessor.Instance.User.Find(user_id);
            var _class = DatabaseAccessor.Instance.Class.Find(class_id);

            if (user == null || _class == null)
                return false;

            DatabaseAccessor.Instance.UserClass.Add(new Database.Models.UserClass { UserId = user_id, ClassId = class_id });
            DatabaseAccessor.Instance.SaveChanges();

            return true;
        }

        public List<UserClassModel> ClassesForUser(int user_id)
        {
            return DatabaseAccessor.Instance.UserClass.Where(x => x.UserId == user_id)
                .Select(s => new UserClassModel 
                { 
                    User = new UserModel { Id = s.User.UserId, Email = s.User.UserEmail, Password = s.User.UserPassword, IsAdmin = s.User.UserIsAdmin },
                    Class = new ClassModel { Id = s.Class.ClassId, Name = s.Class.ClassName, Description = s.Class.ClassDescription, Price = s.Class.ClassPrice }
                })
                .ToList();
        }

        public bool Remove(int user_id, int class_id)
        {
            var user_class = DatabaseAccessor.Instance.UserClass.FirstOrDefault(x => x.UserId == user_id && x.ClassId == class_id);

            if (user_class == null)
                return false;

            DatabaseAccessor.Instance.UserClass.Remove(user_class);
            DatabaseAccessor.Instance.SaveChanges();

            return true;
        }

        public bool RemoveAllByClass(int class_id)
        {
            var rem_classes = DatabaseAccessor.Instance.UserClass.Where(x => x.ClassId == class_id).ToList();

            if (rem_classes == null)
                return false;

            DatabaseAccessor.Instance.UserClass.RemoveRange(rem_classes);
            DatabaseAccessor.Instance.SaveChanges();

            return true;
        }

        public bool RemoveAllByUser(int user_id)
        {
            var rem_classes = DatabaseAccessor.Instance.UserClass.Where(x => x.UserId == user_id).ToList();

            if (rem_classes == null)
                return false;

            DatabaseAccessor.Instance.UserClass.RemoveRange(rem_classes);
            DatabaseAccessor.Instance.SaveChanges();

            return true;
        }

        public List<UserClassModel> UsersInClass(int class_id)
        {
            return DatabaseAccessor.Instance.UserClass.Where(x => x.ClassId == class_id)
                .Select(s => new UserClassModel
                {
                    User = new UserModel { Id = s.User.UserId, Email = s.User.UserEmail, Password = s.User.UserPassword, IsAdmin = s.User.UserIsAdmin },
                    Class = new ClassModel { Id = s.Class.ClassId, Name = s.Class.ClassName, Description = s.Class.ClassDescription, Price = s.Class.ClassPrice }
                })
                .ToList();
        }
    }
}

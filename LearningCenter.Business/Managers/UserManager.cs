using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LearningCenter.Repository.Interfaces;
using LearningCenter.Business.Interfaces;
using LearningCenter.Business.Models;

namespace LearningCenter.Business.Managers
{
    public class UserManager : IUserManager
    {
        private readonly IClassRepository _classRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserClassRepository _userClassRepository;

        public UserManager(IClassRepository classRepository, IUserRepository userRepository, IUserClassRepository userClassRepository)
        {
            _classRepository = classRepository;
            _userRepository = userRepository;
            _userClassRepository = userClassRepository;
        }

        public List<UserModel> Users
        {
            get
            {
                return _userRepository.Users.Select(s => new UserModel(
                    s.Id, 
                    s.Email, 
                    s.Password, 
                    s.IsAdmin,
                    _userClassRepository.ClassesForUser(s.Id)
                        .Select(c => new ClassModel(c.Class.Id, c.Class.Name, c.Class.Description, c.Class.Price, null))
                        .ToList()
                    )).ToList();
            }
        }

        public bool Enroll(int user_id, int class_id)
        {
            var user = _userRepository.User(user_id);
            var _class = _classRepository.Class(class_id);

            if (user == null || _class == null)
                return false;

            bool alreadyEnrolled = _userClassRepository.ClassesForUser(user_id).Any(x => x.Class.Id == class_id);
            if (alreadyEnrolled)
                return false;

            return _userClassRepository.Add(user_id, class_id);
        }

        public bool ChangePassword(int user_id, string password)
        {
            var user = _userRepository.User(user_id);

            if (user == null)
                return false;

            if (user.Password.Equals(password))
                return false;

            var update_user = _userRepository.Update(user.Id, user.Email, user.Password, user.IsAdmin);
            if (update_user == null)
                return false;

            return true;
        }

        public List<ClassModel> EnrolledClasses(int user_id)
        {
            return _userClassRepository.ClassesForUser(user_id)
                .Select(s => new ClassModel(s.Class.Id, s.Class.Name, s.Class.Description, s.Class.Price, null))
                .ToList();
        }

        public UserModel Register(string email, string password, bool is_admin)
        {
            var new_user = _userRepository.Add(email.ToLower(), password, is_admin);

            if (new_user == null)
                return null;

            return new UserModel(
                new_user.Id,
                new_user.Email,
                new_user.Password,
                new_user.IsAdmin,
                null
                );
        }

        public bool Remove(int user_id)
        {
            var user = _userRepository.User(user_id);

            if (user == null)
                return false;

            _userClassRepository.RemoveAllByUser(user_id);
            _userRepository.Remove(user_id);

            return true;
        }

        public UserModel Login(string email, string password)
        {
            var user = _userRepository.Users.FirstOrDefault(x => x.Email.Equals(email.ToLower()) && x.Password.Equals(password));

            if (user == null)
                return null;

            return new UserModel(user.Id, user.Email, user.Password, user.IsAdmin, EnrolledClasses(user.Id));
        }

        public bool UpdateAdmin(int user_id, bool is_admin)
        {
            var user = _userRepository.User(user_id);

            if (user == null)
                return false;

            var update_user = _userRepository.Update(user.Id, user.Email, user.Password, is_admin);
            if (update_user == null)
                return false;

            return true;
        }

        public UserModel User(int user_id)
        {
            var user =  _userRepository.User(user_id);
            if (user == null)
                return null;

            return new UserModel(user.Id, user.Email, user.Password, user.IsAdmin, EnrolledClasses(user_id));
        }
    }
}

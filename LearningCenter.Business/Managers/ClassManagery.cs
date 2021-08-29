using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LearningCenter.Repository.Interfaces;
using LearningCenter.Business.Interfaces;
using LearningCenter.Business.Models;

namespace LearningCenter.Business.Managers
{
    public class ClassManagery : IClassManager
    {
        private readonly IClassRepository _classRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserClassRepository _userClassRepository;

        public ClassManagery(IClassRepository classRepository, IUserRepository userRepository, IUserClassRepository userClassRepository)
        {
            _classRepository = classRepository;
            _userRepository = userRepository;
            _userClassRepository = userClassRepository;
        }

        public List<ClassModel> Classes
        {
            get
            {
                return _classRepository.Classes.Select(s => new ClassModel(
                    s.Id,
                    s.Name,
                    s.Description,
                    s.Price,
                    _userClassRepository.UsersInClass(s.Id)
                        .Select(u => new UserModel(u.User.Id, u.User.Email, u.User.Password, u.User.IsAdmin, null))
                        .ToList()
                    )).ToList();
            }
        }

        public ClassModel Add(string name, string description, decimal price)
        {
            var new_class = _classRepository.Add(name, description, price);

            if (new_class == null)
                return null;

            return new ClassModel(
                new_class.Id,
                new_class.Name,
                new_class.Description,
                new_class.Price,
                null
                );
        }

        public bool ChangeName(int class_id, string name, string description)
        {
            var _class = _classRepository.Class(class_id);

            if (_class == null)
                return false;

            var update_class = _classRepository.Update(class_id, name, description, _class.Price);
            if (update_class == null)
                return false;

            return true;
        }

        public bool ChangePrice(int class_id, decimal price)
        {
            var _class = _classRepository.Class(class_id);

            if (_class == null)
                return false;

            var update_class = _classRepository.Update(class_id, _class.Name, _class.Description, price);
            if (update_class == null)
                return false;

            return true;
        }

        public ClassModel Class(int class_id)
        {
            var _class = _classRepository.Class(class_id);

            if (_class == null)
                return null;

            return new ClassModel(_class.Id, _class.Name, _class.Description, _class.Price, Users(class_id));
        }

        public bool Remove(int class_id)
        {
            var _class = _classRepository.Class(class_id);

            if (_class == null)
                return false;

            _userClassRepository.RemoveAllByClass(class_id);
            _classRepository.Remove(class_id);

            return true;
        }

        public List<UserModel> Users(int class_id)
        {
            return _userClassRepository.UsersInClass(class_id)
                .Select(s => new UserModel(s.User.Id, s.User.Email, s.User.Password, s.User.IsAdmin, null))
                .ToList();
        }
    }
}

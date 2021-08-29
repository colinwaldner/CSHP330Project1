using System;
using System.Collections.Generic;
using System.Text;

namespace LearningCenter.Business.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

        public List<ClassModel> Classes { get; set; }

        public UserModel(int id, string email, string password, bool isadmin, List<ClassModel> classes)
        {
            Id = id;
            Email = email;
            Password = password;
            IsAdmin = isadmin;
            Classes = classes;
        }
    }
}

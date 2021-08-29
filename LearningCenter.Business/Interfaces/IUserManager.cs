using System;
using System.Collections.Generic;
using System.Text;
using LearningCenter.Business.Models;

namespace LearningCenter.Business.Interfaces
{
    public interface IUserManager
    {
        List<UserModel> Users { get; }
        List<ClassModel> EnrolledClasses(int user_id);
        UserModel User(int user_id);
        UserModel Register(string email, string password, bool is_admin);
        UserModel Login(string email, string password);
        bool Enroll(int user_id, int class_id);
        bool UpdateAdmin(int user_id, bool is_admin);
        bool ChangePassword(int user_id, string password);
        bool Remove(int user_id);
    }
}

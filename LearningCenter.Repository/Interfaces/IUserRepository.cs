using System;
using System.Collections.Generic;
using System.Text;
using LearningCenter.Repository.Models;

namespace LearningCenter.Repository.Interfaces
{
    public interface IUserRepository
    {
        List<UserModel> Users { get; }
        UserModel User(int user_id);
        UserModel Add(string email, string password, bool is_admin);
        UserModel Update(int user_id, string email, string password, bool is_admin);
        bool Remove(int user_id);
    }
}

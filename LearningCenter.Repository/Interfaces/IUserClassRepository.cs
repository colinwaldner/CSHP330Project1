using System;
using System.Collections.Generic;
using System.Text;
using LearningCenter.Repository.Models;

namespace LearningCenter.Repository.Interfaces
{
    public interface IUserClassRepository
    {
        List<UserClassModel> ClassesForUser(int user_id);
        List<UserClassModel> UsersInClass(int class_id);
        bool Add(int user_id, int class_id);                 
        bool Remove(int user_id, int class_id);
        bool RemoveAllByUser(int user_id);
        bool RemoveAllByClass(int class_id);
    }
}
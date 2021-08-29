using System;
using System.Collections.Generic;
using System.Text;
using LearningCenter.Business.Models;

namespace LearningCenter.Business.Interfaces
{
    public interface IClassManager
    {
        List<ClassModel> Classes { get; }
        List<UserModel> Users(int class_id);
        ClassModel Class(int class_id);
        ClassModel Add(string name, string description, decimal price);
        bool ChangePrice(int class_id, decimal price);
        bool ChangeName(int class_id, string name, string description);
        bool Remove(int class_id);
    }
}

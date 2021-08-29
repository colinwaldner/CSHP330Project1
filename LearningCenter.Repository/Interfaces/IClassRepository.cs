using System;
using System.Collections.Generic;
using System.Text;
using LearningCenter.Repository.Models;

namespace LearningCenter.Repository.Interfaces
{
    public interface IClassRepository
    {
        List<ClassModel> Classes { get; }
        ClassModel Class(int class_id);
        ClassModel Add(string name, string description, decimal price);
        ClassModel Update(int class_id, string name, string description, decimal price);
        bool Remove(int class_id);
    }
}

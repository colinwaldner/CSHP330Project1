using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LearningCenter.Business.Models
{
    public class ClassModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public List<UserModel> Users { get; set; }

        public ClassModel(int id, string name, string description, decimal price, List<UserModel> users)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Users = users;
        }
    }
}

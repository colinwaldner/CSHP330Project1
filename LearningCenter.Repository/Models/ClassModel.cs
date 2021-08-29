using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LearningCenter.Repository.Models
{
    public class ClassModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}

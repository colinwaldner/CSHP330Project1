using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningCenter.Website.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }

        public List<ClassViewModel> Classes { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace LearningCenter.Repository.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
    }
}

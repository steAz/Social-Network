using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace serwisSpolecznosciowy.Models
{
    public class User
    {
        public string Login { get; set; }
        public List<string> Friends { get; set; }
        public DateTime DateOfCreation { get; set; }

        public User()
        {

        }

        public User(string login, List<string> friends, DateTime dateOfCreation)
        {
            Login = login;
            Friends = friends;
            DateOfCreation = dateOfCreation;
        }
    }
}
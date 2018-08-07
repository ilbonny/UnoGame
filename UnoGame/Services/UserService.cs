using System;
using System.Collections.Generic;
using UnoGame.Models;

namespace UnoGame.Services
{
    public interface IUserService
    {
        List<User> Users { get; set; }
        void Add(User  user);
    }

    public class UserService : IUserService
    {
        public List<User> Users { get; set; }

        public UserService()
        {
            Users = new List<User>();
        }

        public void Add(User  user)
        {
            user.Id = Guid.NewGuid();
            Users.Add(user);
        }
    }
}

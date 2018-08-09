using System;
using System.Collections.Generic;
using UnoGame.Core.Models;

namespace UnoGame.Core.Services
{
    public interface IUserService
    {
        List<User> Users { get; set; }
        User Add(User  user);
    }

    public class UserService : IUserService
    {
        public List<User> Users { get; set; }

        public UserService()
        {
            Users = new List<User>();
        }

        public User Add(User user)
        {
            user.Id = Guid.NewGuid();
            Users.Add(user);
            return user;
        }
    }
}

using System.Collections.Generic;
using UnoGame.Models;

namespace UnoGame.Services
{
    public interface IPlayerService
    {
        List<Player> Create(List<User> users);
    }

    public class PlayerService : IPlayerService
    {
        public List<Player> Create(List<User> users)
        {
            var players = new List<Player>();

            for (var i = 1; i <= users.Count; i++)
                players.Add(new Player { User = users[i-1], Position = i});

            return players;
        }
    }
}

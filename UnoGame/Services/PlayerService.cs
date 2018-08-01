using System.Collections.Generic;
using UnoGame.Models;

namespace UnoGame.Services
{
    public interface IPlayerService
    {
        List<Player> Create(int numPlayer);
    }

    public class PlayerService : IPlayerService
    {
        public List<Player> Create(int numPlayer)
        {
            var players = new List<Player>();

            for (var i = 1; i <= numPlayer; i++)
                players.Add(new Player {Position = i});

            return players;
        }
    }
}

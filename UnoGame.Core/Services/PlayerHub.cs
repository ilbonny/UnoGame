using System.Linq;
using Microsoft.AspNet.SignalR;

namespace UnoGame.Core.Services
{
    public interface IPlayerHub
    {
        void ReloadUsers();
    }

    public class PlayerHub : Hub, IPlayerHub
    {
        private readonly IUserService _userService;
        private readonly IGameService _gameService;
        private const int NumPlayer = 4;
        
        public PlayerHub(IUserService userService, IGameService gameService)
        {
            _userService = userService;
            _gameService = gameService;
        }

        public void ReloadUsers()
        {
            Clients.All.updateUsers(_userService.Users);
        }

        public void StartGame()
        {
            if (_userService.Users.Count < NumPlayer) return;

            var fourUsers = _userService.Users.Take(NumPlayer).ToList();

            var game = _gameService.Start(fourUsers);

            foreach (var user in fourUsers)
                Clients.Client(user.ConnectionHubId).startGame(game.Id);
        }
    }
}
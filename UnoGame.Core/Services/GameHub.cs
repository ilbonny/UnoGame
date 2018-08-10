using System;
using System.Linq;
using Microsoft.AspNet.SignalR;

namespace UnoGame.Core.Services
{
    public interface IGameHub
    {
        void StartGame(Guid gameId, Guid userId, string connectionHubId);
        void ReloadGame(Guid gameId);
    }

    public class GameHub : Hub, IGameHub
    {
        private readonly IGameService _gameService;

        public GameHub(IGameService gameService)
        {
            _gameService = gameService;
        }

        public void StartGame(Guid gameId, Guid userId, string connectionHubId)
        {
            var game = _gameService.Games.FirstOrDefault(x => x.Id == gameId);
            var player = game?.Players.FirstOrDefault(x => x.User.Id == userId);

            if (player == null) return;
            player.User.ConnectionHubId = connectionHubId;

            var gameForUser = _gameService.GetGame(gameId, userId);
            Clients.Client(connectionHubId).reloadGame(gameForUser);
        }

        public void ReloadGame(Guid gameId)
        {
            var game = _gameService.Games.FirstOrDefault(x => x.Id == gameId);
            if (game == null) return;

            if (game.PlayerWin != null)
                game.Message = MessageService.Show(MessageService.PlayerWin, game.PlayerWin.User.UserName);

            foreach (var player in game.Players.Where(c=>!c.User.IsAutomatic))
            {
                var gameForUser = _gameService.GetGame(gameId, player.User.Id);
                Clients.Client(player.User.ConnectionHubId).reloadGame(gameForUser);
            }

            if (game.PlayerWin != null) return;

            var isAutomatic = _gameService.CheckAutomaticPlayer(game);
            if(isAutomatic) ReloadGame(gameId);
        }
    }
}
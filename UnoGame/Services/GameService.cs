using System.Linq;
using UnoGame.Models;

namespace UnoGame.Services
{
    public interface IGameService
    {
        Game Start();
        void PlayerTurnExecute(PlayerTurn playerTurn);
    }

    public class GameService : IGameService
    {
        private const int NumPlayer = 4;
        private const int NumCard = 7;

        private readonly ICardDeskService _cardDeskService;
        private readonly IPlayerService _playerService;

        public GameService(ICardDeskService cardDeskService, IPlayerService playerService)
        {
            _cardDeskService = cardDeskService;
            _playerService = playerService;
        }

        public Game Start()
        {
            var game = new Game {Players = _playerService.Create(NumPlayer), CurrentPlayer = 1};

            var cards = _cardDeskService.Create();
            game.DrawPile = _cardDeskService.Shuffle(cards);

            InitialCarsToPlayer(game);
            AddFirstDiscardPile(game);

            return game;
        }

        public void PlayerTurnExecute(PlayerTurn playerTurn)
        {
            
        }

        private static void InitialCarsToPlayer(Game game)
        {
            foreach (var player in game.Players)
            {
                for (var i = 0; i < NumCard; i++)
                {
                    var firstCard = game.DrawPile.First();
                    player.Hand.Add(firstCard);
                    game.DrawPile.Remove(firstCard);
                }
            }
        }

        private static void AddFirstDiscardPile(Game game)
        {
            var find = game.DrawPile.FirstOrDefault(x => x.Value != CardValue.Reverse 
                                                         && x.Value != CardValue.DrawFour
                                                         && x.Value != CardValue.DrawTwo 
                                                         && x.Value != CardValue.Wild);
            game.DiscardPile.Add(find);
            game.DrawPile.RemoveAt(0);
        }
    }
}

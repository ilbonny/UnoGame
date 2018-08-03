using System.Collections.Generic;
using System.Linq;
using UnoGame.Models;

namespace UnoGame.Services
{
    public interface IGameService
    {
        Game Start();
        Game PlayerTurnExecute(PlayerTurn turn);
        Game DrawDeck();
    }

    public class GameService : IGameService
    {
        private const int NumPlayer = 4;
        private const int NumCard = 7;

        private readonly ICardDeskService _cardDeskService;
        private readonly IPlayerService _playerService;
        private readonly IRuleService _ruleService;

        private Game _game;

        public GameService(ICardDeskService cardDeskService, IPlayerService playerService, IRuleService ruleService)
        {
            _cardDeskService = cardDeskService;
            _playerService = playerService;
            _ruleService = ruleService;
        }

        public Game Start()
        {
            var players = _playerService.Create(NumPlayer);
            _game = new Game {Players = players, CurrentPlayer = players.First() };

            var cards = _cardDeskService.Create();
            _game.DrawPile = _cardDeskService.Shuffle(cards);

            InitialCarsToPlayer();
            AddFirstDiscardPile();

            _game.Message = MessageService.Show(MessageService.StartGame, _game.CurrentPlayer.Position.ToString());

            return _game;
        }
        
        public Game PlayerTurnExecute(PlayerTurn turn)
        {
            _ruleService.Apply(turn, _game);
            return _game;
        }

        private void InitialCarsToPlayer()
        {
            foreach (var player in _game.Players)
            {
                for (var i = 0; i < NumCard; i++)
                {
                    var firstCard = _game.DrawPile.First();
                    player.Hand.Add(firstCard);
                    _game.DrawPile.Remove(firstCard);
                }

                player.Hand = OrderTheCards(player.Hand);
            }
        }

        private void AddFirstDiscardPile()
        {
            var find = _game.DrawPile.FirstOrDefault(x => x.Value != CardValue.Reverse
                                                          && x.Value != CardValue.DrawFour
                                                          && x.Value != CardValue.DrawTwo
                                                          && x.Value != CardValue.Wild);
            _game.DiscardPile.Add(find);
            _game.DrawPile.RemoveAt(0);

            _game.CurrentTurn = new PlayerTurn
            {
                Card = _game.DiscardPile.First(),
            };
        }

        public Game DrawDeck()
        {
            var listDiscard = _game.DiscardPile.Skip(1).Take(_game.DiscardPile.Count-1);

            if (_game.DrawPile.Count == 1)
            {
                _game.DrawPile.AddRange(listDiscard);
                _game.DrawPile = _cardDeskService.Shuffle(_game.DrawPile);
            }
                
            var card = _game.DrawPile.First();

            _game.CurrentPlayer.Hand.Add(card);
            _game.CurrentPlayer.Hand = OrderTheCards(_game.CurrentPlayer.Hand);

            _game.DrawPile.RemoveAt(0);

            _ruleService.SetCurrentPlayer(_game, 1);
            _game.Message = MessageService.Show(MessageService.DrawDeck, _game.CurrentPlayer.Position.ToString());
            return _game;
        }

        public List<Card> OrderTheCards(List<Card> cards)
        {
            return cards.OrderBy(x => x.Color).ThenBy(x => x.Value).ToList();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnoGame.Models;

namespace UnoGame.Services
{
    public interface IGameService
    {
        List<Game> Games { get; set; }
        Game Start(List<User> users);
        void PlayerTurnExecute(PlayerTurn turn);
        void DrawDeck(Guid gameId);
        Game GetGame(Guid gameId, Guid userId);
    }

    public class GameService : IGameService
    {
        private const int NumCard = 7;

        private readonly ICardDeskService _cardDeskService;
        private readonly IPlayerService _playerService;
        private readonly IRuleService _ruleService;

        public List<Game> Games { get; set; }

        public GameService(ICardDeskService cardDeskService, IPlayerService playerService, IRuleService ruleService)
        {
            _cardDeskService = cardDeskService;
            _playerService = playerService;
            _ruleService = ruleService;
            Games = new List<Game>();
        }

        public Game Start(List<User> users)
        {
            var players = _playerService.Create(users);
            var game = new Game {Id = Guid.NewGuid(), Players = players, CurrentPlayer = players.First(), IsReverse = false};

            var cards = _cardDeskService.Create();
            game.DrawPile = _cardDeskService.Shuffle(cards);

            InitialCarsToPlayer(game);
            AddFirstDiscardPile(game);

            game.Message = MessageService.Show(MessageService.StartGame, game.CurrentPlayer.Position.ToString());
            Games.Add(game);

            return game;
        }

        public Game GetGame(Guid gameId, Guid userId)
        {
            var game = Games.FirstOrDefault(x => x.Id == gameId);
            if (game == null) return null;

            return GenerateGameFromUser(game, userId);
        }

        public void PlayerTurnExecute(PlayerTurn turn)
        {
            var game = Games.FirstOrDefault(x => x.Id == turn.GameId);
            if (game == null) return;

            _ruleService.Apply(turn, game);
        }

        public void DrawDeck(Guid gameId)
        {
            var game = Games.FirstOrDefault(x => x.Id == gameId);
            if (game == null) return;

            var listDiscard = game.DiscardPile.Skip(1).Take(game.DiscardPile.Count - 1);

            if (game.DrawPile.Count == 1)
            {
                game.DrawPile.AddRange(listDiscard);
                game.DrawPile = _cardDeskService.Shuffle(game.DrawPile);
            }

            var card = game.DrawPile.First();

            game.CurrentPlayer.Hand.Add(card);
            game.CurrentPlayer.Hand = OrderTheCards(game.CurrentPlayer.Hand);

            game.DrawPile.RemoveAt(0);

            _ruleService.SetCurrentPlayer(game, 1);
            game.Message = MessageService.Show(MessageService.DrawDeck, game.CurrentPlayer.Position.ToString());
        }

        public List<Card> OrderTheCards(List<Card> cards)
        {
            return cards.OrderBy(x => x.Color).ThenBy(x => x.Value).ToList();
        }

        private void InitialCarsToPlayer(Game game)
        {
            foreach (var player in game.Players)
            {
                for (var i = 0; i < NumCard; i++)
                {
                    var firstCard = game.DrawPile.First();
                    player.Hand.Add(firstCard);
                    game.DrawPile.Remove(firstCard);
                }

                player.Hand = OrderTheCards(player.Hand);
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

            game.CurrentTurn = new PlayerTurn
            {
                Card = game.DiscardPile.First(),
            };
        }

        private static Game GenerateGameFromUser(Game game, Guid userId)
        {
            var gameforUser = new Game
            {
                CurrentPlayer = game.CurrentPlayer,
                DiscardPile = game.DiscardPile,
                Players = new List<Player>(),
                CurrentTurn = game.CurrentTurn,
                Message =  game.Message,
                IsReverse = game.IsReverse
            };

            var player = game.Players.FirstOrDefault(x=>x.User.Id == userId);
            gameforUser.Players.Add(player);

            var indexOf = game.Players.IndexOf(player);
            for (var i = 1; i < game.Players.Count; i++)
            {
                var indexCurr = i + indexOf;

                if (i + indexOf >= game.Players.Count)
                    indexCurr = indexCurr - game.Players.Count;

                gameforUser.Players.Add(game.Players[indexCurr]);
            }


            return gameforUser;
        }

    }
}

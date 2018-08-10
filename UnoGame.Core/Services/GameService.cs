using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnoGame.Core.Models;

namespace UnoGame.Core.Services
{
    public interface IGameService
    {
        List<Game> Games { get; set; }
        Game Start(List<User> users);
        void PlayerTurnExecute(PlayerTurn turn);
        void DrawDeck(Guid gameId);
        Game GetGame(Guid gameId, Guid userId);
        bool CheckAutomaticPlayer(Game game);
    }

    public class GameService : IGameService
    {
        private const int NumCard = 7;

        private readonly ICardDeskService _cardDeskService;
        private readonly IPlayerService _playerService;
        private readonly IRuleService _ruleService;
        private readonly IAutomaticPlayerService _automaticPlayerService;

        public List<Game> Games { get; set; }

        public GameService(ICardDeskService cardDeskService, IPlayerService playerService, IRuleService ruleService
            , IAutomaticPlayerService automaticPlayerService)
        {
            _cardDeskService = cardDeskService;
            _playerService = playerService;
            _ruleService = ruleService;
            _automaticPlayerService = automaticPlayerService;
            
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

            game.Message = MessageService.Show(MessageService.StartGame, game.CurrentPlayer.User.UserName);
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

        public bool CheckAutomaticPlayer(Game game)
        {
            if (!game.CurrentPlayer.User.IsAutomatic) return false;

            Thread.Sleep(3000);

            var cardToDiscard = _automaticPlayerService.GetCardToDiscard(game.CurrentPlayer.Hand, game.DiscardPile.Last());

            if (cardToDiscard != null)
            {
                var turn = new PlayerTurn
                {
                    Card = cardToDiscard,
                    UserId = game.CurrentPlayer.User.Id,
                    GameId = game.Id,
                    Num = game.CurrentPlayer.Position
                };

                _ruleService.Apply(turn, game);
            }
            else
                DrawDeck(game.Id);

            return true;
        }

        public void DrawDeck(Guid gameId)
        {
            var game = Games.FirstOrDefault(x => x.Id == gameId);
            if (game == null) return;

            var listDiscard = game.DiscardPile.Skip(1).Take(game.DiscardPile.Count - 1);

            if (game.DrawPile.Count < 5)
            {
                game.DrawPile.AddRange(listDiscard);
                game.DrawPile = _cardDeskService.Shuffle(game.DrawPile);
            }

            var card = game.DrawPile.First();

            game.CurrentPlayer.Hand.Add(card);
            game.CurrentPlayer.Hand = OrderTheCards(game.CurrentPlayer.Hand);

            game.DrawPile.RemoveAt(0);

            _ruleService.SetCurrentPlayer(game, 1);
            game.Message = MessageService.Show(MessageService.DrawDeck, game.CurrentPlayer.User.UserName);
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
                IsReverse = game.IsReverse,
                IsFadeUno = true
            };

            var player = game.Players.FirstOrDefault(x=>x.User.Id == userId);
            gameforUser.Players.Add(player);

            var playersOrder = game.Players.OrderBy(x=>x.Position).ToList();
            var indexOf = playersOrder.IndexOf(player);

            for (var i = 1; i < playersOrder.Count; i++)
            {
                var indexCurr = i + indexOf;

                if (i + indexOf >= playersOrder.Count)
                    indexCurr = indexCurr - playersOrder.Count;

                var playerIndex = playersOrder[indexCurr];
                gameforUser.Players.Add(AddPlayerWithCoverCards(playerIndex));
            }

            return gameforUser;
        }

        private static Player AddPlayerWithCoverCards(Player playerIndex)
        {
            var player = new Player
            {
                Position = playerIndex.Position,
                User = playerIndex.User,
                Hand = new List<Card>()
            };

            var countCards = playerIndex.Hand.Count;
            
            for (var j = 0; j < countCards; j++)
            {
                player.Hand.Add(new Card
                {
                    Value = CardValue.Cover,
                    Color = CardColor.Yellow,
                    Score = 0

                });
            }

            return player;
        }

    }
}

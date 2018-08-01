using System;
using System.Collections.Generic;
using System.Linq;
using UnoGame.Models;

namespace UnoGame.Services
{
    public interface IRuleService
    {
        void Apply(PlayerTurn turn, Game game);
    }

    public class RuleService : IRuleService
    {
        private readonly Dictionary<CardValue, Action<PlayerTurn, Game>> _actions;

        public RuleService()
        {
            _actions =
                new Dictionary<CardValue, Action<PlayerTurn, Game>>
                {
                    {CardValue.Reverse, ReverseMatch}
                };
        }

        public void Apply(PlayerTurn turn, Game game)
        {
            Action<PlayerTurn, Game> action;

            var exist = _actions.TryGetValue(turn.Card.Value, out action);

            if(exist)
                action.Invoke(turn, game);
            else
                NormalMatch(turn, game);
        }

        private static void NormalMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Color != game.CurrentTurn.Card.Color && turn.Card.Value != game.CurrentTurn.Card.Value) return;
            
            RemoveToHandAndAddDiscard(turn, game);
            SetCurrentPlayer(game);
        }

        private static void ReverseMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.Reverse || turn.Card.Color != game.CurrentTurn.Card.Color) return;
            
            RemoveToHandAndAddDiscard(turn, game);

            game.Players.Reverse();
            SetCurrentPlayer(game);
        }

        private static void SetCurrentPlayer(Game game)
        {
            var currentIndex = game.Players.IndexOf(game.CurrentPlayer);
            var indexPlayer = currentIndex + 1 > game.Players.Count-1 ? 0 : currentIndex + 1;

            game.CurrentPlayer = game.Players[indexPlayer];
        }

        private static void RemoveToHandAndAddDiscard(PlayerTurn turn, Game game)
        {
            var cardHandPlayer = game.CurrentPlayer.Hand.Find(x => x.Value == turn.Card.Value && x.Color == turn.Card.Color);
            game.CurrentPlayer.Hand.Remove(cardHandPlayer);

            game.DiscardPile.Add(cardHandPlayer);
            game.CurrentTurn.Card = cardHandPlayer;
        }
    }
}

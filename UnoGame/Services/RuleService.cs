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
                    {CardValue.Reverse, ReverseMatch},
                    {CardValue.Skip, SkipMatch}
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
            SetCurrentPlayer(game, 1);
        }

        private static void ReverseMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.Reverse) return;

            if (game.CurrentTurn.Card.Value == CardValue.Reverse || turn.Card.Color == game.CurrentTurn.Card.Color)
            {
                RemoveToHandAndAddDiscard(turn, game);

                game.Players.Reverse();
                SetCurrentPlayer(game, 1);
            }
        }

        private static void SkipMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.Skip) return;

            if (game.CurrentTurn.Card.Value == CardValue.Skip || turn.Card.Color == game.CurrentTurn.Card.Color)
            {
                RemoveToHandAndAddDiscard(turn, game);

                game.Players.Reverse();
                SetCurrentPlayer(game, 2);
            }
        }

        private static void SetCurrentPlayer(Game game, int next)
        {
            var currentIndex = game.Players.IndexOf(game.CurrentPlayer);
            var move = currentIndex + next;
            var indexPlayer = move > game.Players.Count-1 ? 0 + next - 1 : move;

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

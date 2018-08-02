using System;
using System.Collections.Generic;
using System.Linq;
using UnoGame.Models;

namespace UnoGame.Services
{
    public interface IRuleService
    {
        void Apply(PlayerTurn turn, Game game);
        void SetCurrentPlayer(Game game, int next);
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
                    {CardValue.Skip, SkipMatch},
                    {CardValue.DrawTwo, DrawTwoMatch },
                    {CardValue.DrawFour, DrawFourMatch },
                    {CardValue.Wild, WildMatch }
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

        private void NormalMatch(PlayerTurn turn, Game game)
        {
            if ((turn.Card.Color != game.CurrentTurn.Card.Color && turn.Card.Value == game.CurrentTurn.Card.Value)
                || turn.Card.Color == game.CurrentTurn.Card.Color)
            {
                RemoveToHandAndAddDiscard(turn, game, PredicateFindCardValueAndColor(turn));
                SetCurrentPlayer(game, 1);
            }
        }

        private void ReverseMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.Reverse) return;

            if (game.CurrentTurn.Card.Value == CardValue.Reverse || turn.Card.Color == game.CurrentTurn.Card.Color)
            {
                RemoveToHandAndAddDiscard(turn, game, PredicateFindCardValueAndColor(turn));

                game.Players.Reverse();
                SetCurrentPlayer(game, 1);
            }
        }

        private void SkipMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.Skip) return;

            if (game.CurrentTurn.Card.Value == CardValue.Skip || turn.Card.Color == game.CurrentTurn.Card.Color)
            {
                RemoveToHandAndAddDiscard(turn, game, PredicateFindCardValueAndColor(turn));
                SetCurrentPlayer(game, 2);
            }
        }

        private void DrawTwoMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.DrawTwo) return;

            if (game.CurrentTurn.Card.Value == CardValue.DrawTwo || turn.Card.Color == game.CurrentTurn.Card.Color)
            {
                AddCardToNextPlayer(game, 2);
                RemoveToHandAndAddDiscard(turn, game, PredicateFindCardValueAndColor(turn));
                SetCurrentPlayer(game, 2);
            }
        }

        private void DrawFourMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.DrawFour) return;
            
            AddCardToNextPlayer(game, 4);
            RemoveToHandAndAddDiscard(turn, game, PredicateFindCardValue(turn));
            SetCurrentPlayer(game, 2);
        }

        private void WildMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.Wild) return;

            RemoveToHandAndAddDiscard(turn, game, PredicateFindCardValue(turn));
            SetCurrentPlayer(game, 1);
        }

        public void SetCurrentPlayer(Game game, int next)
        {
            var currentIndex = game.Players.IndexOf(game.CurrentPlayer);
            var move = currentIndex + next;
            var indexPlayer = move > game.Players.Count-1 ? 0 + next - 1 : move;

            game.CurrentPlayer = game.Players[indexPlayer];
        }

        private void RemoveToHandAndAddDiscard(PlayerTurn turn, Game game, Predicate<Card> predicate)
        {
            var cardHandPlayer = game.CurrentPlayer.Hand.Find(predicate);
            game.CurrentPlayer.Hand.Remove(cardHandPlayer);

            game.DiscardPile.Add(cardHandPlayer);
            game.CurrentTurn.Card = turn.Card;
        }

        private void AddCardToNextPlayer(Game game, int numCard)
        {
            var currentIndex = game.Players.IndexOf(game.CurrentPlayer);
            var indexPlayer = currentIndex +1 > game.Players.Count - 1 ? 0 : currentIndex + 1;

            var player = game.Players[indexPlayer];
            var cards = game.DrawPile.Take(numCard);

            player.Hand.AddRange(cards);
            game.DrawPile.RemoveRange(0,numCard);
        }

        private Predicate<Card> PredicateFindCardValue(PlayerTurn turn)
        {
            return x => x.Value == turn.Card.Value;
        }
        private Predicate<Card> PredicateFindCardValueAndColor(PlayerTurn turn)
        {
            return x => x.Value == turn.Card.Value && x.Color == turn.Card.Color;
        }

    }
}

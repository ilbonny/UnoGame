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
                game.Message = MessageService.Show(MessageService.PlayPlayer, game.CurrentPlayer.Position.ToString());
            }
        }

        private void ReverseMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.Reverse) return;

            if (game.CurrentTurn.Card.Value == CardValue.Reverse || turn.Card.Color == game.CurrentTurn.Card.Color)
            {
                RemoveToHandAndAddDiscard(turn, game, PredicateFindCardValueAndColor(turn));

                game.Players.Reverse();
                game.IsReverse = !game.IsReverse;

                SetCurrentPlayer(game, 1);
                game.Message = MessageService.Show(MessageService.Reverse, game.CurrentPlayer.Position.ToString());
            }
        }

        private void SkipMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.Skip) return;

            if (game.CurrentTurn.Card.Value == CardValue.Skip || turn.Card.Color == game.CurrentTurn.Card.Color)
            {
                RemoveToHandAndAddDiscard(turn, game, PredicateFindCardValueAndColor(turn));
                SetCurrentPlayer(game, 2);

                game.Message = MessageService.Show(MessageService.SkipTurn, game.CurrentPlayer.Position.ToString());
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

                game.Message = MessageService.Show(MessageService.DrawTwo, game.CurrentPlayer.Position.ToString());
            }
        }

        private void DrawFourMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.DrawFour) return;

            if (turn.IsChallenge)
                DrawFourChallenge(turn, game);
            else
            {
                DrawFour(turn, game, 4, 2);
                game.Message = MessageService.Show(MessageService.DrawFour, turn.Card.Color.ToString(), game.CurrentPlayer.Position.ToString());
            }
        }

        private void DrawFourChallenge(PlayerTurn turn, Game game)
        {
            var find = game.CurrentPlayer.Hand.FirstOrDefault(x=> x.Color == game.DiscardPile.Last().Color);
            if (find == null)
            {
                DrawFour(turn, game, 6, 2);
                game.Message = MessageService.Show(MessageService.DrawFourFailed, turn.Card.Color.ToString(), game.CurrentPlayer.Position.ToString());
            }
            else
            {
                DrawFourSuccess(turn, game);
                game.Message = MessageService.Show(MessageService.DrawFourSuccess, turn.Card.Color.ToString(), game.CurrentPlayer.Position.ToString());
            }
        }

        private void DrawFour(PlayerTurn turn, Game game, int numCards, int nextPlayer)
        {
            AddCardToNextPlayer(game, numCards);
            RemoveToHandAndAddDiscard(turn, game, PredicateFindCardValue(turn));
            SetCurrentPlayer(game, nextPlayer);
        }

        private void DrawFourSuccess(PlayerTurn turn, Game game)
        {
            var index = game.Players.IndexOf(game.CurrentPlayer);
            AddCardsToPlayer(game, 4, index);

            RemoveToHandAndAddDiscard(turn, game, PredicateFindCardValue(turn));
            SetCurrentPlayer(game, 1);
        }

        private void WildMatch(PlayerTurn turn, Game game)
        {
            if (turn.Card.Value != CardValue.Wild) return;

            RemoveToHandAndAddDiscard(turn, game, PredicateFindCardValue(turn));
            SetCurrentPlayer(game, 1);

            game.Message = MessageService.Show(MessageService.Wild, turn.Card.Color.ToString(), game.CurrentPlayer.Position.ToString());
        }

        public void SetCurrentPlayer(Game game, int next)
        {
            var currentIndex = game.Players.IndexOf(game.CurrentPlayer);
            var move = currentIndex + next;
            var count = game.Players.Count - 1;

            var indexPlayer = move > count ? move-count-1 : move;

            game.CurrentPlayer = game.Players[indexPlayer];
        }

        private void RemoveToHandAndAddDiscard(PlayerTurn turn, Game game, Predicate<Card> predicate)
        {
            var cardHandPlayer = game.CurrentPlayer.Hand.Find(predicate);
            game.CurrentPlayer.Hand.Remove(cardHandPlayer);

            game.DiscardPile.Add(turn.Card);
            game.CurrentTurn.Card = turn.Card;
        }

        private void AddCardToNextPlayer(Game game, int numCard)
        {
            var currentIndex = game.Players.IndexOf(game.CurrentPlayer);
            var indexPlayer = currentIndex +1 > game.Players.Count - 1 ? 0 : currentIndex + 1;
            AddCardsToPlayer(game, numCard, indexPlayer);
        }

        private void AddCardsToPlayer(Game game,  int numCard, int indexPlayer)
        {
            var player = game.Players[indexPlayer];
            var cards = game.DrawPile.Take(numCard);

            player.Hand.AddRange(cards);
            game.DrawPile.RemoveRange(0, numCard);
        }

        private static Predicate<Card> PredicateFindCardValue(PlayerTurn turn)
        {
            return x => x.Value == turn.Card.Value;
        }

        private static Predicate<Card> PredicateFindCardValueAndColor(PlayerTurn turn)
        {
            return x => x.Value == turn.Card.Value && x.Color == turn.Card.Color;
        }
    }
}

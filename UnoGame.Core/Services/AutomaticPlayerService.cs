using System.Collections.Generic;
using System.Linq;
using UnoGame.Core.Models;

namespace UnoGame.Core.Services
{
    public interface IAutomaticPlayerService
    {
        Card GetCardToDiscard(List<Card> handCards, Card comparisonCard);
    }

    public class AutomaticPlayerService : IAutomaticPlayerService
    {
        private List<Card> _handCards;
        private Card _comparisonCard;
        private Card _cardSelected;

        public Card GetCardToDiscard(List<Card> handCards, Card comparisonCard)
        {
            _handCards = handCards;
            _comparisonCard = comparisonCard;
            _cardSelected = null;

            CanAddCardSameColor()
                .CanAddCardSameValue()
                .CanAddCardWild()
                .CanAddCardDrawFour();

            return _cardSelected;
        }

        private AutomaticPlayerService CanAddCardSameColor()
        {
            if (_cardSelected != null) return this;
            _cardSelected = _handCards.FirstOrDefault(x=> x.Value != CardValue.DrawFour && x.Value!= CardValue.Wild && x.Color == _comparisonCard.Color);
            return this;
        }

        private AutomaticPlayerService CanAddCardSameValue()
        {
            if (_cardSelected != null) return this;
            _cardSelected = _handCards.FirstOrDefault(x => x.Value == _comparisonCard.Value);
            return this;
        }

        private AutomaticPlayerService CanAddCardWild()
        {
            if (_cardSelected != null) return this;

            var card = _handCards.FirstOrDefault(x=> x.Value == CardValue.Wild);
            if (card == null) return this;

            card.Color = GetCardWithMoreColor();
            _cardSelected = card;

            return this;
        }

        private AutomaticPlayerService CanAddCardDrawFour() 
        {
            if (_cardSelected != null) return this;

            var card = _handCards.FirstOrDefault(x => x.Value == CardValue.DrawFour);
            if (card == null) return null;

            card.Color = GetCardWithMoreColor();
            _cardSelected = card;

            return this;
        }

        

        private CardColor GetCardWithMoreColor()
        {
            var groupByCard = _handCards.GroupBy(x => x.Color)
                 .Select(group => new { Color = group.Key, Count = group.Count() }).OrderByDescending(c => c.Count);

            var cardWithMoreColor = groupByCard.First();
            return cardWithMoreColor.Color;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnoGame.Core.Models;

namespace UnoGame.Core.Services
{
    public interface ICardDeskService
    {
        List<Card> Create();
        List<Card> Shuffle(List<Card> cards);
        //List<Card> Draw(int count);
    }

    public class CardDeskService : ICardDeskService
    {
        public List<Card> Create()
        {
            var cards = new List<Card>();

            foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
            {
                foreach (CardValue val in Enum.GetValues(typeof(CardValue)))
                {
                    if (val == CardValue.Cover) continue;

                    cards.Add(new Card
                    {
                        Color = color,
                        Value = val,
                        Score = (int)val
                    });
                }
            }

            return cards.OrderBy(x => x.Color).ToList();
        }

        public List<Card> Shuffle(List<Card> cards)
        {
            var r = new Random();
            
            for (var n = cards.Count - 1; n > 0; --n)
            {
                var k = r.Next(n + 1);
                var temp = cards[n];
                cards[n] = cards[k];
                cards[k] = temp;
            }

            return cards;
        }

        //public List<Card> Draw(int count)
        //{
        //    var drawnCards = Cards.Take(count).ToList();
        //    Cards.RemoveAll(x => drawnCards.Contains(x));
        //    return drawnCards;
        //}
    }
}

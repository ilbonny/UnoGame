using System.Collections.Generic;

namespace UnoGame.Core.Models
{
    public class CardDeck 
    {
        public List<Card> Cards { get; set; }

        public CardDeck()
        {
            Cards = new List<Card>();
        }
    }
}
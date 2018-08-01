using System.Collections.Generic;

namespace UnoGame.Models
{
    public class Player
    {
        public List<Card> Hand { get; set; }
        public int Position { get; set; }

        public Player()
        {
            Hand = new List<Card>();
        }
    }
}

using System.Collections.Generic;

namespace UnoGame.Core.Models
{
    public class Player
    {
        public User User { get; set; }
        public List<Card> Hand { get; set; }
        public int Position { get; set; }
        public bool IsAutomatic { get; set; }

        public Player()
        {
            Hand = new List<Card>();
        }
    }
}

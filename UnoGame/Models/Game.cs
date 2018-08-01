using System.Collections.Generic;

namespace UnoGame.Models
{
    public class Game
    {
        public List<Player> Players { get; set; }
        public List<Card> DrawPile { get; set; }
        public List<Card> DiscardPile { get; set; }
        public int CurrentPlayer { get; set; }

        public Game()
        {
            Players = new List<Player>();
            DrawPile= new List<Card>();
            DiscardPile = new List<Card>();
        }
    }
}

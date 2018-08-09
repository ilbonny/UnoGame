using System;
using System.Collections.Generic;

namespace UnoGame.Core.Models
{
    public class Game
    {
        public Guid Id { get; set; }
        public List<Player> Players { get; set; }
        public List<Card> DrawPile { get; set; }
        public List<Card> DiscardPile { get; set; }
        public PlayerTurn CurrentTurn { get; set; }
        public Player CurrentPlayer { get; set; }
        public string Message { get; set; }
        public bool IsReverse { get; set; }

        public Game()
        {
            Players = new List<Player>();
            DrawPile= new List<Card>();
            DiscardPile = new List<Card>();
        }
    }
}

using System;

namespace UnoGame.Models
{
    public class PlayerTurn
    {
        public Guid GameId { get; set; }
        public Guid UserId { get; set; }

        public int Num { get; set; }
        public Card Card { get; set; }
        public bool IsChallenge { get; set; }
    }
}

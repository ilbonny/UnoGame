namespace UnoGame.Models
{
    public class PlayerTurn
    {
        public int Num { get; set; }
        public Card Card { get; set; }
        public CardColor DeclaredColor { get; set; }
        public TurnResult Result { get; set; }
    }
}

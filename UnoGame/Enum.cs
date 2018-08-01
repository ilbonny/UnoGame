namespace UnoGame
{
    public enum CardColor
    {
        Red,
        Blue,
        Yellow,
        Green
    }

    public enum CardValue
    {
        Zero,
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Reverse = 21,
        Skip = 22,
        DrawTwo = 25,
        Wild = 40,
        DrawFour = 50,
    }

    public enum TurnResult
    {
        //Start of game.
        GameStart,

        //Player played a normal number card.
        PlayedCard,

        //Player played a skip card.
        Skip,

        //Player played a draw two card.
        DrawTwo,

        //Player was forced to draw by other player's card.
        Attacked,

        //Player was forced to draw because s/he couldn't match the current discard.
        ForceDraw,

        //Player was forced to draw because s/he couldn't match the current discard, but the drawn card was played.
        ForceDrawPlay,

        //Player played a regular wild card.
        WildCard,

        //Player played a draw-four wild card.
        WildDrawFour,

        //Player played a reverse card.
        Reversed
    }
}

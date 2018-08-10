namespace UnoGame.Core.Services
{
    public class MessageService
    {
        public const string StartGame = "Start the game play {0}";
        public const string PlayPlayer = "Play {0}";
        public const string SkipTurn = "Skip the turn play {0}";
        public const string Reverse = "Reverse play {0}";
        public const string DrawTwo = "Draw two play {0}";
        public const string DrawFour = "Draw four change color {0} play {1}";
        public const string Wild = "Change color {0} play {1}";
        public const string DrawDeck = "Draw a card play {0}";
        public const string DrawFourFailed = "Challenge failed six cards change color {0} play {1}";
        public const string DrawFourSuccess = "Challenge success four cards change color {0} play {1}";
        public const string PlayerWin = "Finished game wins {0}";


        public static string Show(string constMess, params object[] args)
        {
            return string.Format(constMess, args);
        }
    }
}

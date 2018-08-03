namespace UnoGame.Services
{
    public class MessageService
    {
        public const string StartGame = "Start the game play Player {0}";
        public const string PlayPlayer = "Play Player {0}";
        public const string SkipTurn = "Skip the turn play Player {0}";
        public const string Reverse = "Reverse play Player {0}";
        public const string DrawTwo = "Draw two play Player {0}";
        public const string DrawFour = "Draw four change color {0} play Player {1}";
        public const string Wild = "Change color {0} play Player {1}";
        public const string DrawDeck = "Draw a card play Player {0}";
        public const string DrawFourFailed = "Challenge failed six cards change color {0} play Player {1}";
        public const string DrawFourSuccess = "Challenge success four cards change color {0} play Player {1}";


        public static string Show(string constMess, params object[] args)
        {
            return string.Format(constMess, args);
        }
    }
}

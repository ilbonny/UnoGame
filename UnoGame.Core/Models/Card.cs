using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace UnoGame.Core.Models
{
    public class Card
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CardColor Color { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public CardValue Value { get; set; }

        public int Score { get; set; }
    }
}

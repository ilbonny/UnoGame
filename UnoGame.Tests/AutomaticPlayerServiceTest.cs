using System.Collections.Generic;
using NUnit.Framework;
using UnoGame.Core.Models;
using UnoGame.Core.Services;

namespace UnoGame.Tests
{
    [TestFixture]
    public class AutomaticPlayerServiceTest
    {
        private AutomaticPlayerService _automaticPlayerService;

        [SetUp]
        public void SetUp()
        {
            _automaticPlayerService = new AutomaticPlayerService();
        }

        [Test]
        public void CanAddCardSameColor_that_not_is_a_special_card()
        {
            var comparisionCard = new Card { Color = CardColor.Blue, Value = CardValue.Zero, Score = 0 };

            var cards = new List<Card>
            {
                new Card {Color = CardColor.Blue, Value = CardValue.Wild},
                new Card {Color = CardColor.Blue, Value = CardValue.DrawFour},
                new Card {Color = CardColor.Blue, Value = CardValue.Five},
                new Card {Color = CardColor.Red, Value = CardValue.One},
                new Card {Color = CardColor.Yellow, Value = CardValue.Nine},
                
            };
            
            var card = _automaticPlayerService.GetCardToDiscard(cards, comparisionCard);

            Assert.That(card.Color, Is.EqualTo(comparisionCard.Color));
            Assert.That(card.Value, Is.EqualTo(CardValue.Five));
        }

        [Test]
        public void CanAddCardSameColor_draw_two()
        {
            var comparisionCard = new Card { Color = CardColor.Blue, Value = CardValue.Zero, Score = 0 };

            var cards = new List<Card>
            {
                new Card {Color = CardColor.Blue, Value = CardValue.DrawTwo},
                new Card {Color = CardColor.Red, Value = CardValue.One},
                new Card {Color = CardColor.Yellow, Value = CardValue.Nine}
            };

            var card = _automaticPlayerService.GetCardToDiscard(cards, comparisionCard);

            Assert.That(card.Color, Is.EqualTo(comparisionCard.Color));
            Assert.That(card.Value, Is.EqualTo(CardValue.DrawTwo));
        }

        [Test]
        public void CanAddCardSameValue()
        {
            var comparisionCard = new Card { Color = CardColor.Green, Value = CardValue.One};

            var cards = new List<Card>
            {
                new Card {Color = CardColor.Blue, Value = CardValue.Five},
                new Card {Color = CardColor.Yellow, Value = CardValue.Nine},
                new Card {Color = CardColor.Red, Value = CardValue.One}
            };

            var card = _automaticPlayerService.GetCardToDiscard(cards, comparisionCard);

            Assert.That(card.Color, Is.Not.EqualTo(comparisionCard.Color));
            Assert.That(card.Value, Is.EqualTo(comparisionCard.Value));
        }

        [Test]
        public void CanAddCardWild()
        {
            var comparisionCard = new Card { Color = CardColor.Blue, Value = CardValue.Zero, Score = 0 };

            var cards = new List<Card>
            {
                new Card {Color = CardColor.Yellow, Value = CardValue.Wild},
                new Card {Color = CardColor.Yellow, Value = CardValue.Nine},
                new Card {Color = CardColor.Red, Value = CardValue.One},
                new Card {Color = CardColor.Red, Value = CardValue.Two},
                new Card {Color = CardColor.Red, Value = CardValue.Three},
            };

            var card = _automaticPlayerService.GetCardToDiscard(cards, comparisionCard);

            Assert.That(card.Value, Is.EqualTo(CardValue.Wild));
            Assert.That(card.Color, Is.EqualTo(CardColor.Red));
        }

        [Test]
        public void CanAddCardDrawFour()
        {
            var comparisionCard = new Card { Color = CardColor.Blue, Value = CardValue.Zero, Score = 0 };

            var cards = new List<Card>
            {
                new Card {Color = CardColor.Yellow, Value = CardValue.DrawFour},
                new Card {Color = CardColor.Yellow, Value = CardValue.Nine},
                new Card {Color = CardColor.Red, Value = CardValue.One},
                new Card {Color = CardColor.Red, Value = CardValue.Two},
                new Card {Color = CardColor.Red, Value = CardValue.Three},
            };

            var card = _automaticPlayerService.GetCardToDiscard(cards, comparisionCard);

            Assert.That(card.Value, Is.EqualTo(CardValue.DrawFour));
            Assert.That(card.Color, Is.EqualTo(CardColor.Red));
        }

        
        [TestCase(CardColor.Blue, CardValue.Zero, CardColor.Blue, CardValue.Five)]
        [TestCase(CardColor.Yellow, CardValue.One, CardColor.Red, CardValue.One)]
        [TestCase(CardColor.Green, CardValue.Three, CardColor.Blue, CardValue.Wild)]
        public void Verification_of_the_priorities_of_the_rules(CardColor compareColor, CardValue compareValue,
            CardColor expectColor, CardValue expectValue)
        {
            var cards = new List<Card>
            {
                new Card {Color = CardColor.Blue, Value = CardValue.Five},
                new Card {Color = CardColor.Blue, Value = CardValue.Nine},
                new Card {Color = CardColor.Blue, Value = CardValue.Wild},
                new Card {Color = CardColor.Red, Value = CardValue.One},
            };

            var card = _automaticPlayerService.GetCardToDiscard(cards,
                new Card {Color = compareColor, Value = compareValue});

            Assert.That(card.Color, Is.EqualTo(expectColor));
            Assert.That(card.Value, Is.EqualTo(expectValue));

        }
    }
}

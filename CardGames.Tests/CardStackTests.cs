using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CardGames.Tests
{
    [TestClass]
    public class CardStackTests
    {
        [TestMethod]
        public void DeckInitTest()
        {
            {
                Deck deck = new Deck();
                Assert.AreEqual(52, deck.Count);
            }
            {
                Deck deck = new Deck(true);
                Assert.AreEqual(54, deck.Count);
            }
        }

        [TestMethod]
        public void DrawFiveTest()
        {
            Deck deck = new Deck();
            List<Card> cards = new List<Card>();
            int result = deck.Draw(cards, 33, 48, 13, 43, 22);

            Assert.AreEqual(5, result);
            Assert.AreEqual(5, cards.Count);
            areEqualAssert(Face._9, Suit.Diamonds, cards[0]);
            areEqualAssert(Face.King, Suit.Clubs, cards[1]);
            areEqualAssert(Face._4, Suit.Diamonds, cards[2]);
            areEqualAssert(Face.Jack, Suit.Spades, cards[3]);
            areEqualAssert(Face._6, Suit.Hearts, cards[4]);
            Assert.AreEqual(47, deck.Count);
            Assert.IsFalse(deck.Contains(cards[0]));
            Assert.IsFalse(deck.Contains(cards[1]));
            Assert.IsFalse(deck.Contains(cards[2]));
            Assert.IsFalse(deck.Contains(cards[3]));
            Assert.IsFalse(deck.Contains(cards[4]));
        }

        [TestMethod]
        public void DrawFromTopThreeTest()
        {
            Deck deck = new Deck();
            List<Card> cards = new List<Card>();
            int result = deck.DrawFromTop(cards, 3);

            Assert.AreEqual(3, result);
            Assert.AreEqual(3, cards.Count);
            areEqualAssert(Face.Ace, Suit.Clubs, cards[0]);
            areEqualAssert(Face.Ace, Suit.Diamonds, cards[1]);
            areEqualAssert(Face.Ace, Suit.Hearts, cards[2]);
            Assert.AreEqual(49, deck.Count);
            Assert.IsFalse(deck.Contains(cards[0]));
            Assert.IsFalse(deck.Contains(cards[1]));
            Assert.IsFalse(deck.Contains(cards[2]));
        }

        [TestMethod]
        public void ModifiedEventTest()
        {
            Deck deck = new Deck();
            CardStack stack = new CardStack("Other3");
            CardStack stack2 = new CardStack("Other4");
            bool modified = false;
            stack.Modified += () => { modified = true; };
            
            deck.DrawFromTop(stack2, 10);
            stack2.StackOnto(stack);
            Assert.IsTrue(modified);

            modified = false;
            stack.DrawFromTop(deck, 2);
            Assert.IsTrue(modified);

            modified = false;
            stack.StackOnto(stack2);
            Assert.IsTrue(modified);
        }

        [TestMethod]
        public void DrawEventTest()
        {
            Deck deck = new Deck();
            bool drawn = false;
            deck.CardsDrawn += (stack, cards) =>
            {
                drawn = true;
                Assert.AreEqual(deck, stack);
                areEqualAssert(Face.Ace, Suit.Clubs, cards.First());
            };

            List<Card> dest = new List<Card>();
            deck.DrawFromTop(dest, 1);

            Assert.IsTrue(drawn);
        }

        [TestMethod]
        public void DrawCancelTest()
        {
            Deck deck = new Deck();
            bool drawn = false;
            bool cancelled = false;
            deck.AboutToDraw += (sender, e) =>
            {
                cancelled = true;
                Assert.AreEqual(deck, sender);
                Assert.AreEqual(e.CardStack, deck);
                areEqualAssert(Face.Ace, Suit.Clubs, e.Cards.First());
                e.Cancel = true;
            };
            deck.CardsDrawn += (stack, cards) => { drawn = true; };

            List<Card> dest = new List<Card>();
            deck.DrawFromTop(dest, 1);

            Assert.IsTrue(cancelled);
            Assert.IsFalse(drawn);
        }

        [TestMethod]
        public void ReceiveEventTest()
        {
            Deck deck = new Deck();
            CardStack stack = new CardStack("Other");
            bool received = false;
            HashSet<Card> receivedCards = new HashSet<Card>(deck);
            stack.CardsReceived += (sender, cards) =>
            {
                received = true;
                Assert.AreEqual(deck, sender);
                Assert.IsTrue(receivedCards.SetEquals(cards));
            };

            deck.StackOnto(stack);

            Assert.IsTrue(received);
            Assert.AreEqual(0, deck.Count);
            Assert.AreEqual(52, stack.Count);
        }

        [TestMethod]
        public void ReceiveCancelTest()
        {
            Deck deck = new Deck();
            CardStack stack = new CardStack("Other2");
            bool cancelled = false;
            bool received = false;
            HashSet<Card> receivedCards = new HashSet<Card>(deck);
            stack.AboutToReceiveCards += (sender, e) =>
            {
                cancelled = true;
                Assert.AreEqual(stack, sender);
                Assert.AreEqual(deck, e.CardStack);
                Assert.IsTrue(receivedCards.SetEquals(e.Cards));
                e.Cancel = true;
            };
            stack.CardsReceived += (sender, cards) => { received = true; };

            deck.StackOnto(stack);

            Assert.IsTrue(cancelled);
            Assert.IsFalse(received);
            Assert.AreEqual(52, deck.Count);
            Assert.AreEqual(0, stack.Count);
        }

        [TestMethod]
        public void TryDrawFiveTest()
        {
            Deck deck = new Deck();
            var action = deck.TryDraw(33, 48, 13, 43, 22);
            var cards = action.Cards.ToList();

            Assert.AreEqual(deck, action.SourceSequence);
            Assert.AreEqual(5, cards.Count());
            areEqualAssert(Face._9, Suit.Diamonds, cards[0]);
            areEqualAssert(Face.King, Suit.Clubs, cards[1]);
            areEqualAssert(Face._4, Suit.Diamonds, cards[2]);
            areEqualAssert(Face.Jack, Suit.Spades, cards[3]);
            areEqualAssert(Face._6, Suit.Hearts, cards[4]);
            Assert.AreEqual(47, deck.Count);
            Assert.IsFalse(deck.Contains(cards[0]));
            Assert.IsFalse(deck.Contains(cards[1]));
            Assert.IsFalse(deck.Contains(cards[2]));
            Assert.IsFalse(deck.Contains(cards[3]));
            Assert.IsFalse(deck.Contains(cards[4]));

            action.Undo();

            Assert.AreEqual(null, action.SourceSequence);
            Assert.IsFalse(action.Cards.Any());
            Assert.AreEqual(52, deck.Count);
            Assert.IsTrue(deck.Contains(cards[0]));
            Assert.IsTrue(deck.Contains(cards[1]));
            Assert.IsTrue(deck.Contains(cards[2]));
            Assert.IsTrue(deck.Contains(cards[3]));
            Assert.IsTrue(deck.Contains(cards[4]));
        }

        private void areEqualAssert(Face expectedFaceValue, Suit expectedSuit, Card card)
        {
            Assert.AreEqual(expectedFaceValue, card.FaceValue);
            Assert.AreEqual(expectedSuit, card.Suit);
        }
    }
}

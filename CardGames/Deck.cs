using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames
{
    public class Deck : CardStack
    {
        private static int DeckCount = 0;

        public Deck(bool includeJokers) : base(string.Format("Deck{0}", ++DeckCount))
        {
            for (int i = 1; i <= 13; i++)
            {
                Add(new Card(this, Suit.Clubs, (Face)i));
                Add(new Card(this, Suit.Diamonds, (Face)i));
                Add(new Card(this, Suit.Hearts, (Face)i));
                Add(new Card(this, Suit.Spades, (Face)i));
            }

            if (includeJokers)
            {
                Add(new Card.Joker(this, Colour.Red));
                Add(new Card.Joker(this, Colour.Black));
            }
        }
    }
}

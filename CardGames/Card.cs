using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames
{
    public class Card : IEquatable<Card>, ICardSequence
    {
        protected Card() {}

        public class Joker : Card
        {
            private Colour _colour;
            public Joker(Deck src, Colour clr)
            {
                _srcDeck = src;
                _colour = clr;
                _suit = Suit.None;
                _face = Face.Joker;
            }

            public override Colour Colour
            {
                get { return _colour; }
            }
        }

        protected Deck _srcDeck;
        
        public Card(Deck src, Suit suit, Face face)
        {
            if (suit == Suit.None || face == Face.Joker)
                throw new Exception("Cannot instatiate Joker explicitly.");

            _srcDeck = src;
            _suit = suit;
            _face = face;
        }

        protected Suit _suit;
        public Suit Suit
        {
            get { return _suit; }
        }

        protected Face _face;
        public Face FaceValue
        {
            get { return _face; }
        }

        public virtual Colour Colour
        {
            get
            {
                return (_suit == Suit.Diamonds || _suit == Suit.Hearts) ?
                    Colour.Red : Colour.Black;
            }
        }

        private CardVisibility _vis = CardVisibility.FaceDown;
        public CardVisibility Visibility
        {
            get { return _vis; }
            internal set { _vis = value; }
        }

        public ICardCollection Owner { get; internal set; }

        #region IEquatable<Card> Members

        public bool Equals(Card c)
        {
            return c.Suit == Suit && c.FaceValue == FaceValue;
        }

        #endregion

        public void Flip()
        {
            if (_vis != CardVisibility.FaceUp)
                _vis = CardVisibility.FaceUp;
            else
                _vis = CardVisibility.FaceDown;
            emitModified();
        }

        public void Grab()
        {
            if (_vis == CardVisibility.FaceDown)
                _vis = CardVisibility.PlayerHand;
            Owner = null;
            emitModified();
        }

        public void ReturnToDeck()
        {
            _vis = CardVisibility.FaceDown;
            _srcDeck.Add(this);
            Owner = _srcDeck;
            emitModified();
        }

        public bool StackOnto(CardStack stack)
        {
            return stack.receiveCardsOnTop(this);
        }

        #region ICardSequence
        public void Shuffle()
        {
        }

        #region ICardCollection
        public event Action Modified;
        private void emitModified()
        {
            if (Modified != null)
                Modified();
        }

        public int DrawRandom(int count, ICollection<Card> dest)
        {
            Owner = null;
            dest.Add(this);
            return 1;
        }

        public int DrawSequential(int count, ICollection<Card> dest)
        {
            Owner = null;
            dest.Add(this);
            return 1;
        }

        #region IList
        public void Add(Card item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(Card item)
        {
            return Equals(item);
        }

        public void CopyTo(Card[] array, int arrayIndex)
        {
            array[arrayIndex] = this;
        }

        public int Count
        {
            get { return 1; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove(Card item)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Card> GetEnumerator()
        {
            yield return this;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(Card item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, Card item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public Card this[int index]
        {
            get
            {
                if (index == 0)
                    return this;
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion
        #endregion
        #endregion
    }
}

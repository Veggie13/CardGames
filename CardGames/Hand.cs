using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace CardGames
{
    public class Hand : ICardSequence
    {
        private List<Card> _cards = new List<Card>();

        #region ICardSequence
        public event Action Modified = delegate { };

        public void Shuffle()
        {
            var rand = new Random();
            var newCards = new List<Card>();

            while (_cards.Count > 0)
            {
                int i = rand.Next(_cards.Count);
                Card c = _cards[i];
                _cards.RemoveAt(i);
                newCards.Add(c);
            }

            _cards = newCards;
            emitModified();
        }

        public int Draw(ICollection<Card> dest, params int[] positions)
        {
            throwIfInvalid(positions);

            List<Card> drawn = positions.Select(p => _cards[p]).ToList();
            return drawCards(dest, drawn);
        }

        public int DrawFromTop(ICollection<Card> dest, int count)
        {
            return Draw(dest, Enumerable.Range(0, count).ToArray());
        }

        public ICardSequenceAction TryDraw(params int[] positions)
        {
            throwIfInvalid(positions);

            var map = positions.Select(p => new Tuple<int, Card>(p, _cards[p])).ToList();
            List<Card> dest = new List<Card>();
            drawCards(dest, map.Select(t => t.Item2));
            return new NullCardSequenceAction();
        }

        public ICardSequenceAction TryDrawFromTop(int count)
        {
            return TryDraw(Enumerable.Range(0, count).ToArray());
        }

        #region ICollection<Card>
        public void Add(Card item)
        {
            _cards.Add(item);
            //attachCards(item);
            emitModified();
        }

        public void Clear()
        {
            //detachCards(_cards);
            foreach (Card c in _cards)
                c.ReturnToDeck();
            _cards.Clear();
            emitModified();
        }

        public bool Contains(Card item)
        {
            return _cards.Contains(item);
        }

        public void CopyTo(Card[] array, int arrayIndex)
        {
            _cards.CopyTo(array, arrayIndex);
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Card item)
        {
            if (_cards.Remove(item))
            {
                //detachCards(item);
                item.ReturnToDeck();
                emitModified();
                return true;
            }
            return false;
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return _cards.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int IndexOf(Card item)
        {
            return _cards.IndexOf(item);
        }

        public void Insert(int index, Card item)
        {
            _cards.Insert(index, item);
            //attachCards(item);
            emitModified();
        }

        public void RemoveAt(int index)
        {
            //detachCards(_cards[index]);
            _cards[index].ReturnToDeck();
            _cards.RemoveAt(index);
            emitModified();
        }

        public Card this[int index]
        {
            get
            {
                return _cards[index];
            }
            set
            {
                _cards[index] = value;
            }
        }

        public int Count
        {
            get { return _cards.Count; }
        }
        #endregion
        #endregion

        private int drawCards(ICollection<Card> dest, IEnumerable<Card> drawn)
        {
            foreach (Card c in drawn)
            {
                _cards.Remove(c);
                //detachCards(c);
                c.Grab();
                dest.Add(c);
            }

            if (drawn.Any())
            {
                //emitCardsDrawn(drawn);
                emitModified();
            }

            return drawn.Count();
        }

        private void emitModified()
        {
            Modified();
        }

        private void throwIfInvalid(IEnumerable<int> indices)
        {
            if (indices.Any(i => 0 <= i && i < _cards.Count))
                throw new CardException("Invalid Index");
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames
{
    public class Hand : ICardCollection
    {
        private List<Card> _cards = new List<Card>();

        public int DrawRandom(int count, ICollection<Card> dest)
        {
            var rand = new Random();

            int i = 0;
            for (; i < count && _cards.Count > 0; i++)
            {
                int j = rand.Next(_cards.Count);
                Card c = _cards[j];
                _cards.RemoveAt(j);
                c.Grab();
                dest.Add(c);
            }

            return i;
        }

        public int DrawSequential(int count, ICollection<Card> dest)
        {
            return DrawRandom(count, dest);
        }

        public void Add(Card item)
        {
            _cards.Add(item);
        }

        public void Clear()
        {
            foreach (Card c in _cards)
                c.ReturnToDeck();
            _cards.Clear();
        }

        public bool Contains(Card item)
        {
            return _cards.Contains(item);
        }

        public void CopyTo(Card[] array, int arrayIndex)
        {
            _cards.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _cards.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Card item)
        {
            if (_cards.Remove(item))
            {
                item.ReturnToDeck();
                return true;
            }
            return false;
        }

        public IEnumerator<Card> GetEnumerator()
        {
            return _cards.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public event Action Modified;
    }
}

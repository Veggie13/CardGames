using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CardGames
{
    public static class CardStackExtensions
    {
        public static void Shuffle(this IList<Card> cards)
        {

        }
    }

    public class CardStack : ICardSequence
    {
        #region Supporting Classes
        public class CardStackManager
        {
            private Dictionary<string, CardStack> _piles = new Dictionary<string, CardStack>();
            public CardStack this[string name]
            {
                get { return _piles[name]; }
            }

            internal void Add(string name, CardStack pile)
            {
                if (!_piles.ContainsKey(name))
                    _piles[name] = pile;
                else
                    throw new Exception(string.Format("Pile with name \"{0}\" already exists.", name));
            }
        }

        public class CardEventArgs : EventArgs
        {
            public CardEventArgs()
            {
                Cancel = false;
            }

            public IEnumerable<Card> Cards { get; set; }
            public bool Cancel { get; set; }
        }
        #endregion

        #region Members
        private List<Card> _cards = new List<Card>();
        private int _cardsModified = 0;
        #endregion

        #region Static
        private static CardStackManager s_mgr = new CardStackManager();
        public static CardStackManager Manager
        {
            get { return s_mgr; }
        }
        #endregion

        public CardStack(string name)
        {
            Name = name;
            Manager.Add(name, this);
        }

        #region Properties
        public string Name { get; private set; }

        public Card Top
        {
            get
            {
                return (Count > 0) ? _cards[0] : null;
            }
        }

        public Card Bottom
        {
            get
            {
                return (Count > 0) ? _cards[_cards.Count - 1] : null;
            }
        }

        public int Count
        {
            get { return _cards.Count; }
        }
        #endregion

        #region Events
        public delegate void CardPreEventHandler(CardStack stack, CardEventArgs e);
        public delegate void CardPostEventHandler(CardStack stack, IEnumerable<Card> cards);

        public event CardPreEventHandler AboutToDraw;
        public event CardPostEventHandler CardsDrawn;

        public event CardPreEventHandler AboutToReceiveCards;
        public event CardPostEventHandler CardsReceived;

        public event CardPreEventHandler AboutToActivate;
        public event CardPostEventHandler Activated;
        #endregion

        #region Public Methods
        public void Flip()
        {
            foreach (Card c in _cards)
                c.Flip();

            _cards.Reverse();
        }

        public void FlipTop()
        {
            Top.Flip();
        }

        public bool StackOnto(CardStack stack)
        {
            if (!stack.receiveCardsOnTop(this))
                return false;

            detachCards(_cards);
            _cards.Clear();
            emitModified();
            return true;
        }

        public bool Activate()
        {
            if (AboutToActivate != null)
            {
                var e = new CardEventArgs();
                AboutToActivate(this, e);
                if (e.Cancel)
                    return false;
            }

            if (Activated != null)
            {
                Activated(this, null);
            }

            return true;
        }
        #endregion

        #region ICardSequence
        public event Action Modified;

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

        public int DrawRandom(int count, ICollection<Card> dest)
        {
            var rand = new Random();

            int i = 0;
            for (; i < count && _cards.Count > 0; i++)
            {
                int j = rand.Next(_cards.Count);
                Card c = _cards[j];
                _cards.RemoveAt(j);
                detachCards(c);
                c.Grab();
                dest.Add(c);
            }

            if (i > 0)
                emitModified();

            return i;
        }

        public int DrawSequential(int count, ICollection<Card> dest)
        {
            if (!doAboutToDraw())
                return 0;

            int i = 0;
            List<Card> drawn = new List<Card>();
            for (; i < count && _cards.Count > 0; i++)
            {
                Card c = Top;
                _cards.Remove(c);
                detachCards(c);
                c.Grab();
                dest.Add(c);
                drawn.Add(c);
            }

            if (i > 0)
            {
                emitCardsDrawn(drawn);
                emitModified();
            }

            return i;
        }

        public void Add(Card item)
        {
            _cards.Add(item);
            attachCards(item);
            emitModified();
        }

        public void Clear()
        {
            detachCards(_cards);
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
                detachCards(item);
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
            attachCards(item);
            emitModified();
        }

        public void RemoveAt(int index)
        {
            detachCards(_cards[index]);
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
        #endregion

        #region Helpers
        private void emitModified()
        {
            if (Modified != null)
                Modified();
        }

        private bool doAboutToReceiveCards(IEnumerable<Card> cards)
        {
            if (AboutToReceiveCards != null)
            {
                var e = new CardEventArgs();
                e.Cards = cards;
                AboutToReceiveCards(this, e);
                if (e.Cancel)
                    return false;
            }

            return true;
        }

        private bool doAboutToDraw()
        {
            if (AboutToDraw != null)
            {
                var e = new CardEventArgs();
                AboutToDraw(this, e);
                if (e.Cancel)
                    return false;
            }

            return true;
        }

        private void emitCardsDrawn(IEnumerable<Card> cards)
        {
            if (CardsDrawn != null)
                CardsDrawn(this, cards);
        }

        private void attachCards(IEnumerable<Card> newCards)
        {
            foreach (Card c in newCards)
            {
                c.Owner = this;
                c.Modified += new Action(card_Modified);
            }
        }

        private void card_Modified()
        {
            _cardsModified++;
        }

        private void checkModifications()
        {
            if (_cardsModified > 0)
            {
                emitModified();
                _cardsModified = 0;
            }
        }

        private void detachCards(IEnumerable<Card> oldCards)
        {
            foreach (Card c in oldCards)
            {
                if (c.Owner == this)
                    c.Owner = null;
                c.Modified -= card_Modified;
            }
        }

        internal bool receiveCardsOnTop(IEnumerable<Card> cards)
        {
            if (!doAboutToReceiveCards(cards))
                return false;

            _cards.InsertRange(0, cards);
            attachCards(cards);
            emitModified();
            return true;
        }
        #endregion
    }

    static partial class Extensions
    {
        public static bool StackOnto(this IEnumerable<Card> cards, CardStack stack)
        {
            return stack.receiveCardsOnTop(cards);
        }
    }

}

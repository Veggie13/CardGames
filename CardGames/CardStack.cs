using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

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
            public CardEventArgs(CardStack stack, IEnumerable<Card> cards)
            {
                SourceStack = stack;
                Cards = cards;
            }

            public CardStack SourceStack { get; private set; }
            public IEnumerable<Card> Cards { get; private set; }
        }

        public class CancellableCardEventArgs : CardEventArgs
        {
            public CancellableCardEventArgs(CardStack stack, IEnumerable<Card> cards)
                : base(stack, cards)
            {
                Cancel = false;
            }

            public bool Cancel { get; set; }
        }

        class CardStackAction : ICardSequenceAction
        {
            private CardStack _source;
            private List<Tuple<int, Card>> _cards;

            public CardStackAction(CardStack source, IEnumerable<Tuple<int, Card>> cards)
            {
                _source = source;
                _cards = cards.ToList();
            }

            public ICardSequence SourceSequence
            {
                get { return _source; }
            }

            public IEnumerable<Card> Cards
            {
                get { return _cards.Select(t => t.Item2); }
            }

            public void Undo()
            {
                if (_source == null)
                    return;

                foreach (var tuple in _cards.OrderBy(t => t.Item1))
                {
                    _source._cards.Insert(tuple.Item1, tuple.Item2);
                }

                _source.emitModified();
                _source = null;
                _cards.Clear();
            }
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
        public delegate void CancellableCardEventHandler(CardStack stack, CancellableCardEventArgs e);
        public delegate void CardEventHandler(CardStack stack, CardEventArgs e);

        public event CancellableCardEventHandler AboutToDraw = delegate { };
        public event CardEventHandler CardsDrawn = delegate { };

        public event CancellableCardEventHandler AboutToReceiveCards = delegate { };
        public event CardEventHandler CardsReceived = delegate { };

        public event CancellableCardEventHandler AboutToActivate = delegate { };
        public event CardEventHandler Activated = delegate { };
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
            if (!stack.receiveCardsOnTop(this, this))
                return false;

            detachCards(_cards);
            _cards.Clear();
            emitModified();
            return true;
        }

        public bool Activate()
        {
            var e = new CancellableCardEventArgs(this, new Card[0]);
            AboutToActivate(this, e);
            if (e.Cancel)
                return false;

            Activated(this, e);
            return true;
        }
        #endregion

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
            if (0 == drawCards(dest, map.Select(t => t.Item2)))
            {
                return new CardStackAction(null, new Tuple<int, Card>[0]);
            }

            return new CardStackAction(this, map);
        }

        public ICardSequenceAction TryDrawFromTop(int count)
        {
            return TryDraw(Enumerable.Range(0, count).ToArray());
        }

        #region ICollection<Card>
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
        #endregion

        #region Helpers
        private void emitModified()
        {
            Modified();
        }

        private bool doAboutToReceiveCards(CancellableCardEventArgs e)
        {
            AboutToReceiveCards(this, e);
            return !e.Cancel;
        }

        private bool doAboutToDraw(CancellableCardEventArgs e)
        {
            AboutToDraw(this, e);
            return !e.Cancel;
        }

        private void emitCardsDrawn(CardEventArgs e)
        {
            CardsDrawn(this, e);
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

        private int drawCards(ICollection<Card> dest, IEnumerable<Card> drawn)
        {
            var e = new CancellableCardEventArgs(this, drawn);
            if (!doAboutToDraw(e))
                return 0;

            foreach (Card c in drawn)
            {
                _cards.Remove(c);
                detachCards(c);
                c.Grab();
                dest.Add(c);
            }

            if (drawn.Any())
            {
                emitCardsDrawn(e);
                emitModified();
            }

            return drawn.Count();
        }

        internal bool receiveCardsOnTop(CardStack origin, IEnumerable<Card> cards)
        {
            var e = new CancellableCardEventArgs(origin, cards);
            if (!doAboutToReceiveCards(e))
                return false;

            _cards.InsertRange(0, cards);
            attachCards(cards);
            CardsReceived(this, e);
            emitModified();
            return true;
        }

        private void throwIfInvalid(IEnumerable<int> indices)
        {
            if (indices.Any(i => i < 0 || i >= _cards.Count))
                throw new CardException("Invalid Index");
        }
        #endregion
    }

    static partial class Extensions
    {
        public static bool StackOnto(this IEnumerable<Card> cards, CardStack stack)
        {
            return stack.receiveCardsOnTop(null, cards);
        }
    }

}

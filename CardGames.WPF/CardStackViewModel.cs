using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace CardGames.WPF
{
    public class CardStackViewModel : INotifyPropertyChanged
    {
        private CardImageSet _cardImages;
        private CardStack _cardStack;
        private ObservableCollection<CardViewModel> _cardVMs = new ObservableCollection<CardViewModel>();

        public CardStackViewModel(CardImageSet cardImages, CardStack stack)
        {
            _cardImages = cardImages;
            _cardStack = stack;

            loadProperties();
            updateCards();

            _cardStack.Modified += new Action(_cardStack_Modified);
            _cardStack.CardsDrawn += new CardStack.CardEventHandler(_cardStack_CardsDrawn);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public CardStack Stack
        {
            get { return _cardStack; }
        }
        
        public IEnumerable<CardViewModel> Cards
        {
            get { return _cardVMs; }
        }

        private double _x;
        public double X
        {
            get { return _x; }
            internal set
            {
                _x = value;
                PropertyChanged(this, new PropertyChangedEventArgs("X"));
                updateCards();
            }
        }

        private double _y;
        public double Y
        {
            get { return _y; }
            internal set
            {
                _y = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Y"));
                updateCards();
            }
        }

        public int ZOrder
        {
            get;
            private set;
        }

        private bool _visible = true;
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Visible"));
            }
        }

        public double XFanFaceDown
        {
            get;
            internal set;
        }

        public double XFanFaceUp
        {
            get;
            internal set;
        }

        public double YFanFaceDown
        {
            get;
            internal set;
        }

        public double YFanFaceUp
        {
            get;
            internal set;
        }

        private void _cardStack_CardsDrawn(CardStack stack, CardStack.CardEventArgs e)
        {
            updateCards();
        }

        private void _cardStack_Modified()
        {
            updateCards();
        }

        private void updateCards()
        {
            int count = _cardStack.Count;

            _cardVMs.Clear();
            if (count == 0)
            {
                _cardVMs.Add(new CardViewModel(_cardImages, null)
                {
                    X = X,
                    Y = Y,
                    ZOrder = int.MinValue,
                    ParentStackViewModel = this
                });
            }

            double xFanTotal = 0;
            double yFanTotal = 0;
            foreach (var c in _cardStack.Reverse().Select((cc, i) => new CardViewModel(_cardImages, cc)
            {
                X = X + xFanTotal,
                Y = Y + yFanTotal,
                ZOrder = ZOrder + i + 1,
                ParentStackViewModel = this
            }))
            {
                xFanTotal += xFan(c.Card);
                yFanTotal += yFan(c.Card);
                _cardVMs.Add(c);
            }
        }

        private double xFan(Card c)
        {
            return c.Visibility == CardVisibility.FaceUp ? XFanFaceUp : XFanFaceDown;
        }

        private double yFan(Card c)
        {
            return c.Visibility == CardVisibility.FaceUp ? YFanFaceUp : YFanFaceDown;
        }

        private void loadProperties()
        {
            if (_cardStack.HasProperty("X"))
                X = (double)_cardStack["X"];
            if (_cardStack.HasProperty("Y"))
                Y = (double)_cardStack["Y"];
            if (_cardStack.HasProperty("Z"))
                ZOrder = (int)_cardStack["Z"];
            if (_cardStack.HasProperty("XFan"))
            {
                XFanFaceDown = (double)_cardStack["XFan"];
                XFanFaceUp = (double)_cardStack["XFan"];
            }
            if (_cardStack.HasProperty("XFanFaceDown"))
                XFanFaceDown = (double)_cardStack["XFanFaceDown"];
            if (_cardStack.HasProperty("XFanFaceUp"))
                XFanFaceUp = (double)_cardStack["XFanFaceUp"];
            if (_cardStack.HasProperty("YFan"))
            {
                YFanFaceDown = (double)_cardStack["YFan"];
                YFanFaceUp = (double)_cardStack["YFan"];
            }
            if (_cardStack.HasProperty("YFanFaceDown"))
                YFanFaceDown = (double)_cardStack["YFanFaceDown"];
            if (_cardStack.HasProperty("YFanFaceUp"))
                YFanFaceUp = (double)_cardStack["YFanFaceUp"];
        }
    }
}

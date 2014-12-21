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
            Visible = true;

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

        public double X
        {
            get;
            private set;
        }

        public double Y
        {
            get;
            private set;
        }

        public int ZOrder
        {
            get;
            private set;
        }

        public bool Visible
        {
            get;
            set;
        }

        public double XFanFaceDown
        {
            get
            {
                if (_cardStack.HasProperty("XFanFaceDown"))
                {
                    return (double)_cardStack["XFanFaceDown"];
                }
                return (double)_cardStack["XFan"];
            }
        }

        public double XFanFaceUp
        {
            get
            {
                if (_cardStack.HasProperty("XFanFaceUp"))
                {
                    return (double)_cardStack["XFanFaceUp"];
                }
                return (double)_cardStack["XFan"];
            }
        }

        public double YFanFaceDown
        {
            get
            {
                if (_cardStack.HasProperty("YFanFaceDown"))
                {
                    return (double)_cardStack["YFanFaceDown"];
                }
                return (double)_cardStack["YFan"];
            }
        }

        public double YFanFaceUp
        {
            get
            {
                if (_cardStack.HasProperty("YFanFaceUp"))
                {
                    return (double)_cardStack["YFanFaceUp"];
                }
                return (double)_cardStack["YFan"];
            }
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
                    ZOrder = int.MinValue
                });
            }

            double xFanTotal = 0;
            double yFanTotal = 0;
            foreach (var c in _cardStack.Reverse().Select((cc, i) => new CardViewModel(_cardImages, cc)
            {
                X = X + xFanTotal,
                Y = Y + yFanTotal,
                ZOrder = ZOrder + i + 1
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
        }
    }
}

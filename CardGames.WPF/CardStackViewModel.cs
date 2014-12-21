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
            get { return (double)_cardStack["X"]; }
        }

        public double Y
        {
            get { return (double)_cardStack["Y"]; }
        }

        public int ZOrder
        {
            get { return (int)_cardStack["Z"]; }
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
            _cardVMs.Clear();
            int count = _cardStack.Count;
            double xFanTotal = 0;
            double yFanTotal = 0;
            foreach (var c in _cardStack.Reverse().Select((cc, i) => new CardViewModel(_cardImages, cc)
            {
                X = X + xFanTotal,
                Y = Y + yFanTotal,
                ZOrder = ZOrder + i
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
    }
}

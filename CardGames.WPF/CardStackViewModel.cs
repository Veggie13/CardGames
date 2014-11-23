using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace CardGames.WPF
{
    class CardStackViewModel : INotifyPropertyChanged
    {
        private CardImageSet _cardImages;
        private CardStack _cardStack;
        private CardGameViewModel _parent;

        public CardStackViewModel(CardGameViewModel parent, CardImageSet cardImages, CardStack stack)
        {
            _parent = parent;
            _cardImages = cardImages;
            _cardStack = stack;

            _cardStack.Modified += new Action(_cardStack_Modified);
            _cardStack.AboutToDraw += new CardStack.CardPreEventHandler(_cardStack_AboutToDraw);
            _cardStack.CardsDrawn += new CardStack.CardPostEventHandler(_cardStack_CardsDrawn);
        }

        void _cardStack_AboutToDraw(CardStack stack, CardStack.CardEventArgs e)
        {
            
        }

        void _cardStack_CardsDrawn(CardStack stack, IEnumerable<Card> cards)
        {
            _parent.SetCardInHand(new CardViewModel(_cardImages, cards.First()) { X = X, Y = Y });
        }

        void _cardStack_Modified()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("TopCardImage"));
        }

        public void DrawToHand()
        {
            List<Card> cards = new List<Card>();
            _cardStack.DrawSequential(1, cards);
            cards[0].Flip();
        }

        public void Drop(CardViewModel card)
        {
            _cardStack.Insert(0, card.Card);
        }
        
        public BitmapSource TopCardImage
        {
            get { return _cardImages[_cardStack.Top]; }
        }

        private double _x = 0;
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                PropertyChanged(this, new PropertyChangedEventArgs("X"));
            }
        }

        private double _y = 0;
        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Y"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}

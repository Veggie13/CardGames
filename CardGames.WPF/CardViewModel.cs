using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CardGames.WPF
{
    public class CardViewModel : INotifyPropertyChanged, IEquatable<CardViewModel>
    {
        private CardImageSet _cardImages;

        public CardViewModel(CardImageSet cardImages, Card card)
        {
            _cardImages = cardImages;
            _card = card;

            _card.Modified += new Action(_card_Modified);
        }

        void _card_Modified()
        {
            PropertyChanged(this, new PropertyChangedEventArgs("CardImage"));
        }

        private Card _card;
        public Card Card
        {
            get { return _card; }
        }

        public BitmapSource CardImage
        {
            get { return _cardImages[_card]; }
        }

        private double _x = 0f;
        public double X
        {
            get { return _x; }
            set
            {
                _x = value;
                PropertyChanged(this, new PropertyChangedEventArgs("X"));
            }
        }

        private double _y = 0f;
        public double Y
        {
            get { return _y; }
            set
            {
                _y = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Y"));
            }
        }

        private int _z = 0;
        public int ZOrder
        {
            get { return _z; }
            set
            {
                _z = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ZOrder"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public override bool Equals(object obj)
        {
            return Equals(obj as CardViewModel);
        }

        public bool Equals(CardViewModel other)
        {
            if (other == null)
                return false;
            return _card.Equals(other._card);
        }

        public override int GetHashCode()
        {
            return _card.GetHashCode();
        }
    }
}

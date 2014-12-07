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
            
            updateCards();

            _cardStack.Modified += new Action(_cardStack_Modified);
            _cardStack.CardsDrawn += new CardStack.CardEventHandler(_cardStack_CardsDrawn);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

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
            foreach (var c in _cardStack.Select((cc, i) => new CardViewModel(_cardImages, cc)
            {
                X = X,
                Y = Y,
                ZOrder = -i
            }))
            {
                _cardVMs.Add(c);
            }
        }
    }
}

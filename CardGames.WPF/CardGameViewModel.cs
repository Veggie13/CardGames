using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CardGames.WPF
{
    public class CardGameViewModel : INotifyPropertyChanged
    {
        private Deck _deck = new Deck(false);
        private CardImageSet _cardImages = new CardImageSet();

        public CardGameViewModel()
        {
            _cards.Add(new CardStackViewModel(this, _cardImages, _deck) { X = 10, Y = 10 });
            _cards.Add(new CardStackViewModel(this, _cardImages, new CardStack("Discard")) { X = 150, Y = 10 });
        }

        private ObservableCollection<object> _cards = new ObservableCollection<object>();
        public IEnumerable<object> Cards
        {
            get { return _cards; }
        }

        internal void SetCardInHand(CardViewModel card)
        {
            _cards.Add(card);
        }

        internal void RemoveCard(CardViewModel card)
        {
            _cards.Remove(card);
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}

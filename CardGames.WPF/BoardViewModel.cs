using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace CardGames.WPF
{
    public class BoardViewModel : INotifyPropertyChanged
    {
        private Board _board;
        private ObservableCollection<CardStackViewModel> _stacks;
        private CardImageSet _cardImages = new CardImageSet();

        public BoardViewModel(Board board)
        {
            _board = board;
            var stackVMs = _board.Stacks.Select(s => new CardStackViewModel(_cardImages, s));
            _stacks = new ObservableCollection<CardStackViewModel>(stackVMs);
        }

        public IEnumerable<CardStackViewModel> Stacks
        {
            get { return _stacks; }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };
    }
}

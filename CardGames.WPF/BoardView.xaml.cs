using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace CardGames.WPF
{
    /// <summary>
    /// Interaction logic for CardGameView.xaml
    /// </summary>
    public partial class BoardView : UserControl, INotifyPropertyChanged
    {
        private double _dx, _dy;

        public BoardView()
        {
            InitializeComponent();
        }

        private BoardViewModel _viewModel;
        public BoardViewModel ViewModel
        {
            get { return _viewModel; }
            set
            {
                _mainGrid.DataContext = _viewModel = value;
                PropertyChanged(this, new PropertyChangedEventArgs("ViewModel"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if (rect == null)
                return;

            CardViewModel vm = rect.Tag as CardViewModel;
            var location = e.GetPosition(null);
            _dx = location.X - vm.X;
            _dy = location.Y - vm.Y;
            vm.ZOrder *= -1;
        }

        private void Card_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Rectangle rect = sender as Rectangle;
                if (rect == null)
                    return;

                CardViewModel vm = rect.Tag as CardViewModel;
                var location = e.GetPosition(null);
                vm.X = location.X - _dx;
                vm.Y = location.Y - _dy;
            }
        }

        private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if (rect == null)
                return;
            /*
            CardViewModel vm = rect.Tag as CardViewModel;
            ViewModel.RemoveCard(vm);
            var location = e.GetPosition(null);

            foreach (CardStackViewModel stack in ViewModel.Cards)
            {
                Rect r = new Rect(stack.X, stack.Y, stack.TopCardImage.Width, stack.TopCardImage.Height);
                if (r.Contains(location))
                {
                    stack.Drop(vm);
                    return;
                }
            }*/
        }

        private void Stack_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if (rect == null)
                return;
            /*
            CardStackViewModel vm = rect.Tag as CardStackViewModel;
            vm.DrawToHand();
            var location = e.GetPosition(null);
            _dx = location.X - vm.X;
            _dy = location.Y - vm.Y;*/
        }
    }
}

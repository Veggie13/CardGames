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
        private ICardSequenceAction _action;
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

            if (vm.ParentStackViewModel.Stack.Activate())
                return;

            int cardPos = vm.ParentStackViewModel.Stack.IndexOf(vm.Card);
            _action = vm.ParentStackViewModel.Stack.TryDrawFromTop(cardPos + 1);

            if (!_action.Cards.Any())
            {
                _action = null;
                return;
            }

            ViewModel.Hand.Stack.Source = vm.ParentStackViewModel.Stack;
            _action.Cards.StackOnto(ViewModel.Hand.Stack);

            var location = e.GetPosition(null);
            _dx = location.X - vm.X;
            _dy = location.Y - vm.Y;

            ViewModel.Hand.X = vm.X;
            ViewModel.Hand.Y = vm.Y;
            ViewModel.Hand.Visible = true;
        }

        private void Card_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Rectangle rect = sender as Rectangle;
                if (rect == null || _action == null)
                    return;

                //CardViewModel vm = rect.Tag as CardViewModel;
                var location = e.GetPosition(null);
                ViewModel.Hand.X = location.X - _dx;
                ViewModel.Hand.Y = location.Y - _dy;
            }
        }

        private void Card_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Rectangle rect = sender as Rectangle;
            if (rect == null || _action == null)
                return;

            var handAction = ViewModel.Hand.Stack.TryDrawFromTop(ViewModel.Hand.Stack.Count);
            
            var location = e.GetPosition(null);
            var dropStack = stackUnder(location);
            if (dropStack == null || !handAction.Cards.StackOnto(dropStack.Stack))
            {
                _action.Undo();
            }

            _action = null;
            ViewModel.Hand.Visible = false;
        }

        private CardStackViewModel stackUnder(Point location)
        {
            foreach (CardStackViewModel stack in ViewModel.Stacks.Except(new[] { ViewModel.Hand }))
            {
                var topCard = stack.Cards.First();
                Rect r = new Rect(topCard.X, topCard.Y, topCard.CardImage.Width, topCard.CardImage.Height);
                if (r.Contains(location))
                {
                    return stack;
                }
            }
            return null;
        }
    }
}

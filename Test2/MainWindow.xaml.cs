﻿using System;
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

namespace Test2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SolitaireGame _game;

        public MainWindow()
        {
            InitializeComponent();

            _game = new SolitaireGame();
            _game.Initialize();

            _game.Board["draw"]["X"] = 0.0;
            _game.Board["draw"]["Y"] = 0.0;
            _game.Board["draw"]["Z"] = 0;
            _game.Board["draw"]["XFan"] = 0.25;
            _game.Board["draw"]["YFan"] = 0.25;

            _game.Board["discard"]["X"] = 100.0;
            _game.Board["discard"]["Y"] = 0.0;
            _game.Board["discard"]["Z"] = 0;
            _game.Board["discard"]["XFan"] = 10.0;
            _game.Board["discard"]["YFan"] = 0.0;

            _game.Board["top1"]["X"] = 300.0;
            _game.Board["top1"]["Y"] = 0.0;
            _game.Board["top1"]["Z"] = 0;
            _game.Board["top1"]["XFan"] = 0.0;
            _game.Board["top1"]["YFan"] = 0.0;

            _game.Board["top2"]["X"] = 400.0;
            _game.Board["top2"]["Y"] = 0.0;
            _game.Board["top2"]["Z"] = 0;
            _game.Board["top2"]["XFan"] = 0.0;
            _game.Board["top2"]["YFan"] = 0.0;

            _game.Board["top3"]["X"] = 500.0;
            _game.Board["top3"]["Y"] = 0.0;
            _game.Board["top3"]["Z"] = 0;
            _game.Board["top3"]["XFan"] = 0.0;
            _game.Board["top3"]["YFan"] = 0.0;

            _game.Board["top4"]["X"] = 600.0;
            _game.Board["top4"]["Y"] = 0.0;
            _game.Board["top4"]["Z"] = 0;
            _game.Board["top4"]["XFan"] = 0.0;
            _game.Board["top4"]["YFan"] = 0.0;

            _game.Board["pile1"]["X"] = 0.0;
            _game.Board["pile1"]["Y"] = 150.0;
            _game.Board["pile1"]["Z"] = 0;
            _game.Board["pile1"]["XFan"] = 0.0;
            _game.Board["pile1"]["YFanFaceDown"] = 1.0;
            _game.Board["pile1"]["YFanFaceUp"] = 30.0;

            _game.Board["pile2"]["X"] = 100.0;
            _game.Board["pile2"]["Y"] = 150.0;
            _game.Board["pile2"]["Z"] = 0;
            _game.Board["pile2"]["XFan"] = 0.0;
            _game.Board["pile2"]["YFanFaceDown"] = 1.0;
            _game.Board["pile2"]["YFanFaceUp"] = 30.0;

            _game.Board["pile3"]["X"] = 200.0;
            _game.Board["pile3"]["Y"] = 150.0;
            _game.Board["pile3"]["Z"] = 0;
            _game.Board["pile3"]["XFan"] = 0.0;
            _game.Board["pile3"]["YFanFaceDown"] = 1.0;
            _game.Board["pile3"]["YFanFaceUp"] = 30.0;

            _game.Board["pile4"]["X"] = 300.0;
            _game.Board["pile4"]["Y"] = 150.0;
            _game.Board["pile4"]["Z"] = 0;
            _game.Board["pile4"]["XFan"] = 0.0;
            _game.Board["pile4"]["YFanFaceDown"] = 1.0;
            _game.Board["pile4"]["YFanFaceUp"] = 30.0;

            _game.Board["pile5"]["X"] = 400.0;
            _game.Board["pile5"]["Y"] = 150.0;
            _game.Board["pile5"]["Z"] = 0;
            _game.Board["pile5"]["XFan"] = 0.0;
            _game.Board["pile5"]["YFanFaceDown"] = 1.0;
            _game.Board["pile5"]["YFanFaceUp"] = 30.0;

            _game.Board["pile6"]["X"] = 500.0;
            _game.Board["pile6"]["Y"] = 150.0;
            _game.Board["pile6"]["Z"] = 0;
            _game.Board["pile6"]["XFan"] = 0.0;
            _game.Board["pile6"]["YFanFaceDown"] = 1.0;
            _game.Board["pile6"]["YFanFaceUp"] = 30.0;

            _game.Board["pile7"]["X"] = 600.0;
            _game.Board["pile7"]["Y"] = 150.0;
            _game.Board["pile7"]["Z"] = 0;
            _game.Board["pile7"]["XFan"] = 0.0;
            _game.Board["pile7"]["YFanFaceDown"] = 1.0;
            _game.Board["pile7"]["YFanFaceUp"] = 30.0;

            _game.Deal();

            _boardView.ViewModel = new CardGames.WPF.BoardViewModel(_game.Board);
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CardGames.WinForms;

namespace Solitaire
{
    public partial class Form1 : Form
    {
        Solitaire game;
        CardGameControl gameControl;

        public Form1()
        {
            InitializeComponent();

            gameControl = new CardGameControl();
            gameControl.Dock = DockStyle.Fill;
            panel1.Controls.Add(gameControl);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            game = new Solitaire(gameControl);

            panel1.Invalidate();
        }
    }
}

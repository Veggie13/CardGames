using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CardGames.WinForms
{
    public partial class CardGameControl : UserControl
    {
        private Renderer _renderer = new Renderer();

        private Game _game;
        public Game Game
        {
            get { return _game; }
            set
            {
                if (_game != null)
                {
                    _game.Modified -= _game_Modified;
                }

                _game = value;

                if (_game != null)
                {
                    _game.Modified += new Action(_game_Modified);
                }
            }
        }

        private float _xFanAmt = 0f;
        public float HorizontalFan
        {
            get { return _xFanAmt; }
            set
            {
                _xFanAmt = value;
                _game_Modified();
            }
        }

        private float _yFanAmt = 0f;
        public float VerticalFan
        {
            get { return _yFanAmt; }
            set
            {
                _yFanAmt = value;
                _game_Modified();
            }
        }

        private void _game_Modified()
        {
            Invoke(new Action(() => { this.Invalidate(); }));
        }

        public CardGameControl()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
        }

        private void CardGameControl_SizeChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private IEnumerable<Card> _draggingCards = null;
        private Game.LayoutElement _draggingSource = null;
        private void CardGameControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (_draggingCards != null)
                return;

            double x = (double)e.Location.X / DisplayRectangle.Width;
            double y = (double)e.Location.Y / DisplayRectangle.Height;

            var element = this.Game.TableLayout.DeepestElementAt(x, y);
            if (element == null)
                return;
            _draggingCards = element.BeginGrab();
            if (_draggingCards == null)
                return;
            _draggingSource = element;
        }

        private void CardGameControl_Paint(object sender, PaintEventArgs e)
        {
            _renderer.Graphics = e.Graphics;
            _renderer.Window = this.DisplayRectangle;

            if (this.Game != null)
            {
                this.Game.TableLayout.BasicWidth = _renderer.BasicWidth;
                this.Game.TableLayout.BasicHeight = _renderer.BasicHeight;
                this.Game.TableLayout.Render(0.5, 0.5, 1, 1, _renderer);

                if (_draggingCards != null)
                {
                    Point clientPt = this.PointToClient(MousePosition);
                    double x = (double)clientPt.X / DisplayRectangle.Width;
                    double y = (double)clientPt.Y / DisplayRectangle.Height;

                    _renderer.Render(x, y, _xFanAmt, _yFanAmt, _draggingCards);
                }
            }
        }

        private void CardGameControl_Load(object sender, EventArgs e)
        {
            this.Paint += new PaintEventHandler(CardGameControl_Paint);
            this.MouseDown += new MouseEventHandler(CardGameControl_MouseDown);
            this.MouseUp += new MouseEventHandler(CardGameControl_MouseUp);
            this.MouseMove += new MouseEventHandler(CardGameControl_MouseMove);
            this.SizeChanged += new EventHandler(CardGameControl_SizeChanged);
            this.MouseClick += new MouseEventHandler(CardGameControl_MouseClick);
        }

        private void CardGameControl_MouseClick(object sender, MouseEventArgs e)
        {
            double x = (double)e.Location.X / DisplayRectangle.Width;
            double y = (double)e.Location.Y / DisplayRectangle.Height;

            var element = this.Game.TableLayout.DeepestElementAt(x, y);
            if (element == null)
                return;
            
            element.Activate();
        }

        private void CardGameControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_draggingCards == null)
                return;

            this.Invalidate();
        }

        private void CardGameControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (_draggingCards == null)
                return;

            double x = (double)e.Location.X / DisplayRectangle.Width;
            double y = (double)e.Location.Y / DisplayRectangle.Height;

            var element = this.Game.TableLayout.DeepestElementAt(x, y);
            bool result = (element != null) ? element.Drop(_draggingCards) : false;
            _draggingSource.FinishGrab(result);

            _draggingCards = null;
            _draggingSource = null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace CardGames.WinForms
{
    public class Renderer : ICardRenderer
    {
        private Dictionary<Suit, Image[]> _suits = new Dictionary<Suit, Image[]>();
        private Image _back;
        private Image _empty;

        public Renderer()
        {
            var cards = Properties.Resources.cards;
            Color transparent = cards.GetPixel(0, 0);

            int cardWidth = cards.Width / 14;
            int cardHeight = cards.Height / 4;

            _suits[Suit.Hearts] = new Image[13];
            _suits[Suit.Diamonds] = new Image[13];
            _suits[Suit.Clubs] = new Image[13];
            _suits[Suit.Spades] = new Image[13];

            for (int i = 0; i < 13; i++)
            {
                Bitmap newCard = new Bitmap(cardWidth, cardHeight);
                using (Graphics g = Graphics.FromImage(newCard))
                {
                    g.DrawImage(cards, new Rectangle(0, 0, cardWidth, cardHeight), new Rectangle(i * cardWidth, 0, cardWidth, cardHeight), GraphicsUnit.Pixel);
                }
                newCard.MakeTransparent(transparent);
                _suits[Suit.Hearts][i] = newCard;

                newCard = new Bitmap(cardWidth, cardHeight);
                using (Graphics g = Graphics.FromImage(newCard))
                {
                    g.DrawImage(cards, new Rectangle(0, 0, cardWidth, cardHeight), new Rectangle(i * cardWidth, cardHeight, cardWidth, cardHeight), GraphicsUnit.Pixel);
                }
                newCard.MakeTransparent(transparent);
                _suits[Suit.Diamonds][i] = newCard;

                newCard = new Bitmap(cardWidth, cardHeight);
                using (Graphics g = Graphics.FromImage(newCard))
                {
                    g.DrawImage(cards, new Rectangle(0, 0, cardWidth, cardHeight), new Rectangle(i * cardWidth, 2 * cardHeight, cardWidth, cardHeight), GraphicsUnit.Pixel);
                }
                newCard.MakeTransparent(transparent);
                _suits[Suit.Clubs][i] = newCard;

                newCard = new Bitmap(cardWidth, cardHeight);
                using (Graphics g = Graphics.FromImage(newCard))
                {
                    g.DrawImage(cards, new Rectangle(0, 0, cardWidth, cardHeight), new Rectangle(i * cardWidth, 3 * cardHeight, cardWidth, cardHeight), GraphicsUnit.Pixel);
                }
                newCard.MakeTransparent(transparent);
                _suits[Suit.Spades][i] = newCard;
            }

            var back = new Bitmap(cardWidth, cardHeight);
            using (Graphics g = Graphics.FromImage(back))
            {
                g.DrawImage(cards, new Rectangle(0, 0, cardWidth, cardHeight), new Rectangle(13 * cardWidth, 2 * cardHeight, cardWidth, cardHeight), GraphicsUnit.Pixel);
            }
            back.MakeTransparent(transparent);
            _back = back;

            var empty = new Bitmap(cardWidth, cardHeight);
            using (Graphics g = Graphics.FromImage(empty))
            {
                g.DrawImage(cards, new Rectangle(0, 0, cardWidth, cardHeight), new Rectangle(13 * cardWidth, 3 * cardHeight, cardWidth, cardHeight), GraphicsUnit.Pixel);
            }
            empty.MakeTransparent(transparent);
            _empty = empty;
        }

        public float BasicWidth
        {
            get
            {
                return (float)_empty.Width * 1.1f / Window.Width;
            }
        }

        public float BasicHeight
        {
            get
            {
                return (float)_empty.Height * 1.1f / Window.Height;
            }
        }

        public Rectangle Window { get; set; }

        public Graphics Graphics { get; set; }

        #region ICardRenderer
        public void Render(double originX, double originY, Card c)
        {
            var cardImage = (c.Visibility == CardVisibility.FaceUp || (c.Owner == null && c.Visibility == CardVisibility.PlayerHand)) ?
                _suits[c.Suit][(int)c.FaceValue - 1] :
                _back;

            drawCardImage(cardImage, originX, originY, 0, 0);
        }

        public void Render(double originX, double originY, double xFanAmt, double yFanAmt, CardStack stack)
        {
            if (stack.Count == 0)
            {
                drawCardImage(_empty, originX, originY, 0, 0);
            }
            else if (stack.Count == 1 || xFanAmt > 0 || yFanAmt > 0)
            {
                double x = originX, y = originY;
                foreach (Card c in stack.Reverse())
                {
                    var cardImage = c.Visibility == CardVisibility.FaceUp ?
                        _suits[c.Suit][(int)c.FaceValue - 1] :
                        _back;

                    drawCardImage(cardImage, x, y, 0, 0);

                    x += xFanAmt;
                    y += yFanAmt;
                }
            }
            else if (stack.Count < 4)
            {
                Card c = stack.Top;
                var cardImage = c.Visibility == CardVisibility.FaceUp ?
                    _suits[c.Suit][(int)c.FaceValue - 1] :
                    _back;
                drawCardImage(_back, originX, originY, -1, 1);
                drawCardImage(cardImage, originX, originY, 1, -1);
            }
            else
            {
                Card c = stack.Top;
                var cardImage = c.Visibility == CardVisibility.FaceUp ?
                    _suits[c.Suit][(int)c.FaceValue - 1] :
                    _back;
                drawCardImage(_back, originX, originY, -3, 3);
                drawCardImage(_back, originX, originY, -1, 1);
                drawCardImage(_back, originX, originY, 1, -1);
                drawCardImage(cardImage, originX, originY, 3, -3);
            }
        }
        #endregion

        private void coordConvert(double x, double y, out int cx, out int cy)
        {
            cx = (int)Math.Round(x * Window.Width);
            cy = (int)Math.Round(y * Window.Height);
        }

        private void drawCardImage(Image cardImage, double x, double y, int dx, int dy)
        {
            int cx, cy;
            coordConvert(x, y, out cx, out cy);
            cx += dx - _empty.Width / 2;
            cy += dy - _empty.Height / 2;

            this.Graphics.DrawImage(cardImage, cx, cy);
        }
    }
}

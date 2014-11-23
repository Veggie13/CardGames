using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Windows.Data;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CardGames.WPF
{
    public class CardImageSet
    {
        private Dictionary<Suit, Bitmap[]> _suits = new Dictionary<Suit, Bitmap[]>();
        private Bitmap _back;
        private Bitmap _empty;

        public CardImageSet()
        {
            var cards = Properties.Resources.cards;
            Color transparent = cards.GetPixel(0, 0);

            int cardWidth = cards.Width / 14;
            int cardHeight = cards.Height / 4;

            _suits[Suit.Hearts] = new Bitmap[13];
            _suits[Suit.Diamonds] = new Bitmap[13];
            _suits[Suit.Clubs] = new Bitmap[13];
            _suits[Suit.Spades] = new Bitmap[13];

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

        public int CardWidth
        {
            get
            {
                return _empty.Width;
            }
        }

        public int CardHeight
        {
            get
            {
                return _empty.Height;
            }
        }

        public BitmapSource this[Card card]
        {
            get
            {
                if (card == null)
                {
                    return _empty.ToBitmapSource();
                }

                switch (card.Visibility)
                {
                    default:
                    case CardVisibility.FaceDown:
                        return _back.ToBitmapSource();
                    case CardVisibility.FaceUp:
                        return _suits[card.Suit][(int)card.FaceValue - 1].ToBitmapSource();
                }
            }
        }

        public Image CardBack
        {
            get { return _back; }
        }

        public Image Empty
        {
            get { return _empty; }
        }
    }

    class CardConverter : IValueConverter
    {
        public CardConverter()
        {
            Cards = new CardImageSet();
        }

        public CardImageSet Cards
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!targetType.IsAssignableFrom(typeof(BitmapSource)) || !(value is Card))
            {
                return Binding.DoNothing;
            }
            return Cards[value as Card];
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }

    static class Extensions
    {
        [DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        public static BitmapSource ToBitmapSource(this Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("bitmap");

            IntPtr hBitmap = bitmap.GetHbitmap();

            try
            {
                return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }
        }
    }
}

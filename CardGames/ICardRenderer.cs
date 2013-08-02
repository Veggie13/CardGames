using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGames
{
    public interface ICardRenderer
    {
        void Render(double originX, double originY, Card c);
        void Render(double originX, double originY, double xFanAmt, double yFanAmt, CardStack stack);
    }
}

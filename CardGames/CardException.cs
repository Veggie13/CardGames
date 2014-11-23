using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGames
{
    public class CardException : Exception
    {
        public CardException(string msg)
            : base(msg)
        {
        }
    }
}

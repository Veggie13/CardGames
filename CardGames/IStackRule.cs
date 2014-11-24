using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGames
{
    public interface IStackRule
    {
        bool CanDraw(CardStack stack, IEnumerable<Card> cards);
        bool CanDrop(CardStack source, CardStack stack, IEnumerable<Card> cards);
        bool CanActivate(CardStack stack);

        void OnDraw(CardStack stack, IEnumerable<Card> cards);
        void OnDrop(CardStack source, CardStack stack, IEnumerable<Card> cards);
        void OnActivation(CardStack stack);
    }
}

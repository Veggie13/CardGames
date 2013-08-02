using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGames;
using CardGames.WinForms;

namespace Solitaire
{
    class Solitaire
    {
        private static string GameDef { get { return Properties.Resources.Solitaire; } }

        private Game game = Game.ParseDefinition(GameDef);

        private Deck orig = new Deck(false);
        private CardStack deck, discard, suit1, suit2, suit3, suit4;

        public Solitaire(CardGameControl ctrl)
        {
            ctrl.Game = game;

            deck = get("deck");
            discard = get("discard");
            suit1 = get("suit1");
            suit2 = get("suit2");
            suit3 = get("suit3");
            suit4 = get("suit4");

            orig.StackOnto(deck);
            deck.Activated += new CardStack.CardPostEventHandler(deck_Activated);

            suit1.AboutToReceiveCards += new CardStack.CardPreEventHandler(suit_AboutToReceiveCards);
            suit2.AboutToReceiveCards += new CardStack.CardPreEventHandler(suit_AboutToReceiveCards);
            suit3.AboutToReceiveCards += new CardStack.CardPreEventHandler(suit_AboutToReceiveCards);
            suit4.AboutToReceiveCards += new CardStack.CardPreEventHandler(suit_AboutToReceiveCards);
        }

        private void suit_AboutToReceiveCards(CardStack stack, CardStack.CardEventArgs e)
        {
            if (stack.Count == 0 && e.Cards.First().FaceValue != Face.Ace)
                e.Cancel = true;
            else if (stack.Count > 0 && (e.Cards.First().Suit != stack.Top.Suit || e.Cards.First().FaceValue != stack.Top.FaceValue + 1))
                e.Cancel = true;
        }

        private void killEvent(CardStack stack, CardStack.CardEventArgs e)
        {
            e.Cancel = true;
        }

        private void deck_Activated(CardStack stack, IEnumerable<Card> cards)
        {
            List<Card> drawn = new List<Card>();
            if (0 == deck.DrawSequential(3, drawn))
            {
                discard.Flip();
                discard.StackOnto(deck);
            }
            else
            {
                foreach (Card c in drawn)
                {
                    c.Flip();
                    c.StackOnto(discard);
                }
            }
        }

        private CardStack get(string stackName)
        {
            return Game.LayoutElement.Get<Game.CardStackElement>(stackName).CardStack;
        }
    }
}

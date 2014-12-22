using CardGames;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test2
{
    class DrawRule : IStackRule
    {
        private bool _inActivation = false;
        private CardStack _temp;

        public static void Register()
        {
            StackRuleManager.Register<DrawRule>("draw");
        }

        private Board _board;
        public Board Board
        {
            get { return _board; }
            set
            {
                _board = value;
                _temp = new CardStack("drawTemp", _board["draw"]);
            }
        }

        private CardStack Discard { get { return Board["discard"]; } }

        public bool CanDraw(CardStack stack, IEnumerable<Card> cards)
        {
            return _inActivation;
        }

        public bool CanDrop(CardStack source, CardStack stack, IEnumerable<Card> cards)
        {
            return _inActivation;
        }

        public bool CanActivate(CardStack stack)
        {
            return true;
        }

        public void OnDraw(CardStack stack, IEnumerable<Card> cards)
        {
        }

        public void OnDrop(CardStack source, CardStack stack, IEnumerable<Card> cards)
        {
        }

        public void OnActivation(CardStack stack)
        {
            _inActivation = true;
            if (stack.Any())
            {
                stack.DrawFromTop(_temp, 3);
                _temp.Flip();
                _temp.StackOnto(Discard);
            }
            else
            {
                Discard.StackOnto(stack);
                stack.Flip();
            }
            _inActivation = false;
        }
    }

    class DiscardRule : IStackRule
    {
        public static void Register()
        {
            StackRuleManager.Register<DiscardRule>("discard");
        }

        public Board Board { get; set; }

        private CardStack Draw { get { return Board["draw"]; } }

        public bool CanDraw(CardStack stack, IEnumerable<Card> cards)
        {
            if (cards.Skip(1).Any() || cards.First() != stack.Top)
                return false;
            return true;
        }

        public bool CanDrop(CardStack source, CardStack stack, IEnumerable<Card> cards)
        {
            return (source == Draw);
        }

        public bool CanActivate(CardStack stack)
        {
            return false;
        }

        public void OnDraw(CardStack stack, IEnumerable<Card> cards)
        {
        }

        public void OnDrop(CardStack source, CardStack stack, IEnumerable<Card> cards)
        {
        }

        public void OnActivation(CardStack stack)
        {
        }
    }

    class TopRule : IStackRule
    {
        public static void Register()
        {
            StackRuleManager.Register<TopRule>("top");
        }

        public Board Board { get; set; }

        public bool CanDraw(CardStack stack, IEnumerable<Card> cards)
        {
            if (cards.Skip(1).Any() || cards.First() != stack.Top)
                return false;
            return true;
        }

        public bool CanDrop(CardStack source, CardStack stack, IEnumerable<Card> cards)
        {
            if (cards.Skip(1).Any())
                return false;
            if (stack.Any()
                && stack.Top.Suit == cards.First().Suit
                && (int)stack.Top.FaceValue + 1 == (int)cards.First().FaceValue)
            {
                return true;
            }
            if (cards.First().FaceValue == Face.Ace)
            {
                return true;
            }
            return false;
        }

        public bool CanActivate(CardStack stack)
        {
            return false;
        }

        public void OnDraw(CardStack stack, IEnumerable<Card> cards)
        {
        }

        public void OnDrop(CardStack source, CardStack stack, IEnumerable<Card> cards)
        {
        }

        public void OnActivation(CardStack stack)
        {
        }
    }

    class PileRule : IStackRule
    {
        public static void Register()
        {
            StackRuleManager.Register<PileRule>("pile");
        }

        public Board Board { get; set; }

        public bool CanDraw(CardStack stack, IEnumerable<Card> cards)
        {
            var topN = stack.Take(cards.Count());
            if (topN.SequenceEqual(cards) && cards.All(c => c.Visibility == CardVisibility.FaceUp))
            {
                return true;
            }
            return false;
        }

        public bool CanDrop(CardStack source, CardStack stack, IEnumerable<Card> cards)
        {
            if (!stack.Any())
            {
                return (cards.Last().FaceValue == Face.King);
            }
            if (stack.Top.Colour != cards.Last().Colour
                && (int)stack.Top.FaceValue - (int)cards.Last().FaceValue == 1)
            {
                return true;
            }
            return false;
        }

        public bool CanActivate(CardStack stack)
        {
            return stack.Any() && stack.First().Visibility == CardVisibility.FaceDown;
        }

        public void OnDraw(CardStack stack, IEnumerable<Card> cards)
        {
        }

        public void OnDrop(CardStack source, CardStack stack, IEnumerable<Card> cards)
        {
        }

        public void OnActivation(CardStack stack)
        {
            stack.FlipTop();
        }
    }

    class SolitaireGame : Game
    {
        static SolitaireGame()
        {
            DrawRule.Register();
            DiscardRule.Register();
            TopRule.Register();
            PileRule.Register();
        }

        public override IEnumerable<Tuple<string, string>> StackDefinitions
        {
            get
            {
                yield return new Tuple<string, string>("draw", "draw");
                yield return new Tuple<string, string>("discard", "discard");
                yield return new Tuple<string, string>("top1", "top");
                yield return new Tuple<string, string>("top2", "top");
                yield return new Tuple<string, string>("top3", "top");
                yield return new Tuple<string, string>("top4", "top");
                yield return new Tuple<string, string>("pile1", "pile");
                yield return new Tuple<string, string>("pile2", "pile");
                yield return new Tuple<string, string>("pile3", "pile");
                yield return new Tuple<string, string>("pile4", "pile");
                yield return new Tuple<string, string>("pile5", "pile");
                yield return new Tuple<string, string>("pile6", "pile");
                yield return new Tuple<string, string>("pile7", "pile");
            }
        }

        protected override void onDeal()
        {
            Deck.Shuffle();
            for (int i = 1; i <= 7; i++)
            {
                string name = "pile" + i.ToString();
                Deck.DrawFromTop(Board[name], i);
                Board[name].FlipTop();
            }
            Deck.StackOnto(Board["draw"]);
        }
    }
}

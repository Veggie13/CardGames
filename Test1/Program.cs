using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGames;

namespace Test1
{
    class DrawRule : IStackRule
    {
        private bool _inActivation = false;
        private CardStack _temp;

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
            if (stack.Top.Colour != cards.Last().Colour
                && (int)cards.Last().FaceValue - (int)stack.Top.FaceValue == 1)
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
            if (stack.Top.Visibility == CardVisibility.FaceDown)
                stack.Top.Flip();
        }

        public void OnDrop(CardStack source, CardStack stack, IEnumerable<Card> cards)
        {
        }

        public void OnActivation(CardStack stack)
        {
        }
    }

    static class Program
    {
        static void Main(string[] args)
        {
            /*Deck deck = new Deck(false);
            CardStack cs1 = new CardStack("cs1");
            deck.StackOnto(cs1);

            cs1.Shuffle();
            foreach (Card c in cs1)
                c.PrintOut();
             * */
            string gameDef = @"
                <cardgame name='test'>
                    <layout>
                        <row>
                            <card name='single' />
                            <null />
                        </row>
                        <row>
                            <null />
                            <stack name='pile' />
                        </row>
                    </layout>
                    <styles>
                        <row type='auto' value='0' />
                        <row type='grow' value='1' />
                        <col type='grow' value='1' />
                        <col type='fixed' value='0.15' />
                    </styles>
                </cardgame>
            ";
            //Game game = Game.ParseDefinition(gameDef);

            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("draw");
            board.AddStack("discard");
            board.AddStack("top1");
            board.AddStack("top2");
            board.AddStack("top3");
            board.AddStack("top4");
            board.AddStack("pile1");
            board.AddStack("pile2");
            board.AddStack("pile3");
            board.AddStack("pile4");
            board.AddStack("pile5");
            board.AddStack("pile6");
            board.AddStack("pile7");

            deck.Shuffle();
            for (int i = 1; i <= 7; i++)
            {
                string name = "pile" + i.ToString();
                deck.DrawFromTop(board[name], i);
                board[name].FlipTop();
            }
            deck.StackOnto(board["draw"]);

            board.SetStackRule("draw", new DrawRule() { Board = board });
            board.SetStackRule("discard", new DiscardRule() { Board = board });
            board.SetStackRule("top1", new TopRule() { Board = board });
            board.SetStackRule("top2", new TopRule() { Board = board });
            board.SetStackRule("top3", new TopRule() { Board = board });
            board.SetStackRule("top4", new TopRule() { Board = board });
            board.SetStackRule("pile1", new PileRule() { Board = board });
            board.SetStackRule("pile2", new PileRule() { Board = board });
            board.SetStackRule("pile3", new PileRule() { Board = board });
            board.SetStackRule("pile4", new PileRule() { Board = board });
            board.SetStackRule("pile5", new PileRule() { Board = board });
            board.SetStackRule("pile6", new PileRule() { Board = board });
            board.SetStackRule("pile7", new PileRule() { Board = board });

            bool result = board["draw"].Activate();

            //Console.WriteLine(cs1.Count.ToString());
            Console.Read();
        }

        static Dictionary<Suit, string> SuitNames = new Dictionary<Suit, string>();
        static Dictionary<Face, string> FaceNames = new Dictionary<Face, string>();

        static Program()
        {
            SuitNames[Suit.Clubs] = "C";
            SuitNames[Suit.Diamonds] = "D";
            SuitNames[Suit.Hearts] = "H";
            SuitNames[Suit.Spades] = "S";

            FaceNames[Face.Ace] = "A";
            FaceNames[Face.Jack] = "J";
            FaceNames[Face.Queen] = "Q";
            FaceNames[Face.King] = "K";
            for (int i = 2; i < 11; i++)
            {
                FaceNames[(Face)i] = i.ToString();
            }
        }

        static void PrintOut(this Card c)
        {
            Console.WriteLine(FaceNames[c.FaceValue] + SuitNames[c.Suit]);
        }
    }
}

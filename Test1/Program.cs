using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CardGames;

namespace Test1
{
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
            Game game = Game.ParseDefinition(gameDef);

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

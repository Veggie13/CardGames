using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Linq;

namespace CardGames
{
    public abstract class Game
    {
        public Game()
        {
        }

        public Deck Deck { get; private set; }

        public Board Board { get; private set; }

        public abstract IEnumerable<Tuple<string, string>> StackDefinitions { get; }

        public void Initialize()
        {
            Deck = new Deck();
            Board = new Board() { RulesEnabled = false };
            foreach (var defn in StackDefinitions)
            {
                Board.AddStack(defn.Item1, StackRuleManager.Create(defn.Item2));
            }
        }

        public void Deal()
        {
            onDeal();
            Board.RulesEnabled = true;
        }

        protected abstract void onDeal();
    }
}

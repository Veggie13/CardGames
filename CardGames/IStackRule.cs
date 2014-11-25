using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGames
{
    public interface IStackRule
    {
        Board Board { get; set; }

        bool CanDraw(CardStack stack, IEnumerable<Card> cards);
        bool CanDrop(CardStack source, CardStack stack, IEnumerable<Card> cards);
        bool CanActivate(CardStack stack);

        void OnDraw(CardStack stack, IEnumerable<Card> cards);
        void OnDrop(CardStack source, CardStack stack, IEnumerable<Card> cards);
        void OnActivation(CardStack stack);
    }

    public static class StackRuleManager
    {
        private static Dictionary<string, Type> Registry = new Dictionary<string, Type>();

        public static void Register<T>(string name) where T : IStackRule, new()
        {
            Registry[name] = typeof(T);
        }

        public static IStackRule Create(string name)
        {
            return (IStackRule)Activator.CreateInstance(Registry[name]);
        }
    }
}

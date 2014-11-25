using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CardGames
{
    public class Board
    {
        private Dictionary<string, Tuple<CardStack, IStackRule>> _stacks = new Dictionary<string, Tuple<CardStack, IStackRule>>();

        public Board()
        {
            RulesEnabled = true;
        }

        public CardStack this[string name]
        {
            get
            {
                try
                {
                    return _stacks[name].Item1;
                }
                catch (Exception)
                {
                    throw new CardException("No such stack.");
                }
            }
        }

        public bool RulesEnabled { get; set; }

        public void AddStack(string name, IStackRule rule = null)
        {
            if (_stacks.ContainsKey(name))
                throw new CardException("Stack already exists.");

            var stack = new CardStack(name);
            _stacks[name] = new Tuple<CardStack, IStackRule>(stack, rule);

            stack.AboutToDraw += stack_AboutToDraw;
            stack.AboutToReceiveCards += stack_AboutToReceiveCards;
            stack.AboutToActivate += stack_AboutToActivate;
            stack.CardsDrawn += stack_CardsDrawn;
            stack.CardsReceived += stack_CardsReceived;
            stack.Activated += stack_Activated;

            if (rule != null)
                rule.Board = this;
        }

        public void SetStackRule(string stackName, IStackRule rule)
        {
            if (!_stacks.ContainsKey(stackName))
                throw new CardException("No such stack.");

            _stacks[stackName] = new Tuple<CardStack, IStackRule>(_stacks[stackName].Item1, rule);
        }

        private void stack_Activated(CardStack stack, CardStack.CardEventArgs e)
        {
            if (!RulesEnabled)
                return;

            var tuple = _stacks.Values.First(t => t.Item1 == stack);
            if (tuple.Item2 != null)
            {
                tuple.Item2.OnActivation(stack);
            }
        }

        private void stack_CardsReceived(CardStack stack, CardStack.CardEventArgs e)
        {
            if (!RulesEnabled)
                return;

            var tuple = _stacks.Values.First(t => t.Item1 == stack);
            if (tuple.Item2 != null)
            {
                tuple.Item2.OnDrop(e.SourceStack, stack, e.Cards);
            }
        }

        private void stack_CardsDrawn(CardStack stack, CardStack.CardEventArgs e)
        {
            if (!RulesEnabled)
                return;

            var tuple = _stacks.Values.First(t => t.Item1 == stack);
            if (tuple.Item2 != null)
            {
                tuple.Item2.OnDraw(stack, e.Cards);
            }
        }

        private void stack_AboutToActivate(CardStack stack, CardStack.CancellableCardEventArgs e)
        {
            if (!RulesEnabled)
                return;

            var tuple = _stacks.Values.First(t => t.Item1 == stack);
            if (tuple.Item2 != null)
            {
                e.Cancel = !tuple.Item2.CanActivate(stack);
            }
        }

        private void stack_AboutToReceiveCards(CardStack stack, CardStack.CancellableCardEventArgs e)
        {
            if (!RulesEnabled)
                return;

            var tuple = _stacks.Values.First(t => t.Item1 == stack);
            if (tuple.Item2 != null)
            {
                e.Cancel = !tuple.Item2.CanDrop(e.SourceStack, stack, e.Cards);
            }
        }

        private void stack_AboutToDraw(CardStack stack, CardStack.CancellableCardEventArgs e)
        {
            if (!RulesEnabled)
                return;

            var tuple = _stacks.Values.First(t => t.Item1 == stack);
            if (tuple.Item2 != null)
            {
                e.Cancel = !tuple.Item2.CanDraw(stack, e.Cards);
            }
        }
    }
}

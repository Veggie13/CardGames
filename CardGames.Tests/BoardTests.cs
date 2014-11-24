using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace CardGames.Tests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void AddTest()
        {
            Board board = new Board();
            bool thrown = false;
            CardStack stack = null;
            try
            {
                stack = board["testname"];
            }
            catch (CardException)
            {
                thrown = true;
            }
            Assert.IsTrue(thrown);
                        
            board.AddStack("testname");

            thrown = false;
            try
            {
                stack = board["testname"];
            }
            catch (CardException)
            {
                thrown = true;
            }
            Assert.IsFalse(thrown);
            Assert.AreNotEqual(null, stack);
            Assert.AreEqual("testname", stack.Name);
        }

        class MockStackRule : IStackRule
        {
            public Func<CardStack, IEnumerable<Card>, bool> CanDrawFunc;
            public bool CanDraw(CardStack stack, IEnumerable<Card> cards)
            {
                return CanDrawFunc(stack, cards);
            }

            public Func<CardStack, CardStack, IEnumerable<Card>, bool> CanDropFunc;
            public bool CanDrop(CardStack source, CardStack stack, IEnumerable<Card> cards)
            {
                return CanDropFunc(source, stack, cards);
            }

            public Func<CardStack, bool> CanActivateFunc;
            public bool CanActivate(CardStack stack)
            {
                return CanActivateFunc(stack);
            }

            public Action<CardStack, IEnumerable<Card>> OnDrawFunc;
            public void OnDraw(CardStack stack, IEnumerable<Card> cards)
            {
                if (OnDrawFunc != null)
                    OnDrawFunc(stack, cards);
            }

            public Action<CardStack, CardStack, IEnumerable<Card>> OnDropFunc;
            public void OnDrop(CardStack source, CardStack stack, IEnumerable<Card> cards)
            {
                if (OnDropFunc != null)
                    OnDropFunc(source, stack, cards);
            }

            public Action<CardStack> OnActivationFunc;
            public void OnActivation(CardStack stack)
            {
                if (OnActivationFunc != null)
                    OnActivationFunc(stack);
            }
        }

        [TestMethod]
        public void RuleCanDrawAllowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("xxx");
            var xxx = board["xxx"];
            deck.StackOnto(xxx);
            bool called = false;
            MockStackRule rule = new MockStackRule()
            {
                CanDrawFunc = (stack, cards) =>
                {
                    called = true;
                    Assert.AreEqual(xxx, stack);
                    Assert.AreEqual(1, cards.Count());
                    areEqualAssert(Face.Ace, Suit.Clubs, cards.First());
                    return true;
                }
            };
            board.SetStackRule("xxx", rule);

            var action = xxx.TryDrawFromTop(1);
            Assert.IsTrue(called);
            Assert.AreEqual(xxx, action.SourceSequence);
        }

        [TestMethod]
        public void RuleCanDrawDisallowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("yyy");
            var yyy = board["yyy"];
            deck.StackOnto(yyy);
            bool called = false;
            MockStackRule rule = new MockStackRule()
            {
                CanDrawFunc = (stack, cards) =>
                {
                    called = true;
                    return false;
                }
            };
            board.SetStackRule("yyy", rule);

            var action = yyy.TryDrawFromTop(1);
            Assert.IsTrue(called);
            Assert.AreEqual(null, action.SourceSequence);
        }

        [TestMethod]
        public void RuleOnDrawAllowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("ttt");
            var ttt = board["ttt"];
            deck.StackOnto(ttt);
            bool called = false;
            MockStackRule rule = new MockStackRule()
            {
                CanDrawFunc = (stack, cards) => true,
                OnDrawFunc = (stack, cards) =>
                {
                    called = true;
                    Assert.AreEqual(ttt, stack);
                    Assert.AreEqual(1, cards.Count());
                    areEqualAssert(Face.Ace, Suit.Clubs, cards.First());
                }
            };
            board.SetStackRule("ttt", rule);

            var action = ttt.TryDrawFromTop(1);
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void RuleOnDrawDisallowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("www");
            var www = board["www"];
            deck.StackOnto(www);
            bool called = false;
            MockStackRule rule = new MockStackRule()
            {
                CanDrawFunc = (stack, cards) => false,
                OnDrawFunc = (stack, cards) =>
                {
                    called = true;
                }
            };
            board.SetStackRule("www", rule);

            var action = www.TryDrawFromTop(1);
            Assert.IsFalse(called);
        }

        [TestMethod]
        public void RuleCanDropAllowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("zzz");
            var zzz = board["zzz"];
            CardStack other = new CardStack("other");
            deck.StackOnto(other);
            bool called = false;
            MockStackRule rule = new MockStackRule()
            {
                CanDropFunc = (source, stack, cards) =>
                {
                    called = true;
                    Assert.AreEqual(other, source);
                    Assert.AreEqual(zzz, stack);
                    Assert.AreEqual(52, cards.Count());
                    return true;
                }
            };
            board.SetStackRule("zzz", rule);

            bool result = other.StackOnto(zzz);
            Assert.IsTrue(called);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RuleCanDropDisallowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("qqq");
            var qqq = board["qqq"];
            CardStack other = new CardStack("other2");
            deck.StackOnto(other);
            bool called = false;
            MockStackRule rule = new MockStackRule()
            {
                CanDropFunc = (source, stack, cards) =>
                {
                    called = true;
                    return false;
                }
            };
            board.SetStackRule("qqq", rule);

            bool result = other.StackOnto(qqq);
            Assert.IsTrue(called);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RuleOnDropAllowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("uuu");
            var uuu = board["uuu"];
            CardStack other = new CardStack("other3");
            deck.StackOnto(other);
            bool called = false;
            MockStackRule rule = new MockStackRule()
            {
                CanDropFunc = (source, stack, cards) => true,
                OnDropFunc = (source, stack, cards) =>
                {
                    called = true;
                    Assert.AreEqual(other, source);
                    Assert.AreEqual(uuu, stack);
                    Assert.AreEqual(52, cards.Count());
                }
            };
            board.SetStackRule("uuu", rule);

            bool result = other.StackOnto(uuu);
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void RuleOnDropDisallowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("ppp");
            var ppp = board["ppp"];
            deck.StackOnto(ppp);
            bool called = false;
            CardStack other = new CardStack("other4");
            MockStackRule rule = new MockStackRule()
            {
                CanDropFunc = (source, stack, cards) => false,
                OnDropFunc = (source, stack, cards) =>
                {
                    called = true;
                }
            };
            board.SetStackRule("ppp", rule);

            bool result = other.StackOnto(ppp);
            Assert.IsFalse(called);
        }

        [TestMethod]
        public void RuleCanActivateAllowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("rrr");
            var rrr = board["rrr"];
            deck.StackOnto(rrr);
            bool called = false;
            MockStackRule rule = new MockStackRule()
            {
                CanActivateFunc = (stack) =>
                {
                    called = true;
                    Assert.AreEqual(rrr, stack);
                    return true;
                }
            };
            board.SetStackRule("rrr", rule);

            bool result = rrr.Activate();
            Assert.IsTrue(called);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void RuleCanActivateDisallowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("sss");
            var sss = board["sss"];
            deck.StackOnto(sss);
            bool called = false;
            MockStackRule rule = new MockStackRule()
            {
                CanActivateFunc = (stack) =>
                {
                    called = true;
                    return false;
                }
            };
            board.SetStackRule("sss", rule);

            bool result = sss.Activate();
            Assert.IsTrue(called);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RuleOnActivateAllowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("vvv");
            var vvv = board["vvv"];
            deck.StackOnto(vvv);
            bool called = false;
            MockStackRule rule = new MockStackRule()
            {
                CanActivateFunc = (stack) => true,
                OnActivationFunc = (stack) =>
                {
                    called = true;
                    Assert.AreEqual(vvv, stack);
                }
            };
            board.SetStackRule("vvv", rule);

            bool result = vvv.Activate();
            Assert.IsTrue(called);
        }

        [TestMethod]
        public void RuleOnActivateDisallowTest()
        {
            Deck deck = new Deck();
            Board board = new Board();
            board.AddStack("ooo");
            var ooo = board["ooo"];
            deck.StackOnto(ooo);
            bool called = false;
            MockStackRule rule = new MockStackRule()
            {
                CanActivateFunc = (stack) => false,
                OnActivationFunc = (stack) =>
                {
                    called = true;
                }
            };
            board.SetStackRule("ooo", rule);

            bool result = ooo.Activate();
            Assert.IsFalse(called);
        }

        private void areEqualAssert(Face expectedFaceValue, Suit expectedSuit, Card card)
        {
            Assert.AreEqual(expectedFaceValue, card.FaceValue);
            Assert.AreEqual(expectedSuit, card.Suit);
        }
    }
}

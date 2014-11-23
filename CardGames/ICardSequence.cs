using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames
{
    public interface ICardSequenceAction
    {
        ICardSequence SourceSequence { get; }

        IEnumerable<Card> Cards { get; }

        void Undo();
    }

    public class NullCardSequenceAction : ICardSequenceAction
    {
        public ICardSequence SourceSequence
        {
            get { return null; }
        }

        public IEnumerable<Card> Cards
        {
            get { return new Card[0]; }
        }

        public void Undo()
        {
        }
    }

    public interface ICardSequence : IList<Card>
    {
        void Shuffle();

        int Draw(ICollection<Card> dest, params int[] positions);
        int DrawFromTop(ICollection<Card> dest, int count);

        ICardSequenceAction TryDraw(params int[] positions);
        ICardSequenceAction TryDrawFromTop(int count);

        event Action Modified;
    }
}

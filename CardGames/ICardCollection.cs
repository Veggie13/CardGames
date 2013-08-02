using System;
using System.Collections.Generic;
using System.Text;

namespace CardGames
{
    public interface ICardSequence : ICardCollection, IList<Card>
    {
        void Shuffle();
    }

    public interface ICardCollection : ICollection<Card>
    {
        int DrawRandom(int count, ICollection<Card> dest);
        int DrawSequential(int count, ICollection<Card> dest);

        event Action Modified;
    }
}

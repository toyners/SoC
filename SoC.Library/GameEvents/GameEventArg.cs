

namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class GameEventArg<T> : GameEvent
    {
        public readonly T Item;
        public GameEventArg(T item) : base(Guid.Empty) => this.Item = item;
    }
}

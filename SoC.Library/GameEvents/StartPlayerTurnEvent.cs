

namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class StartPlayerTurnEvent : GameEvent
    {
        public StartPlayerTurnEvent() : base(Guid.Empty) {}
    }
}

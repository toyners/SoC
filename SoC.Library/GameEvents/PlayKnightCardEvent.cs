using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class PlayKnightCardEvent : GameEvent
    {
        public PlayKnightCardEvent(Guid playerId) : base(playerId) { }
    }
}

using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    [Obsolete("Deprecated. Use KnightCardPlayedEvent instead")]
    public class PlayKnightCardEvent : GameEvent
    {
        public PlayKnightCardEvent(Guid playerId) : base(playerId) { }
    }
}

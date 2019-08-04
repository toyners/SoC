using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class KnightCardPlayedEvent : GameEvent
    {
        public KnightCardPlayedEvent(Guid playerId, uint hexLocation) : base(playerId)
            => this.HexLocation = hexLocation;

        public uint HexLocation { get; private set; }
    }
}

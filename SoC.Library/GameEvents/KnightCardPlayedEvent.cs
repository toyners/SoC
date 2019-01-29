using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class KnightCardPlayedEvent : GameEvent
    {
        public readonly uint HexLocation;

        public KnightCardPlayedEvent(Guid playerId, uint hexLocation) : base(playerId)
        {
            this.HexLocation = hexLocation;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            return this.HexLocation == ((KnightCardPlayedEvent)obj).HexLocation;
        }
    }
}

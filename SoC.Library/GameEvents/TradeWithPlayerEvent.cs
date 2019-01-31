using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class TradeWithPlayerEvent : GameEvent
    {
        public readonly Guid InitiatingPlayerId;
        public readonly Guid OtherPlayerId;
        public readonly ResourceClutch InitiatingPlayerResources;
        public readonly ResourceClutch OtherPlayerResources;
        public TradeWithPlayerEvent(Guid initiatingPlayerId, Guid otherPlayerId, ResourceClutch initiatingPlayerResources, ResourceClutch otherPlayerResources) : base(Guid.Empty)
        {
            this.InitiatingPlayerId = initiatingPlayerId;
            this.OtherPlayerId = otherPlayerId;
            this.InitiatingPlayerResources = initiatingPlayerResources;
            this.OtherPlayerResources = otherPlayerResources;
        }
    }
}

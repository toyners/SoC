using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class MakeDirectTradeOfferEvent : GameEvent
    {
        public readonly ResourceClutch WantedResources;
        public readonly Guid InitiatingPlayerId;

        public MakeDirectTradeOfferEvent(Guid playerId, Guid initiatingPlayerId, ResourceClutch wantedResources) : base(playerId)
        {
            this.InitiatingPlayerId = initiatingPlayerId;
            this.WantedResources = wantedResources;
        }
    }
}

using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class MakeDirectTradeOfferEvent : GameEvent
    {
        public readonly ResourceClutch WantedResources;
        public MakeDirectTradeOfferEvent(Guid playerId, ResourceClutch wantedResources) : base(playerId)
        {
            this.WantedResources = wantedResources;
        }
    }
}

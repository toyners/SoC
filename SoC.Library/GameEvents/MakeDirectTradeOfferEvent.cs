using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class MakeDirectTradeOfferEvent : GameEvent
    {
        public readonly ResourceClutch WantedResources;
        public readonly Guid BuyingPlayerId;

        public MakeDirectTradeOfferEvent(Guid buyingPlayerId, ResourceClutch wantedResources) : base(Guid.Empty)
        {
            this.BuyingPlayerId = buyingPlayerId;
            this.WantedResources = wantedResources;
        }
    }
}

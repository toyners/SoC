using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class MakeDirectTradeOfferEvent : GameEvent
    {
        public readonly ResourceClutch WantedResources;
        public readonly Guid BuyingPlayerId;

        public MakeDirectTradeOfferEvent(Guid playerId, Guid buyingPlayerId, ResourceClutch wantedResources) : base(playerId)
        {
            this.BuyingPlayerId = buyingPlayerId;
            this.WantedResources = wantedResources;
        }
    }
}

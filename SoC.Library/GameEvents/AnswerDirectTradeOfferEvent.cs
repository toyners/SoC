
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class AnswerDirectTradeOfferEvent : GameEvent
    {
        public readonly Guid BuyingPlayerId;
        public readonly ResourceClutch OfferedResources;

        public AnswerDirectTradeOfferEvent(Guid playerId, Guid buyingPlayerId, ResourceClutch offeredResources) : base(playerId)
        {
            this.BuyingPlayerId = buyingPlayerId;
            this.OfferedResources = offeredResources;
        }
    }
}

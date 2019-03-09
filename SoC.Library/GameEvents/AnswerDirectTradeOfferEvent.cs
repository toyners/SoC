
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class AnswerDirectTradeOfferEvent : GameEvent
    {
        public readonly Guid BuyingPlayerId;
        public readonly ResourceClutch WantedResources;

        public AnswerDirectTradeOfferEvent(Guid playerId, Guid buyingPlayerId, ResourceClutch wantedResources) 
            : base(playerId)
        {
            this.BuyingPlayerId = buyingPlayerId;
            this.WantedResources = wantedResources;
        }
    }
}


namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class AnswerDirectTradeOfferEvent : MakeDirectTradeOfferEvent
    {
        public AnswerDirectTradeOfferEvent(Guid buyingPlayerId, ResourceClutch wantedResources) 
            : base(buyingPlayerId, wantedResources)
        {
        }
    }
}

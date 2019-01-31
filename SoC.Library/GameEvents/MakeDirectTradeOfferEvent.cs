using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class MakeDirectTradeOfferEvent : GameEvent
    {
        public readonly ResourceClutch Resources;
        public MakeDirectTradeOfferEvent(Guid playerId, ResourceClutch resources) : base(playerId)
        {
            this.Resources = resources;
        }
    }
}

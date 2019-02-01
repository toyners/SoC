using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class MakeDirectTradeOfferEvent : GameEvent
    {
        public readonly Guid InitiatingPlayerId;
        public readonly ResourceClutch Resources;
        public MakeDirectTradeOfferEvent(Guid playerId, Guid initiatingPlayerId, ResourceClutch resources) : base(playerId)
        {
            this.InitiatingPlayerId = initiatingPlayerId;
            this.Resources = resources;
        }
    }
}

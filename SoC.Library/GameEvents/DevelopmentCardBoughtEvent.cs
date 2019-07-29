using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class DevelopmentCardBoughtEvent : GameEvent
    {
        public DevelopmentCardTypes? cardType;
        public DevelopmentCardBoughtEvent(Guid playerId) : base(playerId) { }

        public DevelopmentCardBoughtEvent(Guid playerId, DevelopmentCardTypes cardType) : base(playerId)
            => this.cardType = cardType;
    }
}

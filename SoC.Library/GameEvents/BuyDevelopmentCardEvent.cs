using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    [Obsolete("Deprecated. Use DevelopmentCardBoughtEvent instead")]
    public class BuyDevelopmentCardEvent : GameEvent
    {
        public BuyDevelopmentCardEvent(Guid playerId) : base(playerId) { }
    }
}

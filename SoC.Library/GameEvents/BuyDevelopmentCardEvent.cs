using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class BuyDevelopmentCardEvent : GameEvent
    {
        public BuyDevelopmentCardEvent(Guid playerId) : base(playerId) { }
    }
}

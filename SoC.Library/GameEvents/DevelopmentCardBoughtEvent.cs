using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class DevelopmentCardBoughtEvent : GameEvent
    {
        public DevelopmentCardBoughtEvent(Guid playerId) : base(playerId)
        {
        }
    }
}

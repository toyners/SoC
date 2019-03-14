
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class GameJoinedEvent : GameEvent
    {
        public GameJoinedEvent(Guid playerId) : base(playerId)
        {
        }
    }
}

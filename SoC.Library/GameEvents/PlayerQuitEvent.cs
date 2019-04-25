
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class PlayerQuitEvent : GameEvent
    {
        public PlayerQuitEvent(Guid playerId) : base(playerId)
        {
        }
    }
}

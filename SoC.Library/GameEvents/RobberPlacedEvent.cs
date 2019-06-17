using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class RobberPlacedEvent : GameEvent
    {
        public readonly uint Hex;
        public RobberPlacedEvent(Guid playerId, uint hex) : base(playerId) => this.Hex = hex;
    }
}

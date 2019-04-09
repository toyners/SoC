
using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class InfrastructurePlacedEvent : GameEvent
    {
        public readonly uint SettlementLocation;
        public readonly uint RoadSegmentEndLocation;

        public InfrastructurePlacedEvent(Guid playerId, uint settlementLocation, uint roadSegmentEndLocation) : base(playerId)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadSegmentEndLocation = roadSegmentEndLocation;
        }
    }
}

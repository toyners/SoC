
using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class SetupInfrastructurePlacedEvent : GameEvent
    {
        public readonly uint SettlementLocation;
        public readonly uint RoadSegmentEndLocation;

        public SetupInfrastructurePlacedEvent(Guid playerId, uint settlementLocation, uint roadSegmentEndLocation) : base(playerId)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadSegmentEndLocation = roadSegmentEndLocation;
        }
    }
}

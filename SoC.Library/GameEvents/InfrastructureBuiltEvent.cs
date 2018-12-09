
using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class InfrastructureBuiltEvent : GameEvent
    {
        public readonly uint SettlementLocation;
        public readonly uint RoadSegmentEndLocation;

        public InfrastructureBuiltEvent(Guid playerId, uint settlementLocation, uint roadSegmentEndLocation) : base(playerId)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadSegmentEndLocation = roadSegmentEndLocation;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            var other = (InfrastructureBuiltEvent)obj;
            return this.SettlementLocation == other.SettlementLocation && this.RoadSegmentEndLocation == other.RoadSegmentEndLocation;
        }
    }
}

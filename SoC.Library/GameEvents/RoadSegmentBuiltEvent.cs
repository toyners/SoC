
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class RoadSegmentBuiltEvent : GameEvent
    {
        public readonly uint StartLocation;
        public readonly uint EndLocation;

        public RoadSegmentBuiltEvent(Guid playerId, uint startLocation, uint endLocation) : base(playerId)
        {
            this.StartLocation = startLocation;
            this.EndLocation = endLocation;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
            {
                return false;
            }

            var other = (RoadSegmentBuiltEvent)obj;
            return this.StartLocation == other.StartLocation && this.EndLocation == other.EndLocation;
        }

        public override string ToString()
        {
            return $"{base.ToString()} from {this.StartLocation} to {this.EndLocation}";
        }
    }
}

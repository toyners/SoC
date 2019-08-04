
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class RoadBuildingCardPlayedEvent : GameEvent
    {
        public RoadBuildingCardPlayedEvent(Guid playerId, uint firstRoadSegmentStartLocation, uint firstRoadSegmentEndLocation, uint secondRoadSegmentStartLocation, uint secondRoadSegmentEndLocation) : base(playerId)
        {
            this.FirstRoadSegmentStartLocation = firstRoadSegmentStartLocation;
            this.FirstRoadSegmentEndLocation = firstRoadSegmentEndLocation;
            this.SecondRoadSegmentStartLocation = secondRoadSegmentStartLocation;
            this.SecondRoadSegmentEndLocation = secondRoadSegmentEndLocation;
        }

        public uint FirstRoadSegmentStartLocation { get; private set; }
        public uint FirstRoadSegmentEndLocation { get; private set; }
        public uint SecondRoadSegmentStartLocation { get; private set; }
        public uint SecondRoadSegmentEndLocation { get; private set; }
    }
}

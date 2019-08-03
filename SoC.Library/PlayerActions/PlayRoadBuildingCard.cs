
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class PlayRoadBuildingCardAction : PlayerAction
    {
        public PlayRoadBuildingCardAction(Guid initiatingPlayerId, uint firstRoadSegmentStartLocation, uint firstRoadSegmentEndLocation, uint secondRoadSegmentStartLocation, uint secondRoadSegmentEndLocation)
            : base(initiatingPlayerId)
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

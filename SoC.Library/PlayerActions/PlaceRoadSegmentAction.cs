
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class PlaceRoadSegmentAction : PlayerAction
    {
        public readonly uint StartLocation;
        public readonly uint EndLocation;

        public PlaceRoadSegmentAction(Guid playerId, uint startLocation, uint endLocation) : base(playerId)
        {
            this.StartLocation = startLocation;
            this.EndLocation = endLocation;
        }
    }
}

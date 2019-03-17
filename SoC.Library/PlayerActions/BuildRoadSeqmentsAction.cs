
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class BuildRoadSegmentAction : PlayerAction
    {
        public readonly uint StartLocation;
        public readonly uint EndLocation;

        public BuildRoadSegmentAction(uint startLocation, uint endLocation) : base(Guid.Empty)
        {
            this.StartLocation = startLocation;
            this.EndLocation = endLocation;
        }
    }
}


namespace Jabberwocky.SoC.Library.GameActions
{
    using Enums;

    public class BuildRoadSegmentAction : PlayerAction
    {
        public readonly uint StartLocation;
        public readonly uint EndLocation;

        public BuildRoadSegmentAction(uint startLocation, uint endLocation) : base(ComputerPlayerActionTypes.BuildRoadSegment)
        {
            this.StartLocation = startLocation;
            this.EndLocation = endLocation;
        }
    }
}

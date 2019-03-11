
namespace Jabberwocky.SoC.Library.GameActions
{
    public class PlaceInfrastructureAction : PlayerAction
    {
        public readonly uint SettlementLocation;
        public readonly uint RoadEndLocation;
        public PlaceInfrastructureAction(uint settlementLocation, uint roadEndLocation) : base(0)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEndLocation = roadEndLocation;
        }
    }
}

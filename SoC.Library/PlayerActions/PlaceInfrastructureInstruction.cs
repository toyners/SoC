
namespace Jabberwocky.SoC.Library.GameActions
{
    using System;

    public class PlaceInfrastructureAction : PlayerAction
    {
        public readonly uint SettlementLocation;
        public readonly uint RoadEndLocation;
        public PlaceInfrastructureAction(uint settlementLocation, uint roadEndLocation) : base(Guid.Empty)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEndLocation = roadEndLocation;
        }
    }
}

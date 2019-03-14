
namespace Jabberwocky.SoC.Library.GameActions
{
    using System;

    public class PlaceInfrastructureAction : PlayerAction
    {
        public readonly uint SettlementLocation;
        public readonly uint RoadEndLocation;
        public PlaceInfrastructureAction(Guid playerId, uint settlementLocation, uint roadEndLocation) : base(playerId)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEndLocation = roadEndLocation;
        }
    }
}

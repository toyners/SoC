
namespace Jabberwocky.SoC.Library.PlayerActions
{
    using System;

    public class PlaceSetupInfrastructureAction : PlayerAction
    {
        public readonly uint SettlementLocation;
        public readonly uint RoadEndLocation;
        public PlaceSetupInfrastructureAction(Guid playerId, uint settlementLocation, uint roadEndLocation) : base(playerId)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEndLocation = roadEndLocation;
        }
    }
}

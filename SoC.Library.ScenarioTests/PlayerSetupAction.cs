using System;

namespace SoC.Library.ScenarioTests
{
    public class PlayerSetupAction
    {
        public readonly uint SettlementLocation;
        public readonly uint RoadEndLocation;

        public PlayerSetupAction(Guid playerId, uint settlementLocation, uint roadEndLocation)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEndLocation = roadEndLocation;
        }
    }
}

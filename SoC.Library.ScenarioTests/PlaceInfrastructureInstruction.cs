
namespace SoC.Library.ScenarioTests
{
    using System;

    public class PlaceInfrastructureInstruction : Instruction
    {
        public readonly uint SettlementLocation;
        public readonly uint RoadEndLocation;
        public PlaceInfrastructureInstruction(Guid playerId, uint settlementLocation, uint roadEndLocation) : base(playerId)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEndLocation = roadEndLocation;
        }
    }
}

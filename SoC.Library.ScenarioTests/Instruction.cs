
namespace SoC.Library.ScenarioTests
{
    using System;

    public class Instruction
    {
        public readonly Guid Id;
        public Instruction(Guid playerId) => this.Id = playerId;
    }

    internal class BuildRoadInstruction : Instruction
    {
        public readonly uint roadSegmentStart, roadSegmentEnd;
        public BuildRoadInstruction(Guid id, uint roadSegmentStart, uint roadSegmentEnd) : base(id)
        {
            this.roadSegmentStart = roadSegmentStart;
            this.roadSegmentEnd = roadSegmentEnd;
        }
    }

    internal class BuildSettlementInstruction : Instruction
    {
        public readonly uint settlementLocation;
        public BuildSettlementInstruction(Guid id, uint settlementLocation) : base(id)
        {
            this.settlementLocation = settlementLocation;
        }
    }
}

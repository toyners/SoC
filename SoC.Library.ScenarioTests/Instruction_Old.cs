
namespace SoC.Library.ScenarioTests
{
    using System;

    public class Instruction_Old
    {
        public readonly Guid Id;
        public Instruction_Old(Guid playerId) => this.Id = playerId;
    }

    internal class BuildRoadInstruction : Instruction_Old
    {
        public readonly uint roadSegmentStart, roadSegmentEnd;
        public BuildRoadInstruction(Guid id, uint roadSegmentStart, uint roadSegmentEnd) : base(id)
        {
            this.roadSegmentStart = roadSegmentStart;
            this.roadSegmentEnd = roadSegmentEnd;
        }
    }

    internal class BuildSettlementInstruction : Instruction_Old
    {
        public readonly uint settlementLocation;
        public BuildSettlementInstruction(Guid id, uint settlementLocation) : base(id)
        {
            this.settlementLocation = settlementLocation;
        }
    }
}

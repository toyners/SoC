
namespace SoC.Library.ScenarioTests
{
    using System;

    public class Instruction
    {
        public readonly Guid Id;
        public Instruction(Guid playerId) => this.Id = playerId;
    }
}

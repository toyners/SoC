
namespace SoC.Library.ScenarioTests.Instructions
{
    using System.Diagnostics;

    [DebuggerDisplay("{GetType().Name}")]
    internal class Instruction
    {
        public readonly string PlayerName;
        public Instruction(string playerName) => this.PlayerName = playerName;
    }
}
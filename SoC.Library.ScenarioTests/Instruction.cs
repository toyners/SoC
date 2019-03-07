
namespace SoC.Library.ScenarioTests
{
    using System.Diagnostics;

    [DebuggerDisplay("{PlayerName}, {GetType().Name}")]
    internal class Instruction
    {
        public readonly string PlayerName;
        public Instruction(string playerName) => this.PlayerName = playerName;
    }
}
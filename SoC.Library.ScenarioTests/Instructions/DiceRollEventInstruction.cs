
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library.GameEvents;

    internal class DiceRollEventInstruction : EventInstruction
    {
        public DiceRollEventInstruction(string playerName, DiceRollEvent expectedEvent) : base(playerName, expectedEvent) {}
    }
}

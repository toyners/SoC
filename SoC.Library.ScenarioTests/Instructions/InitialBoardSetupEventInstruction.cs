
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library.GameEvents;

    internal class InitialBoardSetupEventInstruction : EventInstruction
    {
        public InitialBoardSetupEventInstruction(string playerName, InitialBoardSetupEvent expectedEvent) : base(playerName, expectedEvent)
        {
        }
    }
}
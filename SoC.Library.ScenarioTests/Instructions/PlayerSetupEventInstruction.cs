
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library.GameEvents;

    internal class PlayerSetupEventInstruction : EventInstruction
    {
        public PlayerSetupEventInstruction(string playerName, PlayerSetupEvent expectedEvent) : base(playerName, expectedEvent)
        {
        }
    }
}
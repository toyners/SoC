
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library.GameEvents;

    internal class PlayerOrderEventInstruction : EventInstruction
    {
        public PlayerOrderEventInstruction(string playerName, PlayerOrderEvent expectedEvent) : base(playerName, expectedEvent)
        {
        }
    }
}

namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library.GameEvents;

    internal class InfrastructurePlacedEventInstruction : EventInstruction
    {
        public InfrastructurePlacedEventInstruction(string playerName, GameEvent expectedEvent) : base(playerName, expectedEvent)
        {
        }
    }
}

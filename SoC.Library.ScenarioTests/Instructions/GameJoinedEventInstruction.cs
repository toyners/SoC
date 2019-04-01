
namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library.GameEvents;

    internal class GameJoinedEventInstruction : EventInstruction
    {
        public GameJoinedEventInstruction(string playerName, GameJoinedEvent expectedEvent) : base(playerName, expectedEvent)
        {
        }
    }
}

namespace SoC.Library.ScenarioTests.Instructions
{
    using Jabberwocky.SoC.Library.GameEvents;

    internal class ResourceCollectedEventInstruction : EventInstruction
    {
        private string playerName;

        public ResourceCollectedEventInstruction(string playerName, ResourcesCollectedEvent gameEvent )
            : base(playerName, gameEvent)
        {
        }
    }
}
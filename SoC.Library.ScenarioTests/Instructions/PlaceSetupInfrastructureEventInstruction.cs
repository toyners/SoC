
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests.Instructions
{
    internal class PlaceSetupInfrastructureEventInstruction : EventInstruction
    {
        public PlaceSetupInfrastructureEventInstruction(string playerName, PlaceSetupInfrastructureEvent expectedEvent) : base(playerName, expectedEvent)
        {
        }
    }
}
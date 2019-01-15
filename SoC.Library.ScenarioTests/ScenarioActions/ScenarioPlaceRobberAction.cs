
using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    internal class ScenarioPlaceRobberAction : ScenarioPlayKnightCardAction
    {
        public readonly ResourceClutch ResourcesToDrop;
        public ScenarioPlaceRobberAction(uint newRobberHex, string selectedPlayerName, ResourceTypes? expectedSingleResource, ResourceClutch resourcesToDrop)
            : base(newRobberHex, selectedPlayerName, expectedSingleResource)
        {
            this.ResourcesToDrop = resourcesToDrop;
        }
    }
}

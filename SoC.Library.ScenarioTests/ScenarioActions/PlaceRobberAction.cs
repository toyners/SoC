
using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    internal class PlaceRobberAction : ScenarioPlayKnightCardAction
    {
        public readonly ResourceClutch ResourcesToDrop;
        public PlaceRobberAction(uint newRobberHex, string selectedPlayerName, ResourceTypes? expectedSingleResource, ResourceClutch resourcesToDrop)
            : base(newRobberHex, selectedPlayerName, expectedSingleResource)
        {
            this.ResourcesToDrop = resourcesToDrop;
        }
    }
}

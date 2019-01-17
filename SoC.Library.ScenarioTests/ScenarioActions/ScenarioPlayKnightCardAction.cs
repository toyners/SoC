using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    internal class ScenarioPlayKnightCardAction : ScenarioSelectResourceFromPlayerAction
    {
        public readonly uint NewRobberHex;

        public ScenarioPlayKnightCardAction(uint newRobberHex, string selectedPlayerName, ResourceTypes expectedSingleResource) 
            : base(selectedPlayerName, expectedSingleResource)
        {
            this.NewRobberHex = newRobberHex;
        }
    }
}

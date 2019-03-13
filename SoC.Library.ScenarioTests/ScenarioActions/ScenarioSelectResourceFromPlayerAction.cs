using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    internal class ScenarioSelectResourceFromPlayerAction : PlayerAction
    {
        public readonly string SelectedPlayerName;
        public readonly ResourceTypes ExpectedSingleResource;

        public ScenarioSelectResourceFromPlayerAction(string selectedPlayerName, ResourceTypes expectedSingleResource) : base()
        {
            this.SelectedPlayerName = selectedPlayerName;
            this.ExpectedSingleResource = expectedSingleResource;
        }
    }
}

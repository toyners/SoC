using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    internal class ScenarioSelectResourceFromPlayerAction : ComputerPlayerAction
    {
        public readonly string SelectedPlayerName;
        public readonly ResourceTypes ExpectedSingleResource;

        public ScenarioSelectResourceFromPlayerAction(string selectedPlayerName, ResourceTypes expectedSingleResource) : base(0)
        {
            this.SelectedPlayerName = selectedPlayerName;
            this.ExpectedSingleResource = expectedSingleResource;
        }
    }
}


namespace SoC.Library.ScenarioTests.ScenarioActions
{
    using System;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.PlayerActions;

    internal class ScenarioSelectResourceFromPlayerAction : PlayerAction
    {
        public readonly string SelectedPlayerName;
        public readonly ResourceTypes ExpectedSingleResource;

        public ScenarioSelectResourceFromPlayerAction(string selectedPlayerName, ResourceTypes expectedSingleResource)
            : base(Guid.Empty)
        {
            this.SelectedPlayerName = selectedPlayerName;
            this.ExpectedSingleResource = expectedSingleResource;
        }
    }
}

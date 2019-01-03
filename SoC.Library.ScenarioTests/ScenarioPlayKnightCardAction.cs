using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Enums;
using Jabberwocky.SoC.Library.GameActions;

namespace SoC.Library.ScenarioTests
{
    internal class ScenarioPlayKnightCardAction : ComputerPlayerAction
    {
        public readonly uint NewRobberHex;
        public readonly string SelectedPlayerName;
        public readonly ResourceTypes ExpectedSingleResource;

        public ScenarioPlayKnightCardAction(uint newRobberHex, string selectedPlayerName, ResourceTypes expectedSingleResource) : base(ComputerPlayerActionTypes.PlayKnightCard)
        {
            this.NewRobberHex = newRobberHex;
            this.SelectedPlayerName = selectedPlayerName;
            this.ExpectedSingleResource = expectedSingleResource;
        }
    }
}

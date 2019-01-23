using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    internal class ScenarioResourcesToDropAction : ComputerPlayerAction
    {
        public readonly string PlayerName;
        public readonly ResourceClutch Resources;

        public ScenarioResourcesToDropAction(string playerName, ResourceClutch resources) : base(0)
        {
            this.PlayerName = playerName;
            this.Resources = resources;
        }
    }
}

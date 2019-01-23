using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    internal class ScenarioDropResourcesAction : ComputerPlayerAction
    {
        public readonly string PlayerName;
        public readonly ResourceClutch Resources;

        public ScenarioDropResourcesAction(string playerName, ResourceClutch resources) : base(0)
        {
            this.PlayerName = playerName;
            this.Resources = resources;
        }

        public DropResourcesAction CreateDropResourcesAction()
        {
            return new DropResourcesAction(this.Resources);
        }
    }
}

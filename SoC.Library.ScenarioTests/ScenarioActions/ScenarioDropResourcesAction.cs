
namespace SoC.Library.ScenarioTests.ScenarioActions
{
    using System;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameActions;

    internal class ScenarioDropResourcesAction : PlayerAction
    {
        public readonly string PlayerName;
        public readonly ResourceClutch Resources;

        public ScenarioDropResourcesAction(string playerName, ResourceClutch resources) : base(Guid.Empty)
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

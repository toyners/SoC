using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    internal class ScenarioResourcesToDropAction : ComputerPlayerAction
    {
        public readonly ResourceClutch Resources;

        public ScenarioResourcesToDropAction(ResourceClutch resources) : base(0)
        {
            this.Resources = resources;
        }
    }
}

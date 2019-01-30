using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    public class ScenarioMakeDirectTradeOffer : ComputerPlayerAction
    {
        public readonly ResourceClutch WantedResources;
        public ScenarioMakeDirectTradeOffer(ResourceClutch wantedResources) : base(0)
        {
            this.WantedResources = wantedResources;
        }
    }
}

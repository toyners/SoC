using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    public class ScenarioMakeDirectTradeOffer : ComputerPlayerActionWrapper
    {
        public readonly string InitiatingPlayerName;
        public ScenarioMakeDirectTradeOffer(string initiatingPlayerName, ResourceClutch wantedResources) : base(new MakeDirectTradeOfferAction(wantedResources))
        {
            this.InitiatingPlayerName = initiatingPlayerName;
        }
    }
}

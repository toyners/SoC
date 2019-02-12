using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    public class ScenarioMakeDirectTradeOffer : ComputerPlayerActionWrapper
    {
        public ScenarioMakeDirectTradeOffer(string buyingPlayerName, ResourceClutch wantedResources)
            : base(buyingPlayerName, new MakeDirectTradeOfferAction(wantedResources))
        {
        }
    }
}

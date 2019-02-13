using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests.ScenarioActions
{
    public class ScenarioMakeDirectTradeOfferAction
    {
        public readonly ResourceClutch WantedResources;
        public readonly string BuyingPlayerName; 
        public ScenarioMakeDirectTradeOfferAction(string buyingPlayerName, ResourceClutch wantedResources)
            //: base(buyingPlayerName, new MakeDirectTradeOfferAction(wantedResources))
        {
            this.WantedResources = wantedResources;
            this.BuyingPlayerName = buyingPlayerName;
        }

        public MakeDirectTradeOfferAction Action(Dictionary<string, IPlayer> playersByName)
        {
            return new MakeDirectTradeOfferAction(playersByName[this.BuyingPlayerName].Id, this.WantedResources);
        }
    }
}

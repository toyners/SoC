
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class MakeDirectTradeOfferEventInstruction : EventInstruction
    {
        private string playerName;
        private string buyingPlayerName;
        private ResourceClutch wantedResources;

        public MakeDirectTradeOfferEventInstruction(string playerName, string buyingPlayerName, ResourceClutch wantedResources)
            : base(playerName)
        {
            this.playerName = playerName;
            this.buyingPlayerName = buyingPlayerName;
            this.wantedResources = wantedResources;
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new MakeDirectTradeOfferEvent(
                playerIdsByName[this.buyingPlayerName], 
                this.wantedResources);
        }
    }
}

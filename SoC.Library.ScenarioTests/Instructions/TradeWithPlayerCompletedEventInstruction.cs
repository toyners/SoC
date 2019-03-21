
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class TradeWithPlayerCompletedEventInstruction : EventInstruction
    {
        private string buyingPlayerName;
        private ResourceClutch buyingResources;
        private string sellingPlayerName;
        private ResourceClutch sellingResources;

        public TradeWithPlayerCompletedEventInstruction(string playerName, 
            string buyingPlayerName,
            ResourceClutch buyingResources,
            string sellingPlayerName,
            ResourceClutch sellingResources)
            : base(playerName)
        {
            this.buyingPlayerName = buyingPlayerName;
            this.buyingResources = buyingResources;
            this.sellingPlayerName = sellingPlayerName;
            this.sellingResources = sellingResources;
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new TradeWithPlayerCompletedEvent(
                playerIdsByName[this.buyingPlayerName],
                this.buyingResources,
                playerIdsByName[this.sellingPlayerName],
                this.sellingResources);
        }
    }
}
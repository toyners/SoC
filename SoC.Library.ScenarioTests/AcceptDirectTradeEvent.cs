using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameEvents;
using SoC.Library.ScenarioTests.Instructions;

namespace SoC.Library.ScenarioTests
{
    internal class AcceptDirectTradeEventInstruction : EventInstruction
    {
        private string buyerName;
        private ResourceClutch buyingResources;
        private string sellerName;
        private ResourceClutch sellingResources;

        public AcceptDirectTradeEventInstruction(string playerName, string buyerName, ResourceClutch buyingResources, string sellerName, ResourceClutch sellingResources)
            : base(playerName)
        {
            this.buyerName = buyerName;
            this.buyingResources = buyingResources;
            this.sellerName = sellerName;
            this.sellingResources = sellingResources;
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new AcceptTradeEvent(
                playerIdsByName[this.buyerName],
                this.buyingResources,
                playerIdsByName[this.sellerName],
                this.sellingResources);
        }
    }
}
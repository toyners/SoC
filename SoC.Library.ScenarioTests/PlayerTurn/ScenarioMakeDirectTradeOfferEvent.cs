using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class ScenarioMakeDirectTradeOfferEvent_Old
    {
        private Guid id1;
        private Guid id2;
        private ResourceClutch resources;

        public ScenarioMakeDirectTradeOfferEvent_Old(Guid id1, Guid id2, ResourceClutch resources)
        {
            this.id1 = id1;
            this.id2 = id2;
            this.resources = resources;
        }
    }

    internal class ScenarioMakeDirectTradeOfferEventInstruction : EventInstruction
    {
        private readonly string receivingPlayerName;
        private readonly string buyingPlayerName;
        private readonly ResourceClutch wantedResources;

        public ScenarioMakeDirectTradeOfferEventInstruction(string receivingPlayerName, string buyingPlayerName, ResourceClutch wantedResources)
            : base(receivingPlayerName)
        {
            this.receivingPlayerName = receivingPlayerName;
            this.buyingPlayerName = buyingPlayerName;
            this.wantedResources = wantedResources;
        }

        public override GameEvent Event(IDictionary<string, Guid> playerIdsByName)
        {
            return new MakeDirectTradeOfferEvent(playerIdsByName[this.receivingPlayerName], playerIdsByName[this.buyingPlayerName], this.wantedResources);
        }
    }
}
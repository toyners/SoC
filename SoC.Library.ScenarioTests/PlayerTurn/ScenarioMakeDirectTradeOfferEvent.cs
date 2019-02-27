using System;
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

    internal class ScenarioMakeDirectTradeOfferEvent : GameEvent
    {
        public readonly string ReceivingPlayerName;
        private readonly string BuyingPlayerName;
        private readonly ResourceClutch WantedResources;

        public ScenarioMakeDirectTradeOfferEvent(string receivingPlayerName, string buyingPlayerName, ResourceClutch wantedResources)
            : base(Guid.Empty)
        {
            this.ReceivingPlayerName = receivingPlayerName;
            this.BuyingPlayerName = buyingPlayerName;
            this.WantedResources = wantedResources;
        }
    }
}
using System;
using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class ScenarioMakeDirectTradeOfferEvent
    {
        private Guid id1;
        private Guid id2;
        private ResourceClutch resources;

        public ScenarioMakeDirectTradeOfferEvent(Guid id1, Guid id2, ResourceClutch resources)
        {
            this.id1 = id1;
            this.id2 = id2;
            this.resources = resources;
        }
    }
}
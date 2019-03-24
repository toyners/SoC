using System;
using Jabberwocky.SoC.Library.GameEvents;

namespace Jabberwocky.SoC.Library
{
    public class AcceptTradeEvent : GameEvent
    {
        private ResourceClutch buyingResources;
        private Guid sellerId;
        private ResourceClutch sellingResources;

        public AcceptTradeEvent(Guid buyerId, ResourceClutch buyingResources, Guid sellerId, ResourceClutch sellingResources)
            : base(buyerId)
        {
            this.buyingResources = buyingResources;
            this.sellerId = sellerId;
            this.sellingResources = sellingResources;
        }
    }
}
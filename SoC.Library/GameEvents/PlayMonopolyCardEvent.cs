
namespace Jabberwocky.SoC.Library
{
    using System;
    using GameEvents;

    public class PlayMonopolyCardEvent : GameEvent
    {
        public PlayMonopolyCardEvent(Guid playerId, ResourceTransactionList resourceTransactionList) : base(playerId)
        {
            this.ResourceTransactionList = resourceTransactionList;
        }

        public ResourceTransactionList ResourceTransactionList { get; }
    }
}

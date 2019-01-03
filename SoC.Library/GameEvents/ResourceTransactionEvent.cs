
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class ResourceTransactionEvent : GameEvent
    {
        #region Fields
        public readonly ResourceTransactionList ResourceTransactions;
        #endregion

        #region Construction
        public ResourceTransactionEvent(Guid playerId, ResourceTransactionList resourceTransactions) : base(playerId)
        {
            this.ResourceTransactions = resourceTransactions;
        }

        public ResourceTransactionEvent(Guid playerId, ResourceTransaction resourceTransaction) : base(playerId)
        {
            this.ResourceTransactions = new ResourceTransactionList();
            this.ResourceTransactions.Add(resourceTransaction);
        }
        #endregion
    }
}

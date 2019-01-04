
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Generic;

    public struct ResourceTransaction
    {
        #region Fields
        public readonly Guid ReceivingPlayerId;
        public readonly Guid GivingPlayerId;
        public readonly ResourceClutch Resources;
        #endregion

        #region Construction
        public ResourceTransaction(Guid receivingPlayerId, Guid givingPlayerId, ResourceClutch resources)
        {
            this.ReceivingPlayerId = receivingPlayerId;
            this.GivingPlayerId = givingPlayerId;
            this.Resources = resources;
        }
        #endregion

        #region Methods
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ResourceTransaction))
                return false;

            var other = (ResourceTransaction)obj;
            return
                this.ReceivingPlayerId.Equals(other.ReceivingPlayerId) &&
                this.GivingPlayerId.Equals(other.GivingPlayerId) &&
                this.Resources.Equals(other.Resources);
        }
        #endregion
    }

    public class ResourceTransactionList
    {
        #region Fields
        private List<ResourceTransaction> resourceTransactions = new List<ResourceTransaction>();
        #endregion

        #region Properties
        public int Count { get { return this.resourceTransactions.Count; } }

        public ResourceTransaction this[int index]
        {
            get
            {
                if (index < 0 || index >= this.resourceTransactions.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return this.resourceTransactions[index];
            }
        }
        #endregion

        #region Methods
        public void Add(ResourceTransaction resourceTransaction)
        {
            this.resourceTransactions.Add(resourceTransaction);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ResourceTransactionList))
                return false;

            var other = (ResourceTransactionList)obj;

            if (!this.resourceTransactions.Count.Equals(other.resourceTransactions.Count))
                return false;

            for (var index = 0; index < this.resourceTransactions.Count; index++)
            {
                if (!this.resourceTransactions[index].Equals(other.resourceTransactions[index]))
                    return false;
            }

            return true;
        }
        #endregion
    }
}

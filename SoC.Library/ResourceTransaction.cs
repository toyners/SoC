
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;

  public struct ResourceTransaction
  {
    public readonly Guid ReceivingPlayerId;
    public readonly Guid GivingPlayerId;
    public readonly ResourceClutch Resources;

    public ResourceTransaction(Guid receivingPlayerId, Guid givingPlayerId, ResourceClutch resources)
    {
      this.ReceivingPlayerId = receivingPlayerId;
      this.GivingPlayerId = givingPlayerId;
      this.Resources = resources;
    }
  }

  public class ResourceTransactionList
  {
    #region Fields
    private List<ResourceTransaction> resourceTransactions = new List<ResourceTransaction>();
    #endregion

    #region Properties
    public Int32 Count { get { return resourceTransactions.Count; } }

    public ResourceTransaction this[Int32 index]
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
    #endregion
  }
}

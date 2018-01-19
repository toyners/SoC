
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections;
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

  public class ResourceTransactionList : IEnumerable<ResourceTransaction>
  {
    private ResourceTransaction[] resourceTransactions;

    public Int32 Count { get;  private set; }

    public ResourceTransaction this[Int32 index]
    {
      get
      {
        if (index < 0 || index >= this.resourceTransactions.Length)
        {
          throw new IndexOutOfRangeException();
        }

        return this.resourceTransactions[index];
      }
    }

    public void Add(ResourceTransaction resourceTransaction)
    {
      throw new NotImplementedException(); 
    }

    public IEnumerator<ResourceTransaction> GetEnumerator()
    {
      throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      throw new NotImplementedException();
    }
  }
}

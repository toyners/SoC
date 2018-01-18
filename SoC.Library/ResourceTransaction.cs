
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
    public Int32 Count { get;  private set; }
    public void Add(ResourceTransaction resourceTransaction)
    {
     
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

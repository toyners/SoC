
namespace Jabberwocky.SoC.Library
{
  using System;

  public struct Transaction
  {
    public readonly Guid ReceivingPlayerId;
    public readonly Guid GivingPlayerId;
    public readonly ResourceClutch Resources;

    public Transaction(Guid receivingPlayerId, Guid givingPlayerId, ResourceClutch resources)
    {
      this.ReceivingPlayerId = receivingPlayerId;
      this.GivingPlayerId = givingPlayerId;
      this.Resources = resources;
    }
  }
}

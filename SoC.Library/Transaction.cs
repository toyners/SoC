
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

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

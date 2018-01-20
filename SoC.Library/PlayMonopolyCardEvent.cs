
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;

  public class PlayMonopolyCardEvent : GameEvent
  {
    public readonly Dictionary<Guid, ResourceClutch> Resources = new Dictionary<Guid, ResourceClutch>();

    public PlayMonopolyCardEvent(Guid playerId, ResourceTransactionList resourceTransactionList) : base(playerId)
    {
    }

    public override Boolean Equals(Object obj)
    {
      throw new NotImplementedException();
    }
  }
}

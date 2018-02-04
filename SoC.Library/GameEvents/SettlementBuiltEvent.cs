
namespace Jabberwocky.SoC.Library.GameEvents
{
  using System;

  public class SettlementBuiltEvent : GameEvent
  {
    public readonly UInt32 Location;

    public SettlementBuiltEvent(Guid playerId, UInt32 location) : base(playerId)
    {
      this.Location = location;
    }

    public override Boolean Equals(Object obj)
    {
      if (!base.Equals(obj))
      {
        return false;
      }

      return this.Location == ((SettlementBuiltEvent)obj).Location;
    }
  }
}

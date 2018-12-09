
namespace Jabberwocky.SoC.Library.GameEvents
{
  using System;

  public class SettlementBuiltEvent : GameEvent
  {
    public readonly uint Location;

    public SettlementBuiltEvent(Guid playerId, uint location) : base(playerId)
    {
      this.Location = location;
    }

    public override bool Equals(object obj)
    {
      if (!base.Equals(obj))
      {
        return false;
      }

      return this.Location == ((SettlementBuiltEvent)obj).Location;
    }
  }
}

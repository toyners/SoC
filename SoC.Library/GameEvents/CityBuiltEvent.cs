
namespace Jabberwocky.SoC.Library.GameEvents
{
  using System;

  public class CityBuiltEvent : SettlementBuiltEvent
  {
    public CityBuiltEvent(Guid playerId, UInt32 location) : base(playerId, location)
    {
    }

    public override Boolean Equals(Object obj)
    {
      if (!base.Equals(obj))
      {
        return false;
      }

      return this.Location == ((CityBuiltEvent)obj).Location;
    }
  }
}

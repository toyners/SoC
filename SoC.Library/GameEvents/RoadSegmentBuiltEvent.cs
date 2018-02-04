
namespace Jabberwocky.SoC.Library.GameEvents
{
  using System;

  public class RoadSegmentBuiltEvent : GameEvent
  {
    public readonly UInt32 StartLocation;
    public readonly UInt32 EndLocation;

    public RoadSegmentBuiltEvent(Guid playerId, UInt32 startLocation, UInt32 endLocation) : base(playerId)
    {
      this.StartLocation = startLocation;
      this.EndLocation = endLocation;
    }

    public override Boolean Equals(Object obj)
    {
      if (!base.Equals(obj))
      {
        return false;
      }

      var other = (RoadSegmentBuiltEvent)obj;
      return this.StartLocation == other.StartLocation && this.EndLocation == other.EndLocation;
    }
  }
}

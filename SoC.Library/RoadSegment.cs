
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;

  [DebuggerDisplay("({Location1}, {Location2})")]
  public class RoadSegment
  {
    public readonly UInt32 Location1;
    public readonly UInt32 Location2;
    public List<RoadSegment> ConnectedToLocation1 = new List<RoadSegment>();
    public List<RoadSegment> ConnectedToLocation2 = new List<RoadSegment>();

    public RoadSegment(UInt32 location1, UInt32 location2)
    {
      if (location1 == location2)
      {
        throw new ArgumentException("Locations cannot be the same.");
      }

      this.Location1 = location1;
      this.Location2 = location2;
    }

    public static Boolean operator ==(RoadSegment road1, RoadSegment road2)
    {
      if (Object.ReferenceEquals(road1, null) && Object.ReferenceEquals(road2, null))
      {
        return true;
      }

      if (Object.ReferenceEquals(road1, null) || Object.ReferenceEquals(road2, null))
      {
        return false;
      }

      if (road1.Location1 == road2.Location1 && road1.Location2 == road2.Location2)
      {
        return true;
      }

      if (road1.Location1 == road2.Location2 && road1.Location2 == road2.Location1)
      {
        return true;
      }

      return false;
    }

    public static Boolean operator !=(RoadSegment road1, RoadSegment road2)
    {
      return !(road1 == road2);
    }

    public override Boolean Equals(Object obj)
    {
      if (!(obj is RoadSegment))
      {
        return false;
      }

      return this == (RoadSegment)obj;
    }

    public override Int32 GetHashCode()
    {
      return base.GetHashCode();
    }

    /// <summary>
    /// Returns true if another road segment is connected (i.e. shares only one location) to this road.
    /// If the other road segment is the same as this road segment then false is returned.
    /// </summary>
    /// <param name="road">Another road segment.</param>
    /// <returns>True if connected; otherwise false.</returns>
    public Boolean IsConnected(RoadSegment road)
    {
      if (this == road)
      {
        return false;
      }

      return this.Location1 == road.Location1 || this.Location1 == road.Location2 || this.Location2 == road.Location1 || this.Location2 == road.Location2;
    }
  }
}

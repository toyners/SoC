
using System;

namespace Jabberwocky.SoC.Library
{
  public class Trail
  {
    public readonly Location Location1;
    public readonly Location Location2;

    public Trail(Location location1, Location location2)
    {
      this.Location1 = location1;
      this.Location2 = location2;
    }
  }

  public struct Road
  {
    public readonly UInt32 Location1;
    public readonly UInt32 Location2;

    public Road(UInt32 location1, UInt32 location2)
    {
      this.Location1 = location1;
      this.Location2 = location2;
    }
  }
}


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
}

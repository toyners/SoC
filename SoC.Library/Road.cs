
namespace Jabberwocky.SoC.Library
{
  using System;

  public struct Road
  {
    public readonly UInt32 Location1;
    public readonly UInt32 Location2;

    public Road(UInt32 location1, UInt32 location2)
    {
      if (location1 == location2)
      {
        throw new ArgumentException("Locations cannot be the same");
      }

      this.Location1 = location1;
      this.Location2 = location2;
    }
  }
}

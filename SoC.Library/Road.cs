
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Diagnostics;

  [DebuggerDisplay("({Location1}, {Location2})")]
  public class Road
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

    public override Boolean Equals(Object obj)
    {
      if (base.Equals(obj))
      {
        return true;
      }

      var other = (Road)obj;

      if (this.Location1 == other.Location1 && this.Location2 == other.Location2)
      {
        return true;
      }

      if (this.Location1 == other.Location2 && this.Location2 == other.Location1)
      {
        return true;
      }

      return false;
    }
  }
}

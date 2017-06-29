
namespace Jabberwocky.SoC.Library
{
  using System;
  using System.Diagnostics;

  [DebuggerDisplay("({Location1}, {Location2})")]
  public struct Road
  {
    public readonly UInt32 Location1;
    public readonly UInt32 Location2;

    public Road(UInt32 location1, UInt32 location2)
    {
      if (location1 == location2)
      {
        throw new ArgumentException("Locations cannot be the same.");
      }

      if (location1 > location2  && location1 - location2 > 1)
      {
        throw new ArgumentException("Length cannot be greater than one.");
      }

      if (location2 > location1 && location2 - location1 > 1)
      {
        throw new ArgumentException("Length cannot be greater than one.");
      }

      this.Location1 = location1;
      this.Location2 = location2;
    }

    public static Boolean operator ==(Road road1, Road road2)
    {
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

    public static Boolean operator !=(Road road1, Road road2)
    {
      return !(road1 == road2);
    }

    public override Boolean Equals(Object obj)
    {
      if (!(obj is Road))
      {
        return false;
      }

      return this == (Road)obj;
    }

    public override Int32 GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}

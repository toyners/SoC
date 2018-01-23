
namespace Jabberwocky.SoC.Library
{
  using System;

  public struct ResourceCollection : IComparable
  {
    public readonly UInt32 Location;
    public readonly ResourceClutch Resources;

    public ResourceCollection(UInt32 location, ResourceClutch resourceClutch)
    {
      this.Location = location;
      this.Resources = resourceClutch;
    }

    public Int32 CompareTo(Object obj)
    {
      var other = (ResourceCollection)obj;
      return this.Location.CompareTo(other.Location);
    }
  }
}

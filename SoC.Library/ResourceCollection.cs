
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("Location {Location}, Resources {Resources}")]
    public struct ResourceCollection : IComparable
    {
        public readonly uint Location;
        public readonly ResourceClutch Resources;

        public ResourceCollection(uint location, ResourceClutch resourceClutch)
        {
            this.Location = location;
            this.Resources = resourceClutch;
        }

        public int CompareTo(object obj)
        {
            var other = (ResourceCollection)obj;
            return this.Location.CompareTo(other.Location);
        }
    }
}

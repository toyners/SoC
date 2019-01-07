
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

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is ResourceCollection))
                return false;

            var other = (ResourceCollection)obj;
            return this.Location.Equals(other.Location) && this.Resources.Equals(other.Resources);
        }
    }
}

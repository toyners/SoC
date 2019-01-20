
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Jabberwocky.SoC.Library.GameEvents;

    public class ResourceUpdate : GameEvent
    {
        public readonly Dictionary<Guid, ResourceClutch> Resources = new Dictionary<Guid, ResourceClutch>();

        public ResourceUpdate(Dictionary<Guid, ResourceClutch> resourcesLost) : base(Guid.Empty)
        {
            this.Resources = resourcesLost;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            var other = (ResourceUpdate)obj;
            if (this.Resources.Count != other.Resources.Count)
                return false;

            var sortedKeys = this.Resources.Keys.OrderBy(k => k);
            foreach(var key in sortedKeys)
            {
                if (!this.Resources[key].Equals(other.Resources[key]))
                    return false;
            }

            return true;
        }
    }
}

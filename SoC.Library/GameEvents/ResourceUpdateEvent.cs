
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ResourceUpdateEvent : GameEvent
    {
        public readonly Dictionary<Guid, ResourceClutch> Resources = new Dictionary<Guid, ResourceClutch>();

        public ResourceUpdateEvent(Dictionary<Guid, ResourceClutch> resourcesLost) : base(Guid.Empty)
        {
            this.Resources = resourcesLost;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            var other = (ResourceUpdateEvent)obj;
            if ((this.Resources != null && other.Resources == null) ||
                (this.Resources == null && other.Resources != null))
                return false;

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

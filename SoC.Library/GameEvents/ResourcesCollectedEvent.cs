
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;
    using System.Collections.Generic;

    public class ResourcesCollectedEvent : GameEvent
    {
        public readonly Dictionary<Guid, ResourceCollection[]> ResourcesCollectedByPlayerId;

        public ResourcesCollectedEvent(Dictionary<Guid, ResourceCollection[]> resourcesCollectedByPlayerId) : base(Guid.Empty)
        {
            this.ResourcesCollectedByPlayerId = resourcesCollectedByPlayerId;
        }

        /*public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            var other = (ResourcesCollectedEvent)obj;

            if (this.ResourcesCollectedByPlayerId.Length != other.ResourcesCollectedByPlayerId.Length)
                return false;

            for (var index = 0; index < this.ResourcesCollectedByPlayerId.Length; index++)
            {
                if (!this.ResourcesCollectedByPlayerId[index].Equals(other.ResourcesCollectedByPlayerId[index]))
                    return false;
            }

            return true;
        }*/
    }
}

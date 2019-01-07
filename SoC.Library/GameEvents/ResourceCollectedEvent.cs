
using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class ResourcesCollectedEvent : GameEvent
    {
        public readonly ResourceCollection[] ResourceCollection;

        public ResourcesCollectedEvent(Guid playerId, ResourceCollection[] resourceCollection) : base(playerId)
        {
            this.ResourceCollection = resourceCollection;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            var other = (ResourcesCollectedEvent)obj;

            if (this.ResourceCollection.Length != other.ResourceCollection.Length)
                return false;

            for (var index = 0; index < this.ResourceCollection.Length; index++)
            {
                if (!this.ResourceCollection[index].Equals(other.ResourceCollection[index]))
                    return false;
            }

            return true;
        }
    }
}

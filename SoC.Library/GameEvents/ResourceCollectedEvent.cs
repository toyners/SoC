
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

        // TODO Implement Equals method
    }
}

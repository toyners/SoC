
using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class ResourceCollectedEvent : GameEvent
    {
        public readonly ResourceCollection[] Resources;

        public ResourceCollectedEvent(Guid playerId, ResourceCollection[] resources) : base(playerId)
        {
            this.Resources = resources;
        }
    }
}

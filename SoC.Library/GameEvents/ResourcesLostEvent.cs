using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class ResourcesLostEvent : GameEvent
    {
        public ResourceClutch ResourcesLost;
        public ResourcesLostEvent(ResourceClutch resourcesLost) : base(Guid.Empty)
        {
            this.ResourcesLost = resourcesLost;
        }
    }
}

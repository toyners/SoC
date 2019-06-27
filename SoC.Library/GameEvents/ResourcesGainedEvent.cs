
namespace Jabberwocky.SoC.Library.GameEvents
{
    using System;

    public class ResourcesGainedEvent : GameEvent
    {
        public ResourcesGainedEvent(ResourceClutch resources) : base(Guid.Empty)
        {
        }
    }
}

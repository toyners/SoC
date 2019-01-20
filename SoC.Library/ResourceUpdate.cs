
namespace Jabberwocky.SoC.Library
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.GameEvents;

    public class ResourceUpdate : GameEvent
    {
        public readonly Dictionary<Guid, ResourceClutch> Resources = new Dictionary<Guid, ResourceClutch>();

        public ResourceUpdate(Dictionary<Guid, ResourceClutch> resourcesLost) : base(Guid.Empty)
        {
            this.Resources = resourcesLost;
        }
    }
}

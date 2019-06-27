using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class ResourcesLostEvent : GameEvent
    {
        [Flags]
        public enum ReasonTypes
        {
            TooManyResources,
            Robbed,
            Witness
        }

        public ResourceClutch ResourcesLost;
        public ReasonTypes Reason;
        public Guid Beneficiary;
        public ResourcesLostEvent(ResourceClutch resourcesLost, Guid beneficiary, ReasonTypes reason) : base(Guid.Empty)
        {
            this.ResourcesLost = resourcesLost;
        }
    }
}

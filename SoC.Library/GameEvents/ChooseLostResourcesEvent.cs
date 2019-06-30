using System;
using static Jabberwocky.SoC.Library.GameEvents.ResourcesLostEvent;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class ChooseLostResourcesEvent : GameEvent
    {
        public int ResourceCount;
        public ReasonTypes Reason;

        public ChooseLostResourcesEvent(int resourceCount) : base(Guid.Empty)
        {
            this.ResourceCount = resourceCount;
        }
    }
}

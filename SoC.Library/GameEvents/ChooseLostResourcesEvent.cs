using System;

namespace Jabberwocky.SoC.Library.GameEvents
{
    public class ChooseLostResourcesEvent : GameEvent
    {
        public int ResourceCount;
        public ChooseLostResourcesEvent(int resourceCount) : base(Guid.Empty)
        {
            this.ResourceCount = resourceCount;
        }
    }
}

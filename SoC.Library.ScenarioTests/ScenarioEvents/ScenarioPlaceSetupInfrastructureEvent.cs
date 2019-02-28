

namespace SoC.Library.ScenarioTests.ScenarioEvents
{
    using System;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameEvents;

    public class ScenarioPlaceSetupInfrastructureEvent : GameEvent
    {
        public ScenarioPlaceSetupInfrastructureEvent() : base(Guid.Empty) { }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(PlaceSetupInfrastructureEvent))
                return false;

            return ((PlaceSetupInfrastructureEvent)obj).Item != null;
        }
    }
}

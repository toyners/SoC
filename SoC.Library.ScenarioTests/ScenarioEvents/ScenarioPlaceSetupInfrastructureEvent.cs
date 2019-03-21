

namespace SoC.Library.ScenarioTests.ScenarioEvents
{
    using System;
    using Jabberwocky.SoC.Library.GameEvents;

    public class ScenarioPlaceSetupInfrastructureEvent : GameEvent
    {
        public ScenarioPlaceSetupInfrastructureEvent() : base(Guid.Empty) { }

        /*public override bool Equals(object obj)
        {
            return obj != null && obj.GetType() == typeof(PlaceSetupInfrastructureEvent);
        }*/
    }
}

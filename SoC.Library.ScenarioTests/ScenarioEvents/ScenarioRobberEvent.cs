
using System;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests.ScenarioEvents
{
    internal class ScenarioRobberEvent : GameEvent
    {
        public readonly int ExpectedResourcesToDropCount;
        public ScenarioRobberEvent(int expectedResourcesToDropCount) : base(Guid.Empty)
        {
            this.ExpectedResourcesToDropCount = expectedResourcesToDropCount;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            return this.ExpectedResourcesToDropCount.Equals(((ScenarioRobberEvent)obj).ExpectedResourcesToDropCount);
        }
    }
}

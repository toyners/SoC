using System;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests
{
    internal class PlaceholderEvent : GameEvent
    {
        public LocalGameControllerScenarioRunner.EventTypes EventType;

        public PlaceholderEvent(LocalGameControllerScenarioRunner.EventTypes eventType) : base(Guid.Empty)
        {
            this.EventType = eventType;
        }
    }
}
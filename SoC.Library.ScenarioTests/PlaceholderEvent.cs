using System;
using System.Diagnostics;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests
{
    [DebuggerDisplay("{GetType().Name}, EventType {EventType}")]
    internal class PlaceholderEvent : GameEvent
    {
        public LocalGameControllerScenarioRunner.EventTypes EventType;

        public PlaceholderEvent(LocalGameControllerScenarioRunner.EventTypes eventType) : base(Guid.Empty)
        {
            this.EventType = eventType;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            switch (this.EventType)
            {
                case LocalGameControllerScenarioRunner.EventTypes.DiceRollEvent: return obj is DiceRollEvent;
                case LocalGameControllerScenarioRunner.EventTypes.ResourcesCollectedEvent: return obj is ResourcesCollectedEvent;
            }

            throw new NotImplementedException("Should not get here");
        }
    }
}
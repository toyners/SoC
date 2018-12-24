using System;
using System.Diagnostics;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests
{
    [DebuggerDisplay("{GetType().Name}, EventType {GameEventType}")]
    internal class IgnoredEvent : GameEvent
    {
        public readonly Type GameEventType;

        public IgnoredEvent(Type gameEventType) : base(Guid.Empty) { this.GameEventType = gameEventType; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            // Compare the type of the other instance with the ignoring type of this instance.
            return obj.GetType() == this.GameEventType;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
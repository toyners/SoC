
using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests.ScenarioEvents
{
    internal class ScenarioRobbingChoicesEvent : GameEvent
    {
        public readonly Dictionary<Guid, int> RobbingChoices;
        public ScenarioRobbingChoicesEvent(Dictionary<Guid, int> robbingChoices) : base(Guid.Empty)
        {
            this.RobbingChoices = robbingChoices;
        }

        public override bool Equals(object obj)
        {
            if (!base.Equals(obj))
                return false;

            var other = (ScenarioRobbingChoicesEvent)obj;
            var otherSortedKeys = new List<Guid>(other.RobbingChoices.Keys);
            otherSortedKeys.Sort();

            var sortedKeys = new List<Guid>(this.RobbingChoices.Keys);
            sortedKeys.Sort();

            if (sortedKeys.Count != otherSortedKeys.Count)
                return false;

            foreach (var key in sortedKeys)
            {
                if (!other.RobbingChoices.ContainsKey(key))
                    return false;

                if (this.RobbingChoices[key] != other.RobbingChoices[key])
                    return false;
            }

            return true;
        }
    }
}

using System.Collections.Generic;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests.Instructions
{
    internal class MultipleEventInstruction : Instruction
    {
        private HashSet<GameEvent> events = new HashSet<GameEvent>();
        public void Add(GameEvent expectedEvent) => this.events.Add(expectedEvent);
    }
}

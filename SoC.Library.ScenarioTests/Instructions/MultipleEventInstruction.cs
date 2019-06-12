using System.Collections.Generic;
using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests.Instructions
{
    internal class MultipleEventInstruction : Instruction
    {
        public void Add(GameEvent expectedEvent) => this.Events.Add(expectedEvent);
        public HashSet<GameEvent> Events { get; private set; } = new HashSet<GameEvent>();
    }
}

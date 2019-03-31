
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using Jabberwocky.SoC.Library.GameEvents;

    [DebuggerDisplay("Event: {GetType().Name}")]
    internal abstract class EventInstruction : Instruction
    {
        public EventInstruction(string playerName) : base(playerName) {}

        public bool Verify { get; set; } = true;

        public abstract GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName);
    }
}

namespace SoC.Library.ScenarioTests.Instructions
{
    using System.Diagnostics;
    using Jabberwocky.SoC.Library.GameEvents;

    [DebuggerDisplay("Event: {GetType().Name}")]
    internal class EventInstruction : Instruction
    {
        private GameEvent expectedEvent;
        public EventInstruction(GameEvent expectedEvent) => this.expectedEvent = expectedEvent;

        public bool Verbose { get; set; }

        public GameEvent GetEvent() => this.expectedEvent;
    }
}
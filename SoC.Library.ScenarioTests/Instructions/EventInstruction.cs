
namespace SoC.Library.ScenarioTests.Instructions
{
    using System.Diagnostics;
    using Jabberwocky.SoC.Library.GameEvents;

    [DebuggerDisplay("Event: {GetType().Name}")]
    internal abstract class EventInstruction : Instruction
    {
        private GameEvent expectedEvent;
        public EventInstruction(string playerName, GameEvent expectedEvent) : base(playerName)
            => this.expectedEvent = expectedEvent;

        public bool Verify { get; set; } = true;

        public GameEvent GetEvent()
        {
            var gameEvent = this.expectedEvent;
            this.expectedEvent = null;
            return gameEvent;
        }
    }
}
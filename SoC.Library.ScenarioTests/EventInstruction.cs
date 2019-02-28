using Jabberwocky.SoC.Library.GameEvents;

namespace SoC.Library.ScenarioTests
{
    internal class EventInstruction : Instruction
    {
        public readonly GameEvent Event;
        public EventInstruction(string playerName, GameEvent gameEvent) : base(playerName)
        {
            this.Event = gameEvent;
        }
    }
}

namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using Jabberwocky.SoC.Library.GameEvents;

    public class PlayerOrderEvent : GameEvent
    {
        public PlayerOrderEvent(Guid[] playerTurnOrder) : base(Guid.Empty)
        {
        }
    }
}
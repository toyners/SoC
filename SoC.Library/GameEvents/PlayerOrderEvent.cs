
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using Jabberwocky.SoC.Library.GameEvents;

    public class PlayerOrderEvent : GameEvent
    {
        public PlayerOrderEvent(Guid playerId, Guid[] playerTurnOrder) : base(playerId)
        {
        }
    }
}

namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class PlayerOrderEventInstruction : EventInstruction
    {
        private Guid[] playerIds;

        public PlayerOrderEventInstruction(string playerName, Guid[] playerIds) : base(playerName)
        {
            this.playerIds = playerIds;
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new PlayerOrderEvent(this.playerIds);
        }
    }
}

namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class GameJoinedEventInstruction : EventInstruction
    {
        private Guid playerId;

        public GameJoinedEventInstruction(string playerName, Guid playerId) : base(playerName)
        {
            this.playerId = playerId;
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new GameJoinedEvent(this.playerId);
        }
    }
}
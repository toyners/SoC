
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class PlayerOrderEventInstruction : EventInstruction
    {
        private string[] playerNames;

        public PlayerOrderEventInstruction(string playerName, string[] playerNames) : base(playerName)
        {
            this.playerNames = playerNames;
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            var playerIds = this.playerNames.Select(playerName => playerIdsByName[playerName]).ToArray();
            return new PlayerOrderEvent(playerIdsByName[this.PlayerName], playerIds);
        }
    }
}
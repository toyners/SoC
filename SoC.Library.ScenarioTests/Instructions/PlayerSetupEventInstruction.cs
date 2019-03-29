
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class PlayerSetupEventInstruction : EventInstruction
    {
        private IDictionary<string, Guid> playerIdsByName;
        public PlayerSetupEventInstruction(string playerName, IDictionary<string, Guid> playerIdsByName) : base(playerName)
        {
            this.playerIdsByName = playerIdsByName;
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new PlayerSetupEvent(this.playerIdsByName);
        }
    }
}
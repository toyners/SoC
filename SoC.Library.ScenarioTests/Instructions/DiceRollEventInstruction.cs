
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class DiceRollEventInstruction : EventInstruction
    {
        private string playerName;
        private uint dice1;
        private uint dice2;

        public DiceRollEventInstruction(string playerName, uint dice1, uint dice2) : base(playerName)
        {
            this.playerName = playerName;
            this.dice1 = dice1;
            this.dice2 = dice2;
        }

        public DiceRollEventInstruction(string instructionName, string playerName, uint dice1, uint dice2) : base(instructionName)
        {
            this.playerName = playerName;
            this.dice1 = dice1;
            this.dice2 = dice2;
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new DiceRollEvent(playerIdsByName[this.PlayerName], this.dice1, this.dice2);
        }
    }
}

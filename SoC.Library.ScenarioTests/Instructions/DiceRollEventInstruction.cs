
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
        private DiceRollEvent diceRollEvent;

        public DiceRollEventInstruction(string playerName, DiceRollEvent expectedEvent) : base(playerName, expectedEvent) {}
    }
}

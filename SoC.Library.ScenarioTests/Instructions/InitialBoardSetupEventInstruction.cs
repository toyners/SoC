
namespace SoC.Library.ScenarioTests.Instructions
{
    using System;
    using System.Collections.Generic;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;

    internal class InitialBoardSetupEventInstruction : EventInstruction
    {
        private GameBoardSetup gameBoardSetup;

        public InitialBoardSetupEventInstruction(string playerName, GameBoardSetup gameBoardSetup) : base(playerName)
        {
            this.gameBoardSetup = gameBoardSetup;
        }

        public override GameEvent GetEvent(IDictionary<string, Guid> playerIdsByName)
        {
            return new InitialBoardSetupEvent(this.gameBoardSetup);
        }
    }
}
using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.GameBoards;
using Jabberwocky.SoC.Library.GameEvents;
using SoC.Library.ScenarioTests.ScenarioEvents;

namespace SoC.Library.ScenarioTests
{
    internal abstract class EventInstruction : Instruction
    {
        public EventInstruction(string playerName) : base(playerName) {}

        public abstract GameEvent Event(IDictionary<string, Guid> playerIdsByName);
    }

    internal class InitialBoardSetupEventInstruction : EventInstruction
    {
        private GameBoardSetup gameBoardSetup;

        public InitialBoardSetupEventInstruction(string playerName, GameBoardSetup gameBoardSetup) : base(playerName)
        {
            this.gameBoardSetup = gameBoardSetup;
        }

        public override GameEvent Event(IDictionary<string, Guid> playerIdsByName)
        {
            return new InitialBoardSetupEventArgs(this.gameBoardSetup);
        }
    }

    internal class PlaceSetupInfrastructureEventInstruction : EventInstruction
    {
        public PlaceSetupInfrastructureEventInstruction(string playerName) : base(playerName)
        {
        }

        public override GameEvent Event(IDictionary<string, Guid> playerIdsByName)
        {
            return new ScenarioPlaceSetupInfrastructureEvent();
        }
    }
}
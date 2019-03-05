﻿using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
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

    internal class PlayerSetupEventInstruction : EventInstruction
    {
        private IDictionary<string, Guid> playerIdsByName;
        public PlayerSetupEventInstruction(string playerName, IDictionary<string, Guid> playerIdsByName) : base(playerName)
        {
            this.playerIdsByName = playerIdsByName;
        }

        public override GameEvent Event(IDictionary<string, Guid> p)
        {
            return new PlayerSetupEvent(this.playerIdsByName);
        }
    }

    internal class TradeWithPlayerCompletedEventInstruction : EventInstruction
    {
        private string buyingPlayerName;
        private ResourceClutch buyingResources;
        private string sellingPlayerName;
        private ResourceClutch sellingResources;

        public TradeWithPlayerCompletedEventInstruction(string playerName, 
            string buyingPlayerName,
            ResourceClutch buyingResources,
            string sellingPlayerName,
            ResourceClutch sellingResources)
            : base(playerName)
        {
            this.buyingPlayerName = buyingPlayerName;
            this.buyingResources = buyingResources;
            this.sellingPlayerName = sellingPlayerName;
            this.sellingResources = sellingResources;
        }

        public override GameEvent Event(IDictionary<string, Guid> playerIdsByName)
        {
            return new TradeWithPlayerCompletedEvent(
                playerIdsByName[this.buyingPlayerName],
                this.buyingResources,
                playerIdsByName[this.sellingPlayerName],
                this.sellingResources);
        }
    }
}
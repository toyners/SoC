﻿
using System;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;
using NUnit.Framework;

namespace SoC.Library.ScenarioTests
{
    [TestFixture]
    public class ScenarioTests
    {
        const string MainPlayerName = "Player";
        const string FirstOpponentName = "Barbara";
        const string SecondOpponentName = "Charlie";
        const string ThirdOpponentName = "Dana";

        const uint MainPlayerFirstSettlementLocation = 12u;
        const uint FirstOpponentFirstSettlementLocation = 18u;
        const uint SecondOpponentFirstSettlementLocation = 25u;
        const uint ThirdOpponentFirstSettlementLocation = 31u;

        const uint ThirdOpponentSecondSettlementLocation = 33u;
        const uint SecondOpponentSecondSettlementLocation = 35u;
        const uint FirstOpponentSecondSettlementLocation = 43u;
        const uint MainPlayerSecondSettlementLocation = 40u;

        const uint MainPlayerFirstRoadEnd = 4;
        const uint FirstOpponentFirstRoadEnd = 17;
        const uint SecondOpponentFirstRoadEnd = 15;
        const uint ThirdOpponentFirstRoadEnd = 30;

        const uint ThirdOpponentSecondRoadEnd = 32;
        const uint SecondOpponentSecondRoadEnd = 24;
        const uint FirstOpponentSecondRoadEnd = 44;
        const uint MainPlayerSecondRoadEnd = 39;

        [Test]
        public void Test()
        {
            var localGameController = this.CreateStandardLocalGameControllerScenarioRunner()
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 2, 2).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 1, 1).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 2, 2).EndTurn()
                .Build()
                .ExpectingEvents()
                .DiceRollEvent(MainPlayerName, 4, 4)
                .DiceRollEvent(FirstOpponentName, 2, 2)
                .DiceRollEvent(SecondOpponentName, 1, 1)
                .DiceRollEvent(ThirdOpponentName, 2, 2)
                .Run();
        }

        [Test]
        public void Scenario_AllPlayersCollectResourcesAsPartOfFirstTurnStart()
        {
            var localGameController = this.CreateStandardLocalGameControllerScenarioRunner()
                .DuringPlayerTurn(MainPlayerName, 1, 5).EndTurn()
                .Build()
                .ExpectingEvents()
                .ResourcesCollectedEvent(FirstOpponentName, FirstOpponentFirstSettlementLocation, ResourceClutch.OneOre)
                .StartResourcesCollectedEvent(SecondOpponentName)
                    .AddResourceCollection(SecondOpponentFirstSettlementLocation, ResourceClutch.OneLumber)
                    .AddResourceCollection(SecondOpponentSecondSettlementLocation, ResourceClutch.OneLumber)
                    .FinishResourcesCollectedEvent()
                .ResourcesCollectedEvent(ThirdOpponentName, ThirdOpponentFirstSettlementLocation, ResourceClutch.OneOre)
                .Run();
        }

        private LocalGameControllerScenarioRunner CreateStandardLocalGameControllerScenarioRunner()
        {
            return LocalGameControllerScenarioRunner.LocalGameController()
                .WithMainPlayer(MainPlayerName)
                .WithComputerPlayer(FirstOpponentName).WithComputerPlayer(SecondOpponentName).WithComputerPlayer(ThirdOpponentName)
                .WithPlayerSetup(MainPlayerName, MainPlayerFirstSettlementLocation, MainPlayerFirstRoadEnd, MainPlayerSecondSettlementLocation, MainPlayerSecondRoadEnd)
                .WithPlayerSetup(FirstOpponentName, FirstOpponentFirstSettlementLocation, FirstOpponentFirstRoadEnd, FirstOpponentSecondSettlementLocation, FirstOpponentSecondRoadEnd)
                .WithPlayerSetup(SecondOpponentName, SecondOpponentFirstSettlementLocation, SecondOpponentFirstRoadEnd, SecondOpponentSecondSettlementLocation, SecondOpponentSecondRoadEnd)
                .WithPlayerSetup(ThirdOpponentName, ThirdOpponentFirstSettlementLocation, ThirdOpponentFirstRoadEnd, ThirdOpponentSecondSettlementLocation, ThirdOpponentSecondRoadEnd)
                .WithTurnOrder(MainPlayerName, FirstOpponentName, SecondOpponentName, ThirdOpponentName);
        }
    }
    

    internal class PlayerTurn
    {
        private readonly LocalGameControllerScenarioRunner localGameControllerScenarioBuilder;
        private readonly IPlayer player;

        public PlayerTurn(LocalGameControllerScenarioRunner localGameControllerScenarioBuilder, IPlayer player)
        {
            this.localGameControllerScenarioBuilder = localGameControllerScenarioBuilder;
            this.player = player;
        }

        public LocalGameControllerScenarioRunner EndTurn()
        {
            return this.localGameControllerScenarioBuilder;
        }
    }

    public class PlayerTurnAction
    {
        public readonly Guid Id;
        public PlayerTurnAction(Guid playerId) => this.Id = playerId;
    }

    public class PlayerTurnSetupAction : PlayerTurnAction
    {
        public readonly uint SettlementLocation;
        public readonly uint RoadEndLocation;
        public PlayerTurnSetupAction(Guid playerId, uint settlementLocation, uint roadEndLocation) : base(playerId)
        {
            this.SettlementLocation = settlementLocation;
            this.RoadEndLocation = roadEndLocation;
        }

    }
}

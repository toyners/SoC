
using System;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;
using NUnit.Framework;
using static SoC.Library.ScenarioTests.LocalGameControllerScenarioRunner;

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

        [Test]
        public void Scenario_AllPlayersCollectResourcesAsPartOfTurnStartAfterComputerPlayersCompleteTheirTurns()
        {
            var localGameController = this.CreateStandardLocalGameControllerScenarioRunner()
                .DuringPlayerTurn(MainPlayerName, 1, 2).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 4, 4).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 4, 4).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 4, 4).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .Build()
                .ExpectingEvents()
                .Events(EventTypes.ResourcesCollectedEvent, 4)
                .ResourcesCollectedEvent(SecondOpponentName, SecondOpponentSecondSettlementLocation, ResourceClutch.OneOre)
                .ResourcesCollectedEvent(MainPlayerName, MainPlayerFirstSettlementLocation, ResourceClutch.OneBrick)
                .ResourcesCollectedEvent(FirstOpponentName, FirstOpponentSecondSettlementLocation, ResourceClutch.OneGrain)
                .ResourcesCollectedEvent(MainPlayerName, MainPlayerFirstSettlementLocation, ResourceClutch.OneBrick)
                .ResourcesCollectedEvent(FirstOpponentName, FirstOpponentSecondSettlementLocation, ResourceClutch.OneGrain)
                .ResourcesCollectedEvent(MainPlayerName, MainPlayerFirstSettlementLocation, ResourceClutch.OneBrick)
                .ResourcesCollectedEvent(FirstOpponentName, FirstOpponentSecondSettlementLocation, ResourceClutch.OneGrain)
                .ResourcesCollectedEvent(FirstOpponentName, FirstOpponentFirstSettlementLocation, ResourceClutch.OneOre)
                .StartResourcesCollectedEvent(SecondOpponentName)
                    .AddResourceCollection(SecondOpponentFirstSettlementLocation, ResourceClutch.OneLumber)
                    .AddResourceCollection(SecondOpponentSecondSettlementLocation, ResourceClutch.OneLumber)
                    .FinishResourcesCollectedEvent()
                .ResourcesCollectedEvent(ThirdOpponentName, ThirdOpponentFirstSettlementLocation, ResourceClutch.OneOre)
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerBuildsSettlement()
        {
            this.CreateStandardLocalGameControllerScenarioRunner()
                .DuringPlayerTurn(MainPlayerName, 2, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 2, 2).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 1, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 2, 3)
                    .BuildRoad(32, 42)
                    .BuildSettlement(42).EndTurn()
                .Build()
                .ExpectingEvents()
                .BuildRoadEvent(ThirdOpponentName, 32, 42)
                .BuildSettlementEvent(ThirdOpponentName, 42)
                .Run();
        }

        private LocalGameControllerScenarioRunner CreateStandardLocalGameControllerScenarioRunner()
        {
            return LocalGameController()
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
        private readonly LocalGameControllerScenarioRunner runner;
        private readonly IPlayer player;

        public Guid PlayerId { get { return this.player.Id; } }

        public PlayerTurn(LocalGameControllerScenarioRunner runner, IPlayer player)
        {
            this.runner = runner;
            this.player = player;
        }

        public LocalGameControllerScenarioRunner EndTurn()
        {
            return this.runner;
        }

        public PlayerTurn BuildRoad(int roadSegmentStart, int roadSegmentEnd)
        {
            throw new NotImplementedException();
        }

        public PlayerTurn BuildSettlement(int settlementLocation)
        {
            throw new NotImplementedException();
        }
    }

    internal class ComputerPlayerTurn : PlayerTurn
    {
        public ComputerPlayerTurn(LocalGameControllerScenarioRunner runner, IPlayer player) : base(runner, player)
        {
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

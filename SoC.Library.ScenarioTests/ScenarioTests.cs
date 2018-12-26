
using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.GameEvents;
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
        public void Scenario_AllPlayersHaveDiceRollEventInTheirTurn()
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
                .IgnoredEvents(typeof(ResourcesCollectedEvent), 4)
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
                .DuringPlayerTurn(ThirdOpponentName, 2, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 2, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 2, 2).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 1, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 2, 3)
                    .BuildRoad(33, 22)
                    .BuildRoad(22, 23)
                    .BuildSettlement(23).EndTurn()
                .Build()
                .ExpectingEvents()
                .BuildRoadEvent(ThirdOpponentName, 33, 22)
                .BuildRoadEvent(ThirdOpponentName, 22, 23)
                .BuildSettlementEvent(ThirdOpponentName, 23)
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerBuildsSettlementToWin()
        {
            var firstOpponentResources = (ResourceClutch.RoadSegment * 5) + (ResourceClutch.Settlement * 3) + (ResourceClutch.City * 3);
            this.CreateStandardLocalGameControllerScenarioRunner(ResourceClutch.Zero, firstOpponentResources, ResourceClutch.Zero, ResourceClutch.Zero)
                .DuringPlayerTurn(MainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3)
                    .BuildRoad(17, 7).BuildRoad(7, 8).BuildRoad(8, 0).BuildRoad(0, 1).BuildRoad(8, 9)
                    .BuildSettlement(7).BuildSettlement(0)
                    .BuildCity(7).BuildCity(0).BuildCity(18)
                    .BuildSettlement(9).EndTurn()
                .Build()
                .ExpectingEvents()
                    .DiceRollEvent(MainPlayerName, 4, 4)
                    .DiceRollEvent(FirstOpponentName, 3, 3)
                    .BuildRoadEvent(FirstOpponentName, 17, 7).BuildRoadEvent(FirstOpponentName, 7, 8).BuildRoadEvent(FirstOpponentName, 8, 0).BuildRoadEvent(FirstOpponentName, 0, 1).BuildRoadEvent(FirstOpponentName, 8, 9)
                    .BuildSettlementEvent(FirstOpponentName, 7).BuildSettlementEvent(FirstOpponentName, 0)
                    .BuildCityEvent(FirstOpponentName, 7).BuildCityEvent(FirstOpponentName, 0).BuildCityEvent(FirstOpponentName, 18)
                    .BuildSettlementEvent(FirstOpponentName, 9)
                    .GameWonEvent(FirstOpponentName, 10)
                .Run();
        }

        [Test]
        public void Scenario_ComputerPlayerBuildsCity()
        {
            this.CreateStandardLocalGameControllerScenarioRunner()
                .DuringPlayerTurn(MainPlayerName, 3, 3).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 2, 3).EndTurn()
                .DuringPlayerTurn(MainPlayerName, 5, 6).EndTurn()
                .DuringPlayerTurn(FirstOpponentName, 5, 6).EndTurn()
                .DuringPlayerTurn(SecondOpponentName, 3, 3).EndTurn()
                .DuringPlayerTurn(ThirdOpponentName, 3, 3)
                    .BuildCity(33).EndTurn()
                .Build()
                .ExpectingEvents()
                .BuildCityEvent(ThirdOpponentName, 33)
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

        private LocalGameControllerScenarioRunner CreateStandardLocalGameControllerScenarioRunner(ResourceClutch mainPlayerResources, ResourceClutch firstOpponentResources, ResourceClutch secondOpponentResources, ResourceClutch thirdOpponentResources)
        {
            return LocalGameController()
                .WithMainPlayer(MainPlayerName)
                .WithComputerPlayer(FirstOpponentName).WithComputerPlayer(SecondOpponentName).WithComputerPlayer(ThirdOpponentName)
                .WithPlayerSetup(MainPlayerName, MainPlayerFirstSettlementLocation, MainPlayerFirstRoadEnd, MainPlayerSecondSettlementLocation, MainPlayerSecondRoadEnd)
                .WithPlayerSetup(FirstOpponentName, FirstOpponentFirstSettlementLocation, FirstOpponentFirstRoadEnd, FirstOpponentSecondSettlementLocation, FirstOpponentSecondRoadEnd)
                .WithPlayerSetup(SecondOpponentName, SecondOpponentFirstSettlementLocation, SecondOpponentFirstRoadEnd, SecondOpponentSecondSettlementLocation, SecondOpponentSecondRoadEnd)
                .WithPlayerSetup(ThirdOpponentName, ThirdOpponentFirstSettlementLocation, ThirdOpponentFirstRoadEnd, ThirdOpponentSecondSettlementLocation, ThirdOpponentSecondRoadEnd)
                .WithStartingResourcesForPlayer(MainPlayerName, mainPlayerResources)
                .WithStartingResourcesForPlayer(FirstOpponentName, firstOpponentResources)
                .WithStartingResourcesForPlayer(SecondOpponentName, secondOpponentResources)
                .WithStartingResourcesForPlayer(ThirdOpponentName, thirdOpponentResources)
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

        public virtual PlayerTurn BuildRoad(uint roadSegmentStart, uint roadSegmentEnd)
        {
            throw new NotImplementedException();
        }

        public virtual PlayerTurn BuildSettlement(uint settlementLocation)
        {
            throw new NotImplementedException();
        }

        internal virtual PlayerTurn BuildCity(uint cityLocation)
        {
            throw new NotImplementedException();
        }
    }

    internal class ComputerPlayerTurn : PlayerTurn
    {
        private MockComputerPlayer computerPlayer;
        private IList<ComputerPlayerAction> actions = new List<ComputerPlayerAction>();

        public ComputerPlayerTurn(LocalGameControllerScenarioRunner runner, IPlayer player) : base(runner, player)
        {
            this.computerPlayer = (MockComputerPlayer)player;
        }

        internal override PlayerTurn BuildCity(uint cityLocation)
        {
            this.actions.Add(new BuildCityAction(cityLocation));
            return this;
        }

        public override PlayerTurn BuildRoad(uint roadSegmentStart, uint roadSegmentEnd)
        {
            this.actions.Add(new BuildRoadSegmentAction(roadSegmentStart, roadSegmentEnd));
            return this;
        }

        public override PlayerTurn BuildSettlement(uint settlementLocation)
        {
            this.actions.Add(new BuildSettlementAction(settlementLocation));
            return this;
        }

        public void ResolveActions()
        {
            this.computerPlayer.AddActions(this.actions);
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

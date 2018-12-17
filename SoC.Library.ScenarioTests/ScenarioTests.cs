
using System;
using Jabberwocky.SoC.Library.Interfaces;
using NUnit.Framework;

namespace SoC.Library.ScenarioTests
{
    [TestFixture]
    public class ScenarioTests
    {
        [Test]
        public void Test()
        {
            var mainPlayerName = "Player";
            var firstOpponentName = "Barbara";
            var secondOpponentName = "Charlie";
            var thirdOpponentName = "Dana";

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

            var localGameController = LocalGameControllerScenarioRunner.LocalGameController()
                .WithMainPlayer(mainPlayerName)
                .WithComputerPlayer(firstOpponentName).WithComputerPlayer(secondOpponentName).WithComputerPlayer(thirdOpponentName)
                .WithPlayerSetup(mainPlayerName, MainPlayerFirstSettlementLocation, MainPlayerFirstRoadEnd, MainPlayerSecondSettlementLocation, MainPlayerSecondRoadEnd)
                .WithPlayerSetup(firstOpponentName, FirstOpponentFirstSettlementLocation, FirstOpponentFirstRoadEnd, FirstOpponentSecondSettlementLocation, FirstOpponentSecondRoadEnd)
                .WithPlayerSetup(secondOpponentName, SecondOpponentFirstSettlementLocation, SecondOpponentFirstRoadEnd, SecondOpponentSecondSettlementLocation, SecondOpponentSecondRoadEnd)
                .WithPlayerSetup(thirdOpponentName, ThirdOpponentFirstSettlementLocation, ThirdOpponentFirstRoadEnd, ThirdOpponentSecondSettlementLocation, ThirdOpponentSecondRoadEnd)
                .WithTurnOrder(mainPlayerName, firstOpponentName, secondOpponentName, thirdOpponentName)
                .DuringPlayerTurn(mainPlayerName, 4, 4).EndTurn()
                .DuringPlayerTurn(firstOpponentName, 2, 2).EndTurn()
                .DuringPlayerTurn(secondOpponentName, 1, 1).EndTurn()
                .DuringPlayerTurn(thirdOpponentName, 2, 2).EndTurn()
                .Build()
                .ExpectingEvents()
                .DiceRollEvent(mainPlayerName, 4, 4)
                .DiceRollEvent(firstOpponentName, 2, 2)
                .DiceRollEvent(secondOpponentName, 1, 1)
                .DiceRollEvent(thirdOpponentName, 2, 2)
                .Run();
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

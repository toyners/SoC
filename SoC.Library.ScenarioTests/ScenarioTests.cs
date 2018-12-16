
using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;

namespace SoC.Library.ScenarioTests
{
    public class ScenarioTests
    {
        public void Test()
        {
            var mainPlayerId = Guid.NewGuid();
            var firstOpponentId = Guid.NewGuid();
            var secondOpponentId = Guid.NewGuid();
            var thirdOpponentId = Guid.NewGuid();
            var localGameController = LocalGameControllerScenarioRunner.LocalGameController()
                .WithMainPlayer("Player")
                .WithComputerPlayer("Barbara").WithComputerPlayer("Charlie").WithComputerPlayer("Dana")
                .WithPlayerSetup("Player", 0u, 1u, 2u, 3u)
                .WithPlayerSetup("Barbara", 0u, 1u, 2u, 3u)
                .WithPlayerSetup("Charlie", 0u, 1u, 2u, 3u)
                .WithPlayerSetup("Dana", 0u, 1u, 2u, 3u)
                .WithTurnOrder("Player", "Barbara", "Charlie", "Dana")
                .DuringPlayerTurn("Player", 1, 1).EndTurn()
                .DuringPlayerTurn("Barbara", 2, 2).EndTurn()
                .BuildAndRun();
        }
    }
    

    internal class PlayerTurn
    {
        private readonly LocalGameControllerScenarioRunner localGameControllerScenarioBuilder;

        public PlayerTurn(LocalGameControllerScenarioRunner localGameControllerScenarioBuilder) => this.localGameControllerScenarioBuilder = localGameControllerScenarioBuilder;

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

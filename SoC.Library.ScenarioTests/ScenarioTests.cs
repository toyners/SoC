
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
                .WithMainPlayer(mainPlayerId)
                .WithComputerPlayer(firstOpponentId).WithComputerPlayer(secondOpponentId).WithComputerPlayer(thirdOpponentId)
                .WithPlayerSetup(mainPlayerId, 0u, 1u, 2u, 3u)
                .WithPlayerSetup(firstOpponentId, 0u, 1u, 2u, 3u)
                .WithPlayerSetup(secondOpponentId, 0u, 1u, 2u, 3u)
                .WithPlayerSetup(thirdOpponentId, 0u, 1u, 2u, 3u)
                .WithTurnOrder(mainPlayerId, firstOpponentId, secondOpponentId, thirdOpponentId)
                .DuringPlayerTurn(mainPlayerId, 1, 1).EndTurn()
                .DuringPlayerTurn(firstOpponentId, 2, 2).EndTurn()
                .BuildAndRun();
        }
    }

    public class LocalGameControllerScenarioRunner
    {
        public enum EventTypes
        {
            DiceRollEvent
        }

        private Guid mainPlayerId;
        private readonly List<Guid> opponentPlayerId = new List<Guid>(3);
        private readonly MockPlayerPool mockPlayerPool = new MockPlayerPool();
        private PlaceInfrastructureInstruction firstPlaceInfrastructureInstruction;
        private PlaceInfrastructureInstruction secondPlaceInfrastructureInstruction;
        private readonly List<PlayerTurnSetupAction> FirstRoundSetupActions = new List<PlayerTurnSetupAction>(4);
        private readonly List<PlayerTurnSetupAction> SecondRoundSetupActions = new List<PlayerTurnSetupAction>(4);
        private readonly Dictionary<EventTypes, Delegate> eventHandlers;
        private readonly List<PlayerTurn> playerTurns = new List<PlayerTurn>();

        private static LocalGameControllerScenarioRunner localGameControllerScenarioBuilder;

        public static LocalGameControllerScenarioRunner LocalGameController(Dictionary<EventTypes, Delegate> eventHandlers = null)
        {
            return localGameControllerScenarioBuilder = new LocalGameControllerScenarioRunner(eventHandlers);
        }

        private LocalGameControllerScenarioRunner(Dictionary<EventTypes, Delegate> eventHandlers)
        {
            this.eventHandlers = eventHandlers;
        }

        public LocalGameController BuildAndRun()
        {
            var localGameController = new LocalGameController(null, this.mockPlayerPool);

            if (this.eventHandlers != null)
            {
                if (this.eventHandlers.TryGetValue(EventTypes.DiceRollEvent, out var eventHandler))
                    localGameController.DiceRollEvent = (Action<uint, uint>)eventHandler;
            }

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();

            return localGameController;
        }

        public LocalGameControllerScenarioRunner WithMainPlayer(Guid id)
        {
            this.mainPlayerId = id;
            this.mockPlayerPool.AddPlayer(id, false);
            return this;
        }

        public LocalGameControllerScenarioRunner WithComputerPlayer(Guid id)
        {
            this.opponentPlayerId.Add(id);
            this.mockPlayerPool.AddPlayer(id, true);
            return this;
        }

        public LocalGameControllerScenarioRunner WithPlayerSetup(Guid playerId, uint firstSettlementLocation, uint firstRoadEndLocation, uint secondSettlementLocation, uint secondRoadEndLocation)
        {
            if (playerId == this.mainPlayerId)
            {
                this.firstPlaceInfrastructureInstruction = new PlaceInfrastructureInstruction(playerId, firstSettlementLocation, firstRoadEndLocation);
                this.secondPlaceInfrastructureInstruction = new PlaceInfrastructureInstruction(playerId, secondSettlementLocation, secondRoadEndLocation);
            }
            else
            {
                this.mockPlayerPool.ComputerPlayers[playerId].AddInstructions(
                    new PlaceInfrastructureInstruction(playerId, firstSettlementLocation, firstRoadEndLocation),
                    new PlaceInfrastructureInstruction(playerId, secondSettlementLocation, secondRoadEndLocation));
            }

            return this;
        }

        public LocalGameControllerScenarioRunner WithTurnOrder(Guid mainPlayer, Guid firstOpponent, Guid secondOpponent, Guid thirdOpponent)
        {
            throw new NotImplementedException();
        }

        public PlayerTurn DuringPlayerTurn(Guid playerId, uint dice1, uint dice2)
        {
            var playerTurn = new PlayerTurn(this);

            this.playerTurns.Add(playerTurn);

            return playerTurn;
        }
    }

    public class PlayerTurn
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

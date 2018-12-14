
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
            var localGameController = new LocalGameController(null, null);

            if (this.eventHandlers != null)
            {
                if (this.eventHandlers.TryGetValue(EventTypes.DiceRollEvent, out var eventHandler))
                    localGameController.DiceRollEvent = (Action<uint, uint>)eventHandler;
            }

            localGameController.JoinGame();
            localGameController.LaunchGame();
            localGameController.StartGameSetup();
            //localGameController.ContinueGameSetup()

            return localGameController;
        }

        public LocalGameControllerScenarioRunner WithMainPlayer(Guid id)
        {
            this.mainPlayerId = id;
            return this;
        }

        public LocalGameControllerScenarioRunner WithComputerPlayer(Guid id)
        {
            this.opponentPlayerId.Add(id);
            return this;
        }

        public LocalGameControllerScenarioRunner WithPlayerSetup(Guid mainPlayer, uint firstSettlementLocation, uint firstRoadEndLocation, uint secondSettlementLocation, uint secondRoadEndLocation)
        {
            throw new NotImplementedException();
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
}

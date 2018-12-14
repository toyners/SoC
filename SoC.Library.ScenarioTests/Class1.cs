
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
            var localGameController = LocalGameControllerScenarioBuilder.LocalGameController()
                .WithMainPlayer(mainPlayerId)
                .WithComputerPlayer(firstOpponentId).WithComputerPlayer(secondOpponentId).WithComputerPlayer(thirdOpponentId)
                .WithPlayerSetup(mainPlayerId, 0u, 1u, 2u, 3u)
                .WithPlayerSetup(firstOpponentId, 0u, 1u, 2u, 3u)
                .WithPlayerSetup(secondOpponentId, 0u, 1u, 2u, 3u)
                .WithPlayerSetup(thirdOpponentId, 0u, 1u, 2u, 3u)
                .WithTurnOrder(mainPlayerId, firstOpponentId, secondOpponentId, thirdOpponentId)
                .DuringTurn()
                .Build();
        }
    }

    public class LocalGameControllerScenarioBuilder
    {
        private Dictionary<string, ResourceClutch> startingResources = new Dictionary<string, ResourceClutch>();
        private Guid mainPlayerId;
        private List<Guid> opponentPlayerId = new List<Guid>();

        private static LocalGameControllerScenarioBuilder localGameControllerScenarioBuilder;

        public static LocalGameControllerScenarioBuilder LocalGameController()
        {
            return localGameControllerScenarioBuilder = new LocalGameControllerScenarioBuilder();
        }

        public LocalGameController Build()
        {
            return null;
        }

        public LocalGameControllerScenarioBuilder WithMainPlayer(Guid id)
        {
            this.mainPlayerId = id;
            return this;
        }

        public LocalGameControllerScenarioBuilder WithComputerPlayer(Guid id)
        {
            this.opponentPlayerId.Add(id);
            return this;
        }

        public LocalGameControllerScenarioBuilder WithPlayerSetup(Guid mainPlayer, uint firstSettlementLocation, uint firstRoadEndLocation, uint secondSettlementLocation, uint secondRoadEndLocation)
        {
            throw new NotImplementedException();
        }

        public LocalGameControllerScenarioBuilder WithTurnOrder(Guid mainPlayer, Guid firstOpponent, Guid secondOpponent, Guid thirdOpponent)
        {
            throw new NotImplementedException();
        }

        public LocalGameControllerScenarioBuilder DuringTurn()
        {
            throw new NotImplementedException();
        }
    }
}

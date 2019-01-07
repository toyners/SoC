using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;
using SoC.Library.ScenarioTests.PlayerTurn;

namespace Jabberwocky.SoC.Library.ScenarioTests.Builders
{
    internal class ExpectedEventsBuilder
    {
        private BasePlayerTurn playerTurn;
        public List<GameEvent> expectedEvents = new List<GameEvent>();
        private Dictionary<string, IPlayer> playersByName;
        public ExpectedEventsBuilder(BasePlayerTurn playerTurn, Dictionary<string, IPlayer> playersByName)
        {
            this.playerTurn = playerTurn;
            this.playersByName = playersByName;
        }

        public ExpectedEventsBuilder BuyDevelopmentCardEvent()
        {
            this.expectedEvents.Add(new BuyDevelopmentCardEvent(this.playerTurn.PlayerId));
            return this;
        }

        public BasePlayerTurn End()
        {
            return this.playerTurn;
        }

        public ExpectedEventsBuilder DiceRollEvent(uint dice1, uint dice2)
        {
            this.expectedEvents.Add(new DiceRollEvent(this.playerTurn.PlayerId, dice1, dice2));
            return this;
        }

        public ExpectedEventsBuilder ResourceCollectionEvent(string playerName, params Tuple<uint, ResourceClutch>[] resourceCollectionPairs)
        {
            var playerId = this.playersByName[playerName].Id;
            this.AddResourceCollectionEvent(playerId, resourceCollectionPairs);
            return this;
        }

        public ExpectedEventsBuilder ResourceCollectionEvent(params Tuple<uint, ResourceClutch>[] resourceCollectionPairs)
        {
            this.AddResourceCollectionEvent(this.playerTurn.PlayerId, resourceCollectionPairs);
            return this;
        }

        private void AddResourceCollectionEvent(Guid playerId, Tuple<uint, ResourceClutch>[] resourceCollectionPairs)
        {
            ResourceCollection[] rc = new ResourceCollection[resourceCollectionPairs.Length];
            var index = 0;
            foreach (var pair in resourceCollectionPairs)
                rc[index++] = new ResourceCollection(pair.Item1, pair.Item2);
            this.expectedEvents.Add(new ResourcesCollectedEvent(playerId, rc));
        }

        public ExpectedEventsBuilder BuildRoadEvent(uint roadSegmentStart, uint roadSegmentEnd)
        {
            this.expectedEvents.Add(new RoadSegmentBuiltEvent(this.playerTurn.PlayerId, roadSegmentStart, roadSegmentEnd));
            return this;
        }

        public ExpectedEventsBuilder BuildSettlementEvent(uint settlementLocation)
        {
            this.expectedEvents.Add(new SettlementBuiltEvent(this.playerTurn.PlayerId, settlementLocation));
            return this;
        }
    }
}

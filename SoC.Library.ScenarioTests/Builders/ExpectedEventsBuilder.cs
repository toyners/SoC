using System;
using System.Collections.Generic;
using System.Linq;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;
using SoC.Library.ScenarioTests;
using SoC.Library.ScenarioTests.PlayerTurn;
using SoC.Library.ScenarioTests.ScenarioEvents;

namespace Jabberwocky.SoC.Library.ScenarioTests.Builders
{
    internal class ExpectedEventsBuilder
    {
        #region Fields
        private BasePlayerTurn playerTurn;
        public List<GameEvent> expectedEvents = new List<GameEvent>();
        public List<Type> unwantedEventTypes = new List<Type>();
        private Dictionary<string, IPlayer> playersByName;
        #endregion

        #region Construction
        public ExpectedEventsBuilder(BasePlayerTurn playerTurn, Dictionary<string, IPlayer> playersByName)
        {
            this.playerTurn = playerTurn;
            this.playersByName = playersByName;
        }
        #endregion

        #region Methods
        public ExpectedEventsBuilder BuildCityEvent(uint cityLocation)
        {
            this.expectedEvents.Add(new CityBuiltEvent(this.playerTurn.PlayerId, cityLocation));
            return this;
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

        public ExpectedEventsBuilder BuyDevelopmentCardEvent()
        {
            this.expectedEvents.Add(new BuyDevelopmentCardEvent(this.playerTurn.PlayerId));
            return this;
        }

        public ExpectedEventsBuilder BuyDevelopmentCardEvent(DevelopmentCardTypes developmentCardType)
        {
            if (this.playerTurn.player is ScenarioPlayer mockPlayer)
            {

            }
            else if (this.playerTurn.player is ScenarioComputerPlayer scenarioComputerPlayer)
            {
                var expectedBuyDevelopmentCardEvent = new ScenarioBuyDevelopmentCardEvent(scenarioComputerPlayer, developmentCardType);
                this.expectedEvents.Add(expectedBuyDevelopmentCardEvent);
            }

            return this;
        }

        public ExpectedEventsBuilder DiceRollEvent(uint dice1, uint dice2)
        {
            this.expectedEvents.Add(new DiceRollEvent(this.playerTurn.PlayerId, dice1, dice2));
            return this;
        }

        public BasePlayerTurn End()
        {
            return this.playerTurn;
        }

        public ExpectedEventsBuilder GameWinEvent(uint expectedVictoryPoints)
        {
            this.expectedEvents.Add(new GameWinEvent(this.playerTurn.PlayerId, expectedVictoryPoints));
            return this;
        }

        public ExpectedEventsBuilder LargestArmyChangedEvent(string previousPlayerName = null)
        {
            Guid previousPlayerId = Guid.Empty;
            if (previousPlayerName != null)
                previousPlayerId = this.playersByName[previousPlayerName].Id;
            var expectedLargestArmyChangedEvent = new LargestArmyChangedEvent(this.playerTurn.PlayerId, previousPlayerId);
            this.expectedEvents.Add(expectedLargestArmyChangedEvent);

            return this;
        }

        public ExpectedEventsBuilder LongestRoadBuiltEvent()
        {
            var expectedLongestRoadBuiltEvent = new LongestRoadBuiltEvent(this.playerTurn.PlayerId, Guid.Empty);
            this.expectedEvents.Add(expectedLongestRoadBuiltEvent);
            return this;
        }

        public ExpectedEventsBuilder NoEventOfType<T>()
        {
            var type = typeof(T);
            this.unwantedEventTypes.Add(type);
            return this;
        }

        public ExpectedEventsBuilder PlayKnightCardEvent()
        {
            var expectedPlayKnightCardEvent = new PlayKnightCardEvent(this.playerTurn.PlayerId);
            this.expectedEvents.Add(expectedPlayKnightCardEvent);
            return this;
        }

        public ExpectedEventsBuilder ResourceCollectedEvent(string playerName, params Tuple<uint, ResourceClutch>[] resourceCollectionPairs)
        {
            var playerId = this.playersByName[playerName].Id;
            this.AddResourceCollectionEvent(playerId, resourceCollectionPairs);
            return this;
        }

        public ExpectedEventsBuilder ResourceCollectedEvent(params Tuple<uint, ResourceClutch>[] resourceCollectionPairs)
        {
            this.AddResourceCollectionEvent(this.playerTurn.PlayerId, resourceCollectionPairs);
            return this;
        }

        public ExpectedEventsBuilder ResourcesGainedEvent(string givingPlayerName, ResourceClutch expectedResources)
        {
            var givingPlayer = this.playersByName[givingPlayerName];
            var resourceTransaction = new ResourceTransaction(this.playerTurn.PlayerId, givingPlayer.Id, expectedResources);
            var expectedResourceTransactonEvent = new ResourceTransactionEvent(this.playerTurn.PlayerId, resourceTransaction);
            this.expectedEvents.Add(expectedResourceTransactonEvent);
            return this;
        }

        public ExpectedEventsBuilder RobberEvent(int expectedResourcesToDropCount)
        {
            var expectedRobberEvent = new ScenarioRobberEvent(expectedResourcesToDropCount);
            this.expectedEvents.Add(expectedRobberEvent);
            return this;
        }

        public ExpectedEventsBuilder RobbingChoicesEvent(params Tuple<string, int>[] playerResourceCount)
        {
            var robbingChoices = playerResourceCount.ToDictionary(t => this.playersByName[t.Item1].Id, t => t.Item2);
            this.expectedEvents.Add(new ScenarioRobbingChoicesEvent(robbingChoices));
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
        #endregion
    }
}

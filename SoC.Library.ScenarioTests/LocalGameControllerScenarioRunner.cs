using System;
using System.Collections.Generic;
using System.Linq;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
using Jabberwocky.SoC.Library.GameBoards;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;
using NUnit.Framework;

namespace SoC.Library.ScenarioTests
{
    internal class LocalGameControllerScenarioRunner
    {
        #region Fields
        private static LocalGameControllerScenarioRunner localGameControllerScenarioBuilder;
        private readonly MockPlayerPool mockPlayerPool = new MockPlayerPool();
        private readonly Queue<Instruction> playerInstructions = new Queue<Instruction>();
        private readonly List<PlayerTurnSetupAction> FirstRoundSetupActions = new List<PlayerTurnSetupAction>(4);
        private readonly List<PlayerTurnSetupAction> SecondRoundSetupActions = new List<PlayerTurnSetupAction>(4);
        private readonly List<PlayerTurn> playerTurns = new List<PlayerTurn>();
        private readonly MockDevelopmentCardHolder mockDevelopmentCardHolder = new MockDevelopmentCardHolder();
        private readonly MockNumberGenerator mockNumberGenerator = new MockNumberGenerator();
        private readonly Dictionary<string, IPlayer> playersByName = new Dictionary<string, IPlayer>();
        private readonly Dictionary<string, MockComputerPlayer> computerPlayersByName = new Dictionary<string, MockComputerPlayer>();
        private readonly List<IPlayer> players = new List<IPlayer>(4);
        private TurnToken currentToken;
        private int expectedEventCount;
        private LocalGameController localGameController = null;
        private List<GameEvent> actualEvents = null;
        private Queue<GameEvent> relevantEvents = null;
        private readonly Dictionary<Guid, List<DevelopmentCardTypes>> developmentCardsByPlayerId = new Dictionary<Guid, List<DevelopmentCardTypes>>();
        #endregion

        #region Construction
        private LocalGameControllerScenarioRunner() {}
        #endregion

        #region Methods
        public static LocalGameControllerScenarioRunner LocalGameController()
        {
            return localGameControllerScenarioBuilder = new LocalGameControllerScenarioRunner();
        }

        public LocalGameControllerScenarioRunner Build(int expectedEventCount = -1)
        {
            this.localGameController = new LocalGameController(
                this.mockNumberGenerator, 
                this.mockPlayerPool, 
                new GameBoard(BoardSizes.Standard), 
                this.mockDevelopmentCardHolder);
            this.localGameController.DiceRollEvent = this.DiceRollEventHandler;
            this.localGameController.GameEvents = this.GameEventsHandler;
            this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { this.currentToken = t; };

            this.expectedEventCount = expectedEventCount;
            this.relevantEvents = new Queue<GameEvent>();
            this.actualEvents = new List<GameEvent>();

            return this;
        }

        public LocalGameControllerScenarioRunner BuildCityEvent(string playerName, uint cityLocation)
        {
            var playerId = this.playersByName[playerName].Id;
            this.relevantEvents.Enqueue(new CityBuiltEvent(playerId, cityLocation));
            return this;
        }

        public LocalGameControllerScenarioRunner BuildRoadEvent(string playerName, uint roadSegmentStart, uint roadSegmentEnd)
        {
            var playerId = this.playersByName[playerName].Id;
            this.relevantEvents.Enqueue(new RoadSegmentBuiltEvent(playerId, roadSegmentStart, roadSegmentEnd));
            return this;
        }

        public LocalGameControllerScenarioRunner BuildSettlementEvent(string playerName, uint settlementLocation)
        {
            var playerId = this.playersByName[playerName].Id;
            this.relevantEvents.Enqueue(new SettlementBuiltEvent(playerId, settlementLocation));
            return this;
        }

        public LocalGameControllerScenarioRunner DiceRollEvent(string playerName, uint dice1, uint dice2)
        {
            var player = this.playersByName[playerName];

            var expectedDiceRollEvent = new DiceRollEvent(player.Id, dice1, dice2);
            this.relevantEvents.Enqueue(expectedDiceRollEvent);

            return this;
        }

        public LocalGameControllerScenarioRunner IgnoredEvents(Type matchingType, uint count)
        {
            while (count-- > 0)
                this.relevantEvents.Enqueue(new IgnoredEvent(matchingType));

            return this;
        }

        public LocalGameControllerScenarioRunner IgnoredEvent(Type matchingType)
        {
            return this.IgnoredEvents(matchingType, 1);
        }

        public LocalGameControllerScenarioRunner ResourcesCollectedEvent(string playerName, uint location, ResourceClutch resourceClutch)
        {
            var player = this.playersByName[playerName];

            return this.ResourcesCollectedEvent(player.Id, new[] { new ResourceCollection(location, resourceClutch) });
        }

        public LocalGameControllerScenarioRunner ResourcesCollectedEvent(Guid playerId, ResourceCollection[] resourceCollection)
        {
            var expectedDiceRollEvent = new ResourcesCollectedEvent(playerId, resourceCollection);
            this.relevantEvents.Enqueue(expectedDiceRollEvent);
            return this;
        }

        public LocalGameController Run()
        {
            this.localGameController.JoinGame();
            this.localGameController.LaunchGame();
            this.localGameController.StartGameSetup();

            var placeInfrastructureInstruction = (PlaceInfrastructureInstruction)this.playerInstructions.Dequeue();
            this.localGameController.ContinueGameSetup(placeInfrastructureInstruction.SettlementLocation, placeInfrastructureInstruction.RoadEndLocation);

            placeInfrastructureInstruction = (PlaceInfrastructureInstruction)this.playerInstructions.Dequeue();
            this.localGameController.CompleteGameSetup(placeInfrastructureInstruction.SettlementLocation, placeInfrastructureInstruction.RoadEndLocation);

            this.localGameController.StartGamePlay();

            var turns = new Queue<PlayerTurn>(this.playerTurns);

            do
            {
                var turn = turns.Dequeue();

                if (turn is ComputerPlayerTurn)
                {
                    ((ComputerPlayerTurn)turn).ResolveActions();
                }
                else if (turn is PlayerTurn playerTurn)
                {
                    // Do the player turns and then the computer turns for this round
                    var computerPlayerTurns = 3;
                    while (computerPlayerTurns-- > 0 && turns.Count > 0)
                    {
                        var computerPlayerTurn = (ComputerPlayerTurn)turns.Dequeue();
                        computerPlayerTurn.ResolveActions();
                    }

                    this.localGameController.EndTurn(this.currentToken);
                }
            } while (turns.Count > 0);

            if (this.relevantEvents != null && this.actualEvents != null)
            {
                if (this.expectedEventCount != -1)
                    Assert.AreEqual(this.expectedEventCount, this.actualEvents.Count, $"Expected event count {this.expectedEventCount} but found actual event count {this.actualEvents.Count}");

                var actualEventIndex = 0;
                while (this.relevantEvents.Count > 0)
                {
                    var expectedEvent = this.relevantEvents.Dequeue();
                    var foundEvent = false;
                    while (actualEventIndex < this.actualEvents.Count)
                    {
                        if (expectedEvent.Equals(this.actualEvents[actualEventIndex++]))
                        {
                            foundEvent = true;
                            break;
                        }
                    }

                    if (!foundEvent)
                        Assert.Fail("Expected event '{0}' not found", expectedEvent);
                }
            }

            return this.localGameController;
        }

        public LocalGameControllerScenarioRunner GameWinEvent(string firstOpponentName, uint expectedVictoryPoints)
        {
            var playerId = this.playersByName[firstOpponentName].Id;
            var expectedGameWonEvent = new GameWinEvent(playerId, expectedVictoryPoints);
            this.relevantEvents.Enqueue(expectedGameWonEvent);
            return this;
        }

        public ResourceCollectedEventGroup StartResourcesCollectedEvent(string playerName)
        {
            var player = this.playersByName[playerName];
            var eventGroup = new ResourceCollectedEventGroup(player.Id, this);
            return eventGroup;
        }

        public LocalGameControllerScenarioRunner WithComputerPlayer(string name)
        {
            this.CreatePlayer(name, true);
            return this;
        }

        public LocalGameControllerScenarioRunner WithMainPlayer(string name)
        {
            this.CreatePlayer(name, false);
            return this;
        }

        public LocalGameControllerScenarioRunner WithPlayerSetup(string playerName, uint firstSettlementLocation, uint firstRoadEndLocation, uint secondSettlementLocation, uint secondRoadEndLocation)
        {
            if (playerName == this.players[0].Name)
            {
                this.playerInstructions.Enqueue(new PlaceInfrastructureInstruction(this.players[0].Id, firstSettlementLocation, firstRoadEndLocation));
                this.playerInstructions.Enqueue(new PlaceInfrastructureInstruction(this.players[0].Id, secondSettlementLocation, secondRoadEndLocation));
            }
            else
            {
                var computerPlayer = this.computerPlayersByName[playerName];
                computerPlayer.AddSetupInstructions(
                    new PlaceInfrastructureInstruction(computerPlayer.Id, firstSettlementLocation, firstRoadEndLocation),
                    new PlaceInfrastructureInstruction(computerPlayer.Id, secondSettlementLocation, secondRoadEndLocation));
            }

            return this;
        }

        public LocalGameControllerScenarioRunner WithTurnOrder(string firstPlayerName, string secondPlayerName, string thirdPlayerName, string fourthPlayerName)
        {
            var rolls = new uint[4];
            for (var index = 0; index < this.players.Count; index++)
            {
                var player = this.players[index];
                if (firstPlayerName == player.Name)
                    rolls[index] = 12;
                else if (secondPlayerName == player.Name)
                    rolls[index] = 10;
                else if (thirdPlayerName == player.Name)
                    rolls[index] = 8;
                else
                    rolls[index] = 6;
            }

            foreach (var roll in rolls)
                this.mockNumberGenerator.AddTwoDiceRoll(roll / 2, roll / 2);

            return this;
        }

        public PlayerTurn DuringPlayerTurn(string playerName, uint dice1, uint dice2)
        {
            this.mockNumberGenerator.AddTwoDiceRoll(dice1, dice2);

            PlayerTurn playerTurn = null;

            if (playerName == this.players[0].Name)
            {
                playerTurn = new PlayerTurn(this, this.players[0]);
            }
            else
            {
                playerTurn = new ComputerPlayerTurn(this, this.computerPlayersByName[playerName]);
            }

            this.playerTurns.Add(playerTurn);
            return playerTurn;
        }

        public LocalGameControllerScenarioRunner BuyDevelopmentCardEvent(string playerName, DevelopmentCardTypes developmentCardType)
        {
            var player = this.playersByName[playerName];
            var expectedBuyDevelopmentCardEvent = new MockBuyDevelopmentCardEvent(player.Id, developmentCardType);
            this.relevantEvents.Enqueue(expectedBuyDevelopmentCardEvent);

            return this;
        }

        public LocalGameControllerScenarioRunner WithStartingResourcesForPlayer(string playerName, ResourceClutch playerResources)
        {
            var player = this.playersByName[playerName];
            player.AddResources(playerResources);
            return this;
        }

        private IPlayer CreatePlayer(string name, bool isComputerPlayer)
        {
            IPlayer player = isComputerPlayer
                ? new MockComputerPlayer(name, this.mockNumberGenerator) as IPlayer
                : new MockPlayer(name) as IPlayer;

            this.players.Add(player);
            this.playersByName.Add(name, player);
            this.mockPlayerPool.AddPlayer(player);
            if (isComputerPlayer)
                this.computerPlayersByName.Add(name, (MockComputerPlayer)player);

            return player;
        }

        private void DiceRollEventHandler(Guid playerId, uint dice1, uint dice2)
        {
            this.actualEvents.Add(new DiceRollEvent(playerId, dice1, dice2));
        }

        private void GameEventsHandler(List<GameEvent> gameEvents)
        {
            this.actualEvents.AddRange(gameEvents);
        }

        internal void AddDevelopmentCardToBuy(DevelopmentCardTypes developmentCardType)
        {
            DevelopmentCard developmentCard = null;
            switch(developmentCardType)
            {
                case DevelopmentCardTypes.Knight: developmentCard = new KnightDevelopmentCard(); break;
                default: throw new Exception($"Development card type {developmentCardType} not recognised");
            }

            this.mockDevelopmentCardHolder.AddDevelopmentCard(developmentCard);
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;
using NUnit.Framework;

namespace SoC.Library.ScenarioTests
{
    internal class LocalGameControllerScenarioRunner
    {
        internal enum EventTypes
        {
            DiceRollEvent,
            ResourcesCollectedEvent
        }

        #region Fields
        private static LocalGameControllerScenarioRunner localGameControllerScenarioBuilder;
        private readonly MockPlayerPool mockPlayerPool = new MockPlayerPool();
        private readonly Queue<Instruction> playerInstructions = new Queue<Instruction>();
        private readonly List<PlayerTurnSetupAction> FirstRoundSetupActions = new List<PlayerTurnSetupAction>(4);
        private readonly List<PlayerTurnSetupAction> SecondRoundSetupActions = new List<PlayerTurnSetupAction>(4);
        private readonly List<PlayerTurn> playerTurns = new List<PlayerTurn>();
        private readonly MockNumberGenerator mockNumberGenerator = new MockNumberGenerator();
        private readonly Dictionary<string, IPlayer> playersByName = new Dictionary<string, IPlayer>();
        private readonly Dictionary<string, MockComputerPlayer> computerPlayersByName = new Dictionary<string, MockComputerPlayer>();
        private readonly List<IPlayer> players = new List<IPlayer>(4);
        private LocalGameController localGameController = null;
        private Queue<GameEvent> expectedEvents = null;
        private List<GameEvent> actualEvents = null;
        private Action<DiceRollEvent> diceRollEvent;
        private Action<ResourcesCollectedEvent> ResourcesCollectedEventHandler;
        private TurnToken currentToken;
        private int lastPlayerTurnCount;
        private Guid lastPlayerId;
        private bool refuseEvents;
        #endregion

        #region Construction
        private LocalGameControllerScenarioRunner() {}
        #endregion

        #region Methods
        public static LocalGameControllerScenarioRunner LocalGameController()
        {
            return localGameControllerScenarioBuilder = new LocalGameControllerScenarioRunner();
        }

        public LocalGameControllerScenarioRunner Build()
        {
            this.localGameController = new LocalGameController(this.mockNumberGenerator, this.mockPlayerPool);
            this.localGameController.DiceRollEvent = this.DiceRollEventHandler;
            this.localGameController.GameEvents = this.GameEventsHandler;
            this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { this.currentToken = t; };
            return this;
        }

        public LocalGameControllerScenarioRunner BuildRoadEvent(string thirdOpponentName, uint roadSegmentStart, uint roadSegmentEnd)
        {
            var playerId = this.playersByName[thirdOpponentName].Id;
            this.expectedEvents.Enqueue(new RoadSegmentBuiltEvent(playerId, roadSegmentStart, roadSegmentEnd));
            return this;
        }

        public LocalGameControllerScenarioRunner BuildSettlementEvent(string thirdOpponentName, uint settlementLocation)
        {
            var playerId = this.playersByName[thirdOpponentName].Id;
            this.expectedEvents.Enqueue(new SettlementBuiltEvent(playerId, settlementLocation));
            return this;
        }

        public LocalGameControllerScenarioRunner DiceRollEvent(string playerName, uint dice1, uint dice2)
        {
            var player = this.playersByName[playerName];

            var expectedDiceRollEvent = new DiceRollEvent(player.Id, dice1, dice2);
            this.expectedEvents.Enqueue(expectedDiceRollEvent);
            this.RegisterEventHandler(EventTypes.DiceRollEvent);

            return this;
        }

        public LocalGameControllerScenarioRunner Events(EventTypes eventType, uint count)
        {
            while (count-- > 0)
            {
                this.expectedEvents.Enqueue(new PlaceholderEvent(eventType));
                this.RegisterEventHandler(eventType);
            }

            return this;
        }

        public LocalGameControllerScenarioRunner Event(EventTypes eventType)
        {
            return this.Events(eventType, 1);
        }

        public LocalGameControllerScenarioRunner ExpectingEvents()
        {
            this.expectedEvents = new Queue<GameEvent>();
            this.actualEvents = new List<GameEvent>();
            return this;
        }

        public LocalGameControllerScenarioRunner ResourcesCollectedEvent(string playerName, uint location, ResourceClutch resourceClutch)
        {
            var player = this.playersByName[playerName];

            return this.ResourcesCollectedEvent(player.Id, new[] { new ResourceCollection(location, resourceClutch) });
        }

        public LocalGameControllerScenarioRunner ResourcesCollectedEvent(Guid playerId, ResourceCollection[] resourceCollection)
        {
            var expectedDiceRollEvent = new ResourcesCollectedEvent(playerId, resourceCollection);
            this.expectedEvents.Enqueue(expectedDiceRollEvent);
            this.RegisterEventHandler(EventTypes.ResourcesCollectedEvent);
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

            this.lastPlayerId = this.playerTurns[this.playerTurns.Count - 1].PlayerId;
            this.lastPlayerTurnCount = this.playerTurns.Count(p => p.PlayerId == this.lastPlayerId);

            this.localGameController.StartGamePlay();

            var turns = new Queue<PlayerTurn>(this.playerTurns);

            do
            {
                var turn = turns.Dequeue();

                if (turn is ComputerPlayerTurn computerPlayerTurn)
                {
                    computerPlayerTurn.ResolveActions();
                }
                else if (turn is PlayerTurn playerTurn)
                {
                    // Do the player turns and then the computer turns for this round
                    var computerPlayerTurns = 3;
                    while (computerPlayerTurns-- > 0)
                    {
                        var cpt = (ComputerPlayerTurn)turns.Dequeue();
                        cpt.ResolveActions();

                    }

                    this.localGameController.EndTurn(this.currentToken);
                }
            } while (turns.Count > 0);

            var actualEventIndex = 0;
            while (this.expectedEvents.Count > 0)
            {
                var expectedEvent = this.expectedEvents.Dequeue();
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

            return this.localGameController;
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
            if (!this.refuseEvents && this.lastPlayerId == playerId && --this.lastPlayerTurnCount < 0)
                this.refuseEvents = true;

            this.diceRollEvent?.Invoke(new DiceRollEvent(playerId, dice1, dice2));
        }

        private void GameEventsHandler(List<GameEvent> gameEvents)
        {
            if (this.refuseEvents)
                return;

            foreach (var gameEvent in gameEvents)
            {
                if (gameEvent is ResourcesCollectedEvent resourceCollectedEvent)
                    this.ResourcesCollectedEventHandler?.Invoke(resourceCollectedEvent);
            }
        }

        private void RegisterEventHandler(EventTypes eventType)
        {
            switch (eventType)
            {
                case EventTypes.DiceRollEvent:
                {
                    this.diceRollEvent = (DiceRollEvent d) => { this.TryAddEvent(d); };
                    break;
                }
                case EventTypes.ResourcesCollectedEvent:
                {
                    this.ResourcesCollectedEventHandler = (ResourcesCollectedEvent r) => { this.TryAddEvent(r); };
                    break;
                }
            }
        }

        private void TryAddEvent(GameEvent gameEvent)
        {
            if (!this.refuseEvents)
                this.actualEvents.Add(gameEvent);
        }
        #endregion
    }

    internal class ResourceCollectedEventGroup
    {
        private List<ResourceCollection> resourceCollectionList = new List<ResourceCollection>();
        private readonly LocalGameControllerScenarioRunner runner;
        internal Guid PlayerId { get; }

        internal ResourceCollectedEventGroup(Guid playerId, LocalGameControllerScenarioRunner runner)
        {
            this.PlayerId = playerId;
            this.runner = runner;
        }

        internal ResourceCollectedEventGroup AddResourceCollection(uint location, ResourceClutch resourceClutch)
        {
            this.resourceCollectionList.Add(new ResourceCollection(location, resourceClutch));
            return this;
        }

        internal LocalGameControllerScenarioRunner FinishResourcesCollectedEvent()
        {
            return this.runner.ResourcesCollectedEvent(this.PlayerId, this.resourceCollectionList.ToArray());
        }
    }
}

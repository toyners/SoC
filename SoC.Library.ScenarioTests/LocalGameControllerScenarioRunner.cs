using System;
using System.Collections.Generic;
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
        private Dictionary<EventTypes, Delegate> eventHandlers;
        private readonly Queue<PlayerTurn> playerTurns = new Queue<PlayerTurn>();
        private readonly MockNumberGenerator mockNumberGenerator = new MockNumberGenerator();
        private readonly Dictionary<string, IPlayer> playersByName = new Dictionary<string, IPlayer>();
        private readonly Dictionary<string, MockComputerPlayer> computerPlayersByName = new Dictionary<string, MockComputerPlayer>();
        private readonly List<IPlayer> players = new List<IPlayer>(4);
        private LocalGameController localGameController = null;
        private Queue<GameEvent> expectedEvents = null;
        private List<GameEvent> actualEvents = null;
        private Action<ResourcesCollectedEvent> ResourcesCollectedEventHandler;
        private TurnToken currentToken;
        #endregion

        #region Construction
        private LocalGameControllerScenarioRunner(Dictionary<EventTypes, Delegate> eventHandlers)
        {
            this.eventHandlers = eventHandlers;
        }
        #endregion

        internal static LocalGameControllerScenarioRunner LocalGameController(Dictionary<EventTypes, Delegate> eventHandlers = null)
        {
            return localGameControllerScenarioBuilder = new LocalGameControllerScenarioRunner(eventHandlers);
        }

        internal LocalGameControllerScenarioRunner Build()
        {
            this.localGameController = new LocalGameController(this.mockNumberGenerator, this.mockPlayerPool);
            this.localGameController.GameEvents = this.GameEventsHandler;
            this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { this.currentToken = t; };
            return this;
        }

        internal LocalGameControllerScenarioRunner DiceRollEvent(string playerName, uint dice1, uint dice2)
        {
            var player = this.playersByName[playerName];

            var expectedDiceRollEvent = new DiceRollEvent(player.Id, dice1, dice2);
            this.expectedEvents.Enqueue(expectedDiceRollEvent);
            this.RegisterEventHandler(EventTypes.DiceRollEvent);

            return this;
        }

        internal LocalGameControllerScenarioRunner Events(EventTypes eventType, uint count)
        {
            while (count-- > 0)
            {
                this.expectedEvents.Enqueue(new PlaceholderEvent(eventType));
                this.RegisterEventHandler(eventType);
            }

            return this;
        }

        internal LocalGameControllerScenarioRunner Event(EventTypes eventType)
        {
            return this.Events(eventType, 1);
        }

        internal LocalGameControllerScenarioRunner ResourcesCollectedEvent(string playerName, uint location, ResourceClutch resourceClutch)
        {
            var player = this.playersByName[playerName];

            this.ResourcesCollectedEvent(player.Id, new[] { new ResourceCollection(location, resourceClutch) });
            return this;
        }

        internal void ResourcesCollectedEvent(Guid playerId, ResourceCollection[] resourceCollection)
        {
            var expectedDiceRollEvent = new ResourcesCollectedEvent(playerId, resourceCollection);
            this.expectedEvents.Enqueue(expectedDiceRollEvent);
            this.RegisterEventHandler(EventTypes.ResourcesCollectedEvent);
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
            this.playerTurns.Dequeue();

            while (this.playerTurns.Count > 0)
            {
                this.playerTurns.Dequeue();
                this.localGameController.EndTurn(this.currentToken);
            }

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

        public LocalGameControllerScenarioRunner ExpectingEvents()
        {
            this.expectedEvents = new Queue<GameEvent>();
            this.actualEvents = new List<GameEvent>();
            return this;
        }

        private void RegisterEventHandler(EventTypes eventType)
        {
            switch (eventType)
            {
                case EventTypes.DiceRollEvent:
                {
                    this.localGameController.DiceRollEvent =
                    (uint dice1, uint dice2) =>
                    {
                        this.actualEvents.Add(new DiceRollEvent(Guid.Empty, dice1, dice2));
                        var customHandler = this.eventHandlers?[EventTypes.DiceRollEvent];
                        if (customHandler != null)
                            ((Action<uint, uint>)customHandler).Invoke(dice1, dice2);
                    };
                    break;
                }
                case EventTypes.ResourcesCollectedEvent:
                {
                    this.ResourcesCollectedEventHandler = 
                    (ResourcesCollectedEvent r) => 
                    {
                        this.actualEvents.Add(r);
                        var customHandler = this.eventHandlers?[EventTypes.ResourcesCollectedEvent];
                        if (customHandler != null)
                            ((Action<ResourcesCollectedEvent>)customHandler).Invoke(r);
                    };
                    break;
                }
            }
        }

        private void GameEventsHandler(List<GameEvent> gameEvents)
        {
            foreach (var gameEvent in gameEvents)
            {
                if (gameEvent is ResourcesCollectedEvent resourceCollectedEvent)
                    this.ResourcesCollectedEventHandler?.Invoke(resourceCollectedEvent);
            }
        }

        public LocalGameControllerScenarioRunner WithMainPlayer(string name)
        {
            this.CreatePlayer(name, false);
            return this;
        }

        public LocalGameControllerScenarioRunner WithComputerPlayer(string name)
        {
            this.CreatePlayer(name, true);
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
                this.playerTurns.Enqueue(playerTurn);
            }
            else
            {
                playerTurn = new ComputerPlayerTurn(this, this.computerPlayersByName[playerName]);
            }

            return playerTurn;
        }
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
            this.runner.ResourcesCollectedEvent(this.PlayerId, this.resourceCollectionList.ToArray());
            return this.runner;
        }
    }
}

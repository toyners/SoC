using System;
using System.Collections.Generic;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class LocalGameControllerScenarioRunner
    {
        internal enum EventTypes
        {
            DiceRollEvent
        }

        private readonly MockPlayerPool mockPlayerPool = new MockPlayerPool();
        private readonly Queue<Instruction> playerInstructions = new Queue<Instruction>();
        private readonly List<PlayerTurnSetupAction> FirstRoundSetupActions = new List<PlayerTurnSetupAction>(4);
        private readonly List<PlayerTurnSetupAction> SecondRoundSetupActions = new List<PlayerTurnSetupAction>(4);
        private Dictionary<EventTypes, Delegate> eventHandlers;
        private readonly List<PlayerTurn> playerTurns = new List<PlayerTurn>();
        private readonly MockNumberGenerator mockNumberGenerator = new MockNumberGenerator();
        private readonly Dictionary<string, IPlayer> playersByName = new Dictionary<string, IPlayer>();
        private readonly Dictionary<string, MockComputerPlayer> computerPlayersByName = new Dictionary<string, MockComputerPlayer>();
        private readonly List<IPlayer> players = new List<IPlayer>(4);
        private LocalGameController localGameController = null;

        private static LocalGameControllerScenarioRunner localGameControllerScenarioBuilder;

        public static LocalGameControllerScenarioRunner LocalGameController(Dictionary<EventTypes, Delegate> eventHandlers = null)
        {
            return localGameControllerScenarioBuilder = new LocalGameControllerScenarioRunner(eventHandlers);
        }

        private LocalGameControllerScenarioRunner(Dictionary<EventTypes, Delegate> eventHandlers)
        {
            this.eventHandlers = eventHandlers;
        }

        public LocalGameControllerScenarioRunner Build()
        {
            this.localGameController = new LocalGameController(this.mockNumberGenerator, this.mockPlayerPool);
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

            var actualEventIndex = 0;
            foreach (var expectedEvent in this.expectedEvents)
            {
                var foundEvent = false;
                for (; actualEventIndex < this.actualEvents.Count; actualEventIndex++)
                {
                    if (this.actualEvents[actualEventIndex].Equals(expectedEvent))
                    {
                        foundEvent = true;
                        break;
                    }
                }

                if (!foundEvent)
                    throw new NotImplementedException();
            }

            return this.localGameController;
        }

        List<ResourceCollectedEventGroup> eventGroups;
        public ResourceCollectedEventGroup StartResourcesCollectedEvent(string playerName)
        {
            var player = this.playersByName[playerName];
            var eventGroup = new ResourceCollectedEventGroup(player.Id, this);
            this.eventGroups.Add(eventGroup);
            return eventGroup;
        }

        List<GameEvent> expectedEvents = null;
        public LocalGameControllerScenarioRunner ExpectingEvents()
        {
            this.expectedEvents = new List<GameEvent>();
            this.actualEvents = new List<GameEvent>();
            return this;
        }

        public LocalGameControllerScenarioRunner DiceRollEvent(string playerName, uint dice1, uint dice2)
        {
            var player = this.playersByName[playerName];

            var expectedDiceRollEvent = new DiceRollEvent(player.Id, dice1, dice2);
            this.expectedEvents.Add(expectedDiceRollEvent);
            this.RegisterEventHandler(EventTypes.DiceRollEvent);

            return this;
        }

        List<GameEvent> actualEvents = null;
        private void RegisterEventHandler(EventTypes eventType)
        {
            switch (eventType)
            {
                case EventTypes.DiceRollEvent: this.localGameController.DiceRollEvent =
                        (uint dice1, uint dice2) =>
                        {
                            this.actualEvents.Add(new DiceRollEvent(Guid.Empty, dice1, dice2));
                            var customHandler = this.eventHandlers?[EventTypes.DiceRollEvent];
                            if (customHandler != null)
                                ((Action<uint, uint>)customHandler).Invoke(dice1, dice2);
                        };
                        break;
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
                var player = this.players[0];
                playerTurn = new PlayerTurn(this, player);
            }
            else
            {
                var computerPlayer = this.computerPlayersByName[playerName];
                playerTurn = new PlayerTurn(this, computerPlayer);
            }

            this.playerTurns.Add(playerTurn);

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
            return this.runner;
        }
    }
}

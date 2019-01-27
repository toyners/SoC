using System;
using System.Collections.Generic;
using System.Linq;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
using Jabberwocky.SoC.Library.Enums;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;
using Jabberwocky.SoC.Library.ScenarioTests.Builders;
using NUnit.Framework;
using SoC.Library.ScenarioTests.Builders;
using SoC.Library.ScenarioTests.ScenarioActions;
using SoC.Library.ScenarioTests.ScenarioEvents;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class BasePlayerTurn
    {
        #region Fields
        public readonly IPlayer player;
        protected readonly LocalGameControllerScenarioRunner runner;
        protected readonly List<GameEvent> actualEvents = new List<GameEvent>();
        private List<Tuple<string, ComputerPlayerAction>> playerActions;
        private readonly List<GameEvent> expectedEvents = new List<GameEvent>();
        private readonly int roundNumber;
        private readonly int turnNumber;
        private Dictionary<string, IPlayer> playersByName;
        #endregion

        #region Construction
        public BasePlayerTurn(IPlayer player, LocalGameControllerScenarioRunner runner, int roundNumber, int turnNumber)
        {
            this.player = player;
            this.runner = runner;
            this.roundNumber = roundNumber;
            this.turnNumber = turnNumber;
        }

        public BasePlayerTurn(string playerName, Dictionary<string, IPlayer> playersByName, LocalGameControllerScenarioRunner runner, int roundNumber, int turnNumber)
        {
            this.player = playersByName[playerName];
            this.runner = runner;
            this.playersByName = playersByName;
            this.roundNumber = roundNumber;
            this.turnNumber = turnNumber;
        }
        #endregion

        #region Properties
        public IDictionary<Guid, ComputerPlayerAction> ActionsByPlayerId { protected get; set; }
        public IList<GameEvent> ExpectedEvents { private get; set; }
        public IDictionary<Guid, GameEvent> GameEventsByPlayerId { private get; set; }
        public IDictionary<string, ResourceClutch> PlayerResourcesToDropByName { protected get; set; }
        public Guid PlayerId { get { return this.player.Id; } }
        public IList<RunnerAction> RunnerActions { get; set; }
        public bool TreatErrorsAsEvents
        {
            get
            {
                return this.ExpectedEvents != null &&
                    this.ExpectedEvents.FirstOrDefault(e => e.GetType() == typeof(ScenarioErrorMessageEvent)) != null;
            }
        }
        public IList<Type> UnwantedEventTypes { private get; set; }
        public LocalGameController LocalGameController { get; set; }
        #endregion

        #region Methods
        public PlayerActionBuilder Actions()
        {
            return new PlayerActionBuilder(this);
        }

        public LocalGameControllerScenarioRunner EndTurn()
        {
            return this.runner;
        }

        public ExpectedEventsBuilder Events()
        {
            return new ExpectedEventsBuilder(this, this.runner.playersByName);
        }

        List<PlayerState> playerStates = new List<PlayerState>();

        private int actualEventIndex;
        private int expectedEventIndex;
        public virtual void AddEvent(GameEvent actualEvent)
        {
            this.actualEvents.Add(actualEvent);

            bool eventProcessed = false;
            while (this.instructions.Count > 0)
            {
                var instruction = this.instructions.Dequeue();
                if (instruction is GameEvent expectedGameEvent)
                {
                    this.expectedEvents.Add(expectedGameEvent);
                    if (actualEvent.Equals(this.expectedEvents[this.expectedEventIndex]) && !eventProcessed)
                    {
                        this.expectedEventIndex++;
                        this.actualEventIndex++;
                        eventProcessed = true;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (instruction is ComputerPlayerAction action)
                {
                    if (action is ScenarioDropResourcesAction scenarioResourcesToDropAction)
                    {
                        var player = this.playersByName[scenarioResourcesToDropAction.PlayerName];
                        var dropResourcesAction = scenarioResourcesToDropAction.CreateDropResourcesAction();
                        
                        if (player is ScenarioComputerPlayer computerPlayer)
                        {
                            computerPlayer.AddDropResourcesAction(dropResourcesAction);
                        }
                        else if (player is ScenarioPlayer humanPlayer)
                        {
                            this.LocalGameController.DropResources(dropResourcesAction.Resources);
                        }
                    }
                }
                else if (instruction is PlayerState playerState)
                {
                    var player = playerState.Player;
                    this.playerStates.Add(playerState);

                    if (player is ScenarioComputerPlayer computerPlayer)
                    {
                        var verifySnapshotAction = new ScenarioVerifySnapshotAction(playerState);
                        computerPlayer.AddAction(verifySnapshotAction);
                    }
                    else if (player is ScenarioPlayer humanPlayer)
                    {
                        playerState.Verify();
                    }
                }
            }
        }

        public BasePlayerTurn ResourceCollectedEvent(string playerName, params Tuple<uint, ResourceClutch>[] resourceCollectionPairs)
        {
            var playerId = this.playersByName[playerName].Id;

            ResourceCollection[] rc = new ResourceCollection[resourceCollectionPairs.Length];
            var index = 0;
            foreach (var pair in resourceCollectionPairs)
                rc[index++] = new ResourceCollection(pair.Item1, pair.Item2);
            this.instructions.Enqueue(new ResourcesCollectedEvent(playerId, rc));

            return this;
        }

        public virtual void ResolveActions(TurnToken turnToken, LocalGameController localGameController)
        {
        /*    if (this.PlayerActions == null || this.PlayerActions.Count == 0)
                return;

            foreach (var action in this.PlayerActions)
            {
                if (action is ScenarioPlaceRobberAction placeRobberAction)
                {
                    if (!placeRobberAction.ResourcesToDrop.Equals(ResourceClutch.Zero))
                        localGameController.DropResources(placeRobberAction.ResourcesToDrop);

                    localGameController.SetRobberHex(placeRobberAction.NewRobberHex);
                }
                else if (action is ScenarioPlayKnightCardAction scenarioPlayKnightCardAction)
                {
                    var selectedPlayer = this.runner.GetPlayerFromName(scenarioPlayKnightCardAction.SelectedPlayerName);
                    this.SetupResourceSelectionOnPlayer(selectedPlayer, scenarioPlayKnightCardAction.ExpectedSingleResource);
                    var knightCard = (KnightDevelopmentCard)this.player.HeldCards.Where(c => c.Type == DevelopmentCardTypes.Knight).First();
                    localGameController.UseKnightCard(turnToken, knightCard, scenarioPlayKnightCardAction.NewRobberHex, selectedPlayer.Id);
                }
                else if (action is ScenarioSelectResourceFromPlayerAction scenarioSelectResourceFromPlayerAction)
                {
                    var selectedPlayer = this.runner.GetPlayerFromName(scenarioSelectResourceFromPlayerAction.SelectedPlayerName);
                    this.SetupResourceSelectionOnPlayer(selectedPlayer, scenarioSelectResourceFromPlayerAction.ExpectedSingleResource);
                    localGameController.ChooseResourceFromOpponent(selectedPlayer.Id);
                }
                else if (action is PlayKnightCardAction playKnightCardAction)
                {
                    var knightCard = (KnightDevelopmentCard)this.player.HeldCards.Where(c => c.Type == DevelopmentCardTypes.Knight).First();
                    localGameController.UseKnightCard(turnToken, knightCard, playKnightCardAction.NewRobberHex, playKnightCardAction.PlayerId);
                }
                else if (action is ScenarioResourcesToDropAction scenarioResourcesToDropAction)
                {
                    localGameController.DropResources(scenarioResourcesToDropAction.Resources);
                }
                else if (action is ComputerPlayerAction)
                {
                    switch (action.ActionType)
                    {
                        case ComputerPlayerActionTypes.BuyDevelopmentCard: localGameController.BuyDevelopmentCard(turnToken); break;
                        default: throw new Exception($"Action type '{action.ActionType}' not handled");
                    }
                }
                else
                {
                    throw new Exception($"Action of type '{action.GetType()}' not handled");
                }
            }*/
        }

        public void ResolveResponses(LocalGameController localGameController)
        {
            /*if (this.PlayerResourcesToDropByName == null || this.PlayerResourcesToDropByName.Count == 0)
                return;

            foreach (var kv in this.PlayerResourcesToDropByName)
            {
                var player = this.runner.playersByName[kv.Key];

                if (player is ScenarioPlayer)
                {
                    this.PlayerActions = new List<ComputerPlayerAction>();
                    this.PlayerActions.Add(new ScenarioResourcesToDropAction(kv.Value));
                }
                else
                {
                    ((ScenarioComputerPlayer)player).AddResourcesToDrop(kv.Value);
                }
            }*/
        }

        protected virtual void ResolveResponse(ComputerPlayerAction response, LocalGameController localGameController)
        {
            if (response is ScenarioDropResourcesAction scenarioResourcesToDropAction)
            {
                localGameController.DropResources(scenarioResourcesToDropAction.Resources);
            }
            else
            {
                throw new Exception($"Response of type '{response.GetType()}' not handled");
            }
        }

        public PlayerResponseBuilder Responses()
        {
            return new PlayerResponseBuilder(this, this.runner.playersByName);
        }

        public PlayerStateBuilder OldState(string playerName)
        {
            throw new NotImplementedException();
        }

        public PlayerState State(string playerName)
        {
            var player = this.playersByName[playerName];
            var playerState = new PlayerState(this, player);
            this.instructions.Enqueue(playerState);

            return playerState;
        }

        public void AddEvents(List<GameEvent> gameEvents)
        {
            foreach (var gameEvent in gameEvents)
                this.AddEvent(gameEvent);
        }

        internal void VerifyEvents()
        {
            if (this.UnwantedEventTypes != null && this.UnwantedEventTypes.Count > 0)
            {
                var unwantedEvents = this.actualEvents.Where(e => this.UnwantedEventTypes.Contains(e.GetType())).ToList();
                if (unwantedEvents.Count > 0)
                    Assert.Fail($"{unwantedEvents.Count} events of unwanted type {unwantedEvents[0].GetType()} found.\r\nFirst event:\r\n{this.GetEventDetails(unwantedEvents[0])}");
            }

            if (this.ExpectedEvents == null || this.ExpectedEvents.Count == 0)
                return;

            var actualEventIndex = 0;
            foreach (var expectedEvent in this.ExpectedEvents)
            {
                var foundEvent = false;
                while (actualEventIndex < this.actualEvents.Count)
                {
                    var actualEvent = this.actualEvents[actualEventIndex++];
                    if (expectedEvent.Equals(actualEvent))
                    {
                        foundEvent = true;
                        break;
                    }
                }

                if (!foundEvent)
                    Assert.Fail($"Did not find {expectedEvent.GetType()} event for '{this.player.Name}'.\r\n{this.GetEventDetails(expectedEvent)}");
            }
        }

        internal void VerifyState(Dictionary<string, IPlayer> playersByName)
        {
            /*foreach (var kv in this.playerSnapshotsByName)
                kv.Value.Verify(playersByName[kv.Key]);*/
        }

        protected void AddDevelopmentCard(Guid playerId, DevelopmentCardTypes developmentCardType)
        {
            this.runner.AddDevelopmentCardToBuy(developmentCardType);
        }

        protected void SetupResourceSelectionOnPlayer(IPlayer selectedPlayer, ResourceTypes expectedSingleResource)
        {
            var randomNumber = int.MinValue;
            switch (expectedSingleResource)
            {
                case ResourceTypes.Grain:
                {
                    randomNumber = selectedPlayer.Resources.BrickCount;
                    break;
                }
                case ResourceTypes.Ore:
                {
                    randomNumber = selectedPlayer.Resources.BrickCount +
                    selectedPlayer.Resources.GrainCount +
                    selectedPlayer.Resources.LumberCount;
                    break;
                }
                case ResourceTypes.Wool:
                {
                    randomNumber = selectedPlayer.Resources.Count - 1;
                    break;
                }
                default: throw new Exception($"Resource type '{expectedSingleResource}' not handled");
            }

            this.runner.NumberGenerator.AddRandomNumber(randomNumber);
        }

        private string GetEventDetails(GameEvent gameEvent)
        {
            var message = "";

            if (gameEvent is DiceRollEvent diceRollEvent)
            {
                message += $"Dice roll 1 is {diceRollEvent.Dice1}, Dice roll 2 is {diceRollEvent.Dice2}";
            }
            else if (gameEvent is ScenarioErrorMessageEvent scenarioErrorMessageEvent)
            {
                message += $"With error message '{scenarioErrorMessageEvent.ExpectedErrorMessage}'";
            }
            else if (gameEvent is ResourcesCollectedEvent resourcesCollectedEvent)
            {
                message += $"Resources collected entries count is {resourcesCollectedEvent.ResourceCollection.Length}";
                foreach (var entry in resourcesCollectedEvent.ResourceCollection)
                    message += $"\r\nLocation {entry.Location}, Resources {entry.Resources}";
            }
            else if (gameEvent is ResourceTransactionEvent resourceTransactionEvent)
            {
                message += $"Resource transaction count is {resourceTransactionEvent.ResourceTransactions.Count}";
                for (var index = 0; index < resourceTransactionEvent.ResourceTransactions.Count; index++)
                {
                    var resourceTransaction = resourceTransactionEvent.ResourceTransactions[index];
                    var receivingPlayer = this.runner.GetPlayer(resourceTransaction.ReceivingPlayerId);
                    var givingPlayer = this.runner.GetPlayer(resourceTransaction.GivingPlayerId);
                    message += $"\r\nReceiving player '{receivingPlayer.Name}', Giving player '{givingPlayer.Name}', Resources {resourceTransaction.Resources}";
                }
            }
            else if (gameEvent is RoadSegmentBuiltEvent roadSegmentBuildEvent)
            {
                message += $"From {roadSegmentBuildEvent.StartLocation} to {roadSegmentBuildEvent.EndLocation}";
            }
            else if (gameEvent is ScenarioRobberEvent scenarioRobberEvent)
            {
                message += $"Expected resource-to-drop count is {scenarioRobberEvent.ExpectedResourcesToDropCount}";
            }
            else if (gameEvent is ScenarioRobbingChoicesEvent scenarioRobbingChoicesEvent)
            {
                if (scenarioRobbingChoicesEvent.RobbingChoices != null)
                {
                    message += "With choices:\r\n";
                    foreach (var robbingChoicePair in scenarioRobbingChoicesEvent.RobbingChoices)
                        message += $"'{this.runner.GetPlayer(robbingChoicePair.Key).Name}' with {robbingChoicePair.Value} resources\r\n";
                }
                else
                {
                    message += "With no choices";
                }
            }

            return message;
        }

        public virtual void Process(TurnToken currentToken, LocalGameController localGameController)
        {
            if (this.instructions == null)
            {
                return;
            }

            this.LocalGameController = localGameController;

            while (this.instructions.Count > 0)
            {
                var instruction = this.instructions.Peek();
                if (instruction is GameEvent expectedEvent)
                {
                    this.expectedEvents.Add(expectedEvent);
                }
                else if (instruction is ComputerPlayerAction action)
                {
                    if (action is ScenarioDropResourcesAction scenarioResourcesToDropAction)
                    {
                        var player = this.playersByName[scenarioResourcesToDropAction.PlayerName];
                        var dropResourcesAction = scenarioResourcesToDropAction.CreateDropResourcesAction();

                        if (player is ScenarioComputerPlayer computerPlayer)
                        {
                            computerPlayer.AddDropResourcesAction(dropResourcesAction);
                        }
                        else if (player is ScenarioPlayer humanPlayer)
                        {
                            this.LocalGameController.DropResources(dropResourcesAction.Resources);
                        }
                    }
                }
                else if (instruction is PlayerState playerState)
                {
                    var player = playerState.Player;
                    this.playerStates.Add(playerState);

                    if (player is ScenarioComputerPlayer computerPlayer)
                    {
                        var verifySnapshotAction = new ScenarioVerifySnapshotAction(playerState);
                        computerPlayer.AddAction(verifySnapshotAction);
                    }
                    else if (player is ScenarioPlayer humanPlayer)
                    {
                        playerState.Verify();
                    }
                }
            }

            // If there are expected events yet to check then do it now
            if (this.expectedEventIndex < this.expectedEvents.Count)
            {
                while (this.actualEventIndex < this.actualEvents.Count)
                {
                    if (this.expectedEvents[this.expectedEventIndex].Equals(this.actualEvents[this.actualEventIndex]))
                    {
                        this.expectedEventIndex++;
                    }

                    this.actualEventIndex++;
                }

                // At least one expected event was not matched with an actual event.
                if (this.expectedEventIndex < this.expectedEvents.Count)
                {
                    var expectedEvent = this.expectedEvents[this.expectedEventIndex];
                    Assert.Fail($"Did not find {expectedEvent.GetType()} event for '{this.player.Name}'.\r\n{this.GetEventDetails(expectedEvent)}");
                }
            }
        }

        protected Queue<object> instructions = new Queue<object>();
        public BasePlayerTurn DiceRollEvent(uint dice1, uint dice2)
        {
            this.instructions.Enqueue(new DiceRollEvent(this.PlayerId, dice1, dice2));
            return this;
        }

        internal BasePlayerTurn DropResources(string playerName, ResourceClutch resourcesToDrop)
        {
            //var player = this.runner.GetPlayerFromName(playerName);
            this.instructions.Enqueue(new ScenarioDropResourcesAction(playerName, resourcesToDrop));
            
            //((ScenarioComputerPlayer)player).AddResourcesToDrop(resourcesToDrop);

            return this;
        }

        public BasePlayerTurn ResourcesLostEvent(params Tuple<string, ResourceClutch>[] resourcesLostPairs)
        {
            var dict = new Dictionary<Guid, ResourceClutch>();
            foreach (var pair in resourcesLostPairs)
            {
                var player = this.runner.GetPlayerFromName(pair.Item1);
                dict.Add(player.Id, pair.Item2);
            }

            this.instructions.Enqueue(new ResourceUpdateEvent(dict));

            return this;
        }
        #endregion
    }
}

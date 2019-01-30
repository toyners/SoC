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
        private Dictionary<IPlayer, Action<ComputerPlayerAction>> actionProcessorsByPlayer;
        private Dictionary<IPlayer, Action<ComputerPlayerAction>> actionResolversByPlayer;
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
            this.actionProcessorsByPlayer = new Dictionary<IPlayer, Action<ComputerPlayerAction>>();
            this.actionResolversByPlayer = new Dictionary<IPlayer, Action<ComputerPlayerAction>>();
            foreach (var player in playersByName.Values)
            {
                if (player is ScenarioComputerPlayer computerPlayer)
                {
                    this.actionProcessorsByPlayer.Add(player, computerPlayer.AddAction);
                    this.actionResolversByPlayer.Add(player, computerPlayer.AddAction);
                }
                else
                {
                    this.actionProcessorsByPlayer.Add(player, this.AddActionForHumanPlayer);
                    this.actionResolversByPlayer.Add(player, this.ProcessActionForHumanPlayer);
                }
            }
        }

        private void ProcessActionForHumanPlayer(ComputerPlayerAction action)
        {
            if (action is ScenarioDropResourcesAction scenarioResourcesToDropAction)
            {
                this.LocalGameController.DropResources(scenarioResourcesToDropAction.Resources);
            }
            else if (action is BuildCityAction buildCityAction)
            {
                this.LocalGameController.BuildCity(this.TurnToken, buildCityAction.CityLocation);
            }
            else if (action is BuyDevelopmentCardAction)
            {
                this.LocalGameController.BuyDevelopmentCard(this.TurnToken);
            }
            else if (action is PlayKnightCardAction playKnightCardAction)
            {
                var knightCard = (KnightDevelopmentCard)this.player.HeldCards.Where(c => c.Type == Jabberwocky.SoC.Library.DevelopmentCardTypes.Knight).First();
                this.LocalGameController.UseKnightCard(this.TurnToken, knightCard, playKnightCardAction.NewRobberHex, playKnightCardAction.PlayerId);
            }
            else
            {
                throw new Exception("Scenario Player action not recognised");
            }
        }

        private void AddActionForComputerPlayer(ComputerPlayerAction obj)
        {
            throw new NotImplementedException();
        }

        private Queue<ComputerPlayerAction> actions = new Queue<ComputerPlayerAction>();
        private void AddActionForHumanPlayer(ComputerPlayerAction obj)
        {
            this.actions.Enqueue(obj);
        }
        #endregion

        #region Properties
        public bool IsHumanPlayer { get { return this.player is ScenarioPlayer; } }
        public string PlayerName { get { return this.player.Name; } }
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
        public TurnToken TurnToken { get; set; }
        #endregion

        #region Methods
        public PlayerActionBuilder Actions()
        {
            return new PlayerActionBuilder(this);
        }

        public BasePlayerTurn BuildCity(uint cityLocation)
        {
            this.instructions.Enqueue(new BuildCityAction(cityLocation));
            return this;
        }

        public BasePlayerTurn BuildCityEvent(uint cityLocation)
        {
            this.instructions.Enqueue(new CityBuiltEvent(this.PlayerId, cityLocation));
            return this;
        }

        public BasePlayerTurn BuildRoad(uint roadSegmentStart, uint roadSegmentEnd)
        {
            this.instructions.Enqueue(new BuildRoadSegmentAction(roadSegmentStart, roadSegmentEnd));
            return this;
        }

        public BasePlayerTurn BuildRoadEvent(uint roadSegmentStart, uint roadSegmentEnd)
        {
            this.instructions.Enqueue(new RoadSegmentBuiltEvent(this.PlayerId, roadSegmentStart, roadSegmentEnd));
            return this;
        }

        public BasePlayerTurn BuildSettlement(uint settlementLocation)
        {
            this.instructions.Enqueue(new BuildSettlementAction(settlementLocation));
            return this;
        }

        public BasePlayerTurn BuildSettlementEvent(uint settlementLocation)
        {
            this.instructions.Enqueue(new SettlementBuiltEvent(this.PlayerId, settlementLocation));
            return this;
        }

        public List<DevelopmentCardTypes> DevelopmentCardTypes = new List<DevelopmentCardTypes>();
        public BasePlayerTurn BuyDevelopmentCard(DevelopmentCardTypes developmentCardType)
        {
            this.DevelopmentCardTypes.Add(developmentCardType);
            this.instructions.Enqueue(new BuyDevelopmentCardAction());
            return this;
        }

        public BasePlayerTurn DevelopmentCardBoughtEvent()
        {
            this.instructions.Enqueue(new DevelopmentCardBoughtEvent(this.PlayerId));
            return this;
        }

        public LocalGameControllerScenarioRunner EndTurn()
        {
            return this.runner;
        }

        public BasePlayerTurn GameWinEvent(uint expectedVictoryPoints)
        {
            this.instructions.Enqueue(new GameWinEvent(this.PlayerId, expectedVictoryPoints));
            return this;
        }

        public BasePlayerTurn KnightCardPlayedEvent(uint hexLocation)
        {
            this.instructions.Enqueue(new KnightCardPlayedEvent(this.PlayerId, hexLocation));
            return this;
        }

        public BasePlayerTurn LargestArmyChangedEvent(string previousPlayerName = null)
        {
            Guid previousPlayerId = Guid.Empty;
            if (previousPlayerName != null)
                previousPlayerId = this.playersByName[previousPlayerName].Id;
            var expectedLargestArmyChangedEvent = new LargestArmyChangedEvent(this.PlayerId, previousPlayerId);
            this.instructions.Enqueue(expectedLargestArmyChangedEvent);

            return this;
        }

        public BasePlayerTurn LongestRoadBuiltEvent()
        {
            var expectedLongestRoadBuiltEvent = new LongestRoadBuiltEvent(this.PlayerId, Guid.Empty);
            this.instructions.Enqueue(expectedLongestRoadBuiltEvent);
            return this;
        }

        public ExpectedEventsBuilder Events()
        {
            return new ExpectedEventsBuilder(this, this.runner.playersByName);
        }

        //List<PlayerState> playerStates = new List<PlayerState>();

        private int actualEventIndex;
        private int expectedEventIndex;
        public virtual void AddEvent(GameEvent actualEvent)
        {
            this.actualEvents.Add(actualEvent);
            var eventProcessed = false;

            while (this.instructions.Count > 0)
            {
                var instruction = this.instructions.Dequeue();
                if (instruction is GameEvent expectedGameEvent)
                {
                    this.expectedEvents.Add(expectedGameEvent);
                    if (actualEvent.Equals(this.expectedEvents[this.expectedEventIndex]) && !eventProcessed)
                    {
                        this.expectedEventIndex++;
                        this.actualEventIndex = this.actualEvents.Count;
                        eventProcessed = true;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (instruction is ComputerPlayerAction action)
                {
                    this.actionProcessorsByPlayer[this.player].Invoke(action);
                    /*if (action is ScenarioDropResourcesAction scenarioResourcesToDropAction)
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
                    else if (this.player is ScenarioPlayer)
                    {
                        if (action is BuyDevelopmentCardAction)
                        {
                            this.LocalGameController.BuyDevelopmentCard(this.TurnToken);
                        }
                        else if (action is PlayKnightCardAction playKnightCardAction)
                        {
                            var knightCard = (KnightDevelopmentCard)this.player.HeldCards.Where(c => c.Type == Jabberwocky.SoC.Library.DevelopmentCardTypes.Knight).First();
                            this.LocalGameController.UseKnightCard(this.TurnToken, knightCard, playKnightCardAction.NewRobberHex, playKnightCardAction.PlayerId);
                        }
                        else
                        {
                            throw new Exception("Scenario Player action not recognised");
                        }
                    }
                    else if (this.player is ScenarioComputerPlayer computerPlayer)
                    {
                        computerPlayer.AddAction(action);
                    }*/
                }
                else if (instruction is PlayerState playerState)
                {
                    var player = playerState.Player;
                    //this.playerStates.Add(playerState);

                    if (player is ScenarioComputerPlayer computerPlayer)
                    {
                        playerState.Verify();
                        //var verifySnapshotAction = new ScenarioVerifySnapshotAction(playerState);
                        //computerPlayer.AddAction(verifySnapshotAction);
                    }
                    else if (player is ScenarioPlayer humanPlayer)
                    {
                        playerState.Verify();
                    }
                }
            }
        }

        public BasePlayerTurn PlayKnightCard(uint hexLocation)
        {
            this.instructions.Enqueue(new PlayKnightCardAction(hexLocation));
            return this;
        }


        public List<ScenarioSelectResourceFromPlayerAction> ScenarioSelectResourceFromPlayerActions = new List<ScenarioSelectResourceFromPlayerAction>();
        public BasePlayerTurn PlayKnightCard(uint hexLocation, string targetPlayerName, ResourceTypes resourceTaken)
        {
            this.ScenarioSelectResourceFromPlayerActions.Add(new ScenarioSelectResourceFromPlayerAction(targetPlayerName, resourceTaken));
            this.instructions.Enqueue(new PlayKnightCardAction(hexLocation, this.playersByName[targetPlayerName].Id));
            return this;
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

        public BasePlayerTurn ResourcesGainedEvent(string receivingPlayerName, string givingPlayerName, ResourceClutch expectedResources)
        {
            var receivingPlayer = this.playersByName[receivingPlayerName];
            var givingPlayer = this.playersByName[givingPlayerName];
            var resourceTransaction = new ResourceTransaction(receivingPlayer.Id, givingPlayer.Id, expectedResources);
            var expectedResourceTransactonEvent = new ResourceTransactionEvent(receivingPlayer.Id, resourceTransaction);
            this.instructions.Enqueue(expectedResourceTransactonEvent);
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

        public virtual void CompleteProcessing(TurnToken currentToken, LocalGameController localGameController)
        {
            while (this.actions.Count > 0)
            {
                var action = this.actions.Dequeue();
                this.actionResolversByPlayer[this.player]?.Invoke(action);
            }

            while (this.instructions.Count > 0)
            {
                var instruction = this.instructions.Dequeue();
                if (instruction is GameEvent expectedEvent)
                {
                    this.expectedEvents.Add(expectedEvent);
                }
                else if (instruction is ComputerPlayerAction action)
                {
                    this.actionResolversByPlayer[this.player]?.Invoke(action);
                    /*if (action is ScenarioDropResourcesAction scenarioResourcesToDropAction)
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
                    }*/
                }
                else if (instruction is PlayerState playerState)
                {
                    var player = playerState.Player;
                    //this.playerStates.Add(playerState);

                    if (player is ScenarioComputerPlayer computerPlayer)
                    {
                        //var verifySnapshotAction = new ScenarioVerifySnapshotAction(playerState);
                        //computerPlayer.AddAction(verifySnapshotAction);
                        playerState.Verify();
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
                    Assert.Fail($"Did not find {expectedEvent.GetType()} event for '{this.player.Name}' in round {this.roundNumber}, turn {this.turnNumber}.\r\n{this.GetEventDetails(expectedEvent)}");
                }
            }
        }

        protected Queue<object> instructions = new Queue<object>();
        public BasePlayerTurn DiceRollEvent(uint dice1, uint dice2)
        {
            this.instructions.Enqueue(new DiceRollEvent(this.PlayerId, dice1, dice2));
            return this;
        }

        public BasePlayerTurn DropResources(string playerName, ResourceClutch resourcesToDrop)
        {
            this.instructions.Enqueue(new ScenarioDropResourcesAction(playerName, resourcesToDrop));
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

        public BasePlayerTurn MakeDirectTradeOffer(ResourceClutch wantedResources, params Tuple<string, ResourceClutch>[] playerAnswers)
        {
            throw new NotImplementedException();
        }

        public BasePlayerTurn FinaliseTrade(ResourceClutch givenResources, string playerName, ResourceClutch receivedResources)
        {
            throw new NotImplementedException();
        }

        public BasePlayerTurn TradeFinalisedEvent(string playerName, string otherPlayerName, ResourceClutch givenResources, ResourceClutch receivingResources)
        {
            throw new NotImplementedException();
        }

        public BasePlayerTurn MakeDirectTradeOfferEvent(string firstOpponent_Babara, ResourceClutch oneWool)
        {
            throw new NotImplementedException();
        }

        public BasePlayerTurn AnswerDirectTradeOffer(ResourceClutch oneGrain)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

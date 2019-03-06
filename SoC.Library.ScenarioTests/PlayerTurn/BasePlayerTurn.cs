using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
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
    [DebuggerDisplay("{this.PlayerName} {this.RoundNumber}-{this.TurnNumber}")]
    internal class BasePlayerTurn
    {
        #region Fields
        public readonly IPlayer player;
        protected readonly LocalGameControllerScenarioRunner runner;
        protected readonly List<GameEvent> actualEvents = new List<GameEvent>();
        //private List<Tuple<string, ComputerPlayerAction>> playerActions;
        private readonly List<GameEvent> expectedEvents = new List<GameEvent>();
        public readonly int RoundNumber;
        public readonly int TurnNumber;
        //private Dictionary<string, IPlayer> playersByName;
        private Dictionary<IPlayer, Action<ComputerPlayerAction>> actionProcessorsByPlayer;
        private Dictionary<IPlayer, Action<ComputerPlayerAction>> actionResolversByPlayer;
        #endregion

        #region Construction
        public BasePlayerTurn(IPlayer player, LocalGameControllerScenarioRunner runner, int roundNumber, int turnNumber)
        {
            this.player = player;
            this.runner = runner;
            this.RoundNumber = roundNumber;
            this.TurnNumber = turnNumber;
        }

        public BasePlayerTurn(string playerName, LocalGameControllerScenarioRunner runner, int roundNumber, int turnNumber)
        {
            //this.player = playersByName[playerName];
            this.PlayerName = playerName;
            this.runner = runner;
            this.RoundNumber = roundNumber;
            this.TurnNumber = turnNumber;
            this.actionProcessorsByPlayer = new Dictionary<IPlayer, Action<ComputerPlayerAction>>();
            this.actionResolversByPlayer = new Dictionary<IPlayer, Action<ComputerPlayerAction>>();
            /*foreach (var player in playersByName.Values)
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
            }*/
        }

        private void ProcessActionForHumanPlayer(ComputerPlayerAction action)
        {
            if (action is BuildCityAction buildCityAction)
            {
                this.LocalGameController.BuildCity(this.TurnToken, buildCityAction.CityLocation);
            }
            else if (action is BuyDevelopmentCardAction)
            {
                this.LocalGameController.BuyDevelopmentCard(this.TurnToken);
            }
            else if (action is EndOfTurnAction)
            {
                this.LocalGameController.EndTurn(this.TurnToken);
            }
            else if (action is PlayKnightCardAction playKnightCardAction)
            {
                var knightCard = (KnightDevelopmentCard)this.player.HeldCards.Where(c => c.Type == Jabberwocky.SoC.Library.DevelopmentCardTypes.Knight).First();
                this.LocalGameController.UseKnightCard(this.TurnToken, knightCard, playKnightCardAction.NewRobberHex, playKnightCardAction.PlayerId);
            }
            else if (action is ScenarioDropResourcesAction scenarioResourcesToDropAction)
            {
                this.LocalGameController.DropResources(scenarioResourcesToDropAction.Resources);
            }
            else if (action is ScenarioVerifySnapshotAction scenarioVerifySnapshotAction)
            {
                scenarioVerifySnapshotAction.Verify();
            }
            else
            {
                throw new Exception("Scenario Player action not recognised");
            }
        }

        internal void Initialise(LocalGameController localGameController)
        {
            this.LocalGameController = localGameController;
        }

        private void AddActionForComputerPlayer(ComputerPlayerAction action)
        {

        }

        private Queue<ComputerPlayerAction> actions = new Queue<ComputerPlayerAction>();
        private void AddActionForHumanPlayer(ComputerPlayerAction action)
        {
            this.actions.Enqueue(action);
        }
        #endregion

        #region Properties
        public bool IsHumanPlayer { get { return this.player is ScenarioPlayer; } }
        public string PlayerName { get; private set; }
        public IDictionary<Guid, ComputerPlayerAction> ActionsByPlayerId { protected get; set; }
        public IList<GameEvent> ExpectedEvents { private get; set; }
        public IDictionary<Guid, GameEvent> GameEventsByPlayerId { private get; set; }
        public IDictionary<string, ResourceClutch> PlayerResourcesToDropByName { protected get; set; }
        public Guid PlayerId { get { return this.player.Id; } } // TODO: Fix this
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
        private LocalGameController LocalGameController { get; set; }
        public TurnToken TurnToken { get; set; }
        #endregion

        #region Methods
        public PlayerActionBuilder Actions()
        {
            return new PlayerActionBuilder(this);
        }

        public BasePlayerTurn BuildCity(uint cityLocation)
        {
            this.instructions_old.Enqueue(new BuildCityAction(cityLocation));
            return this;
        }

        private class CityBuiltEventInstruction
        {
            public uint CityLocation;
            public string PlayerName;
        }

        public BasePlayerTurn BuildCityEvent(uint cityLocation)
        {
            //this.instructions.Enqueue(new CityBuiltEvent(this.PlayerId, cityLocation));

            this.instructions_old.Enqueue(new CityBuiltEventInstruction
            {
                CityLocation = cityLocation, PlayerName = this.PlayerName
            });

            return this;
        }

        public BasePlayerTurn BuildRoad(uint roadSegmentStart, uint roadSegmentEnd)
        {
            this.instructions_old.Enqueue(new BuildRoadSegmentAction(roadSegmentStart, roadSegmentEnd));
            return this;
        }

        private class BuildRoadEventInstruction
        {
            public uint RoadSegmentStart, RoadSegmentEnd;
            public string PlayerName;
        }

        public BasePlayerTurn BuildRoadEvent(uint roadSegmentStart, uint roadSegmentEnd)
        {
            //this.instructions.Enqueue(new RoadSegmentBuiltEvent(this.PlayerId, roadSegmentStart, roadSegmentEnd));

            this.instructions_old.Enqueue(new BuildRoadEventInstruction
            {
                RoadSegmentStart = roadSegmentEnd, RoadSegmentEnd = roadSegmentEnd, PlayerName = this.PlayerName
            });
            return this;
        }

        public BasePlayerTurn BuildSettlement(uint settlementLocation)
        {
            this.instructions_old.Enqueue(new BuildSettlementAction(settlementLocation));
            return this;
        }

        private class BuildSettlementEventInstruction
        {
            public uint SettlementLocation;
            public string PlayerName;
        }

        public BasePlayerTurn BuildSettlementEvent(uint settlementLocation)
        {
            //this.instructions.Enqueue(new SettlementBuiltEvent(this.PlayerId, settlementLocation));

            this.instructions_old.Enqueue(new BuildSettlementEventInstruction
            {
                SettlementLocation = settlementLocation,
                PlayerName = this.PlayerName
            });

            return this;
        }

        public List<DevelopmentCardTypes> DevelopmentCardTypes = new List<DevelopmentCardTypes>();
        public BasePlayerTurn BuyDevelopmentCard(DevelopmentCardTypes developmentCardType)
        {
            this.DevelopmentCardTypes.Add(developmentCardType);
            this.instructions_old.Enqueue(new BuyDevelopmentCardAction());
            return this;
        }

        public BasePlayerTurn DevelopmentCardBoughtEvent()
        {
            this.instructions_old.Enqueue(new DevelopmentCardBoughtEvent(this.PlayerId));
            return this;
        }

        public LocalGameControllerScenarioRunner EndTurn()
        {
            var instruction = new ActionInstruction(this.PlayerName, ActionInstruction.OperationTypes.EndOfTurn, null);
            this.instructions_old.Enqueue(instruction);
            return this.runner;
        }

        public BasePlayerTurn GameWinEvent(uint expectedVictoryPoints)
        {
            this.instructions_old.Enqueue(new GameWinEvent(this.PlayerId, expectedVictoryPoints));
            return this;
        }

        public BasePlayerTurn KnightCardPlayedEvent(uint hexLocation)
        {
            this.instructions_old.Enqueue(new KnightCardPlayedEvent(this.PlayerId, hexLocation));
            return this;
        }

        private struct LargestArmyChangedEventInstruction
        {
            public string CurrentPlayerName, PreviousPlayerName;
        }

        public BasePlayerTurn LargestArmyChangedEvent(string previousPlayerName = null)
        {
            this.instructions_old.Enqueue(new LargestArmyChangedEventInstruction
            {
                CurrentPlayerName = this.PlayerName,
                PreviousPlayerName = previousPlayerName
            });

            return this;
        }

        public BasePlayerTurn LongestRoadBuiltEvent()
        {
            var expectedLongestRoadBuiltEvent = new LongestRoadBuiltEvent(this.PlayerId, Guid.Empty);
            this.instructions_old.Enqueue(expectedLongestRoadBuiltEvent);
            return this;
        }

        public ExpectedEventsBuilder Events()
        {
            //return new ExpectedEventsBuilder(this, this.runner.PlayersByName);
            return null;
        }

        private int actualEventIndex;
        private int expectedEventIndex;
        public virtual void AddEvent(GameEvent actualEvent)
        {
            this.actualEvents.Add(actualEvent);
        }

        public BasePlayerTurn PlayKnightCard(uint hexLocation)
        {
            this.instructions_old.Enqueue(new PlayKnightCardAction(hexLocation));
            return this;
        }

        private struct PlayKnightCardActionInstruction
        {
            public uint HexLocation;
            public string TargetPlayerName;
        }

        public List<ScenarioSelectResourceFromPlayerAction> ScenarioSelectResourceFromPlayerActions = new List<ScenarioSelectResourceFromPlayerAction>();
        public BasePlayerTurn PlayKnightCard(uint hexLocation, string targetPlayerName, ResourceTypes resourceTaken)
        {
            this.ScenarioSelectResourceFromPlayerActions.Add(new ScenarioSelectResourceFromPlayerAction(targetPlayerName, resourceTaken));
            //this.instructions.Enqueue(new PlayKnightCardAction(hexLocation, this.playersByName[targetPlayerName].Id));

            this.instructions_old.Enqueue(new PlayKnightCardActionInstruction()
            {
                HexLocation = hexLocation,
                TargetPlayerName = targetPlayerName
            });

            return this;
        }
        
        public BasePlayerTurn ResourceCollectedEvent(string playerName, params Tuple<uint, ResourceClutch>[] resourceCollectionPairs)
        {
            //var playerId = this.playersByName[playerName].Id;

            ResourceCollection[] rc = new ResourceCollection[resourceCollectionPairs.Length];
            var index = 0;
            foreach (var pair in resourceCollectionPairs)
                rc[index++] = new ResourceCollection(pair.Item1, pair.Item2);
            //this.instructions.Enqueue(new ResourcesCollectedEvent(playerId, rc));

            this.instructions_old.Enqueue(new ResourcesCollectedEventInstruction()
            {
                PlayerName = playerName,
                ResourceCollections = rc
            });

            return this;
        }

        private class ResourcesGainedEventInstruction
        {
            public string ReceivingPlayerName;
            public string GivingPlayerName;
            public ResourceClutch ExpectedResources;
        }

        public BasePlayerTurn ResourcesGainedEvent(string receivingPlayerName, string givingPlayerName, ResourceClutch expectedResources)
        {
            /*var receivingPlayer = this.playersByName[receivingPlayerName];
            var givingPlayer = this.playersByName[givingPlayerName];
            var resourceTransaction = new ResourceTransaction(receivingPlayer.Id, givingPlayer.Id, expectedResources);
            var expectedResourceTransactonEvent = new ResourceTransactionEvent(receivingPlayer.Id, resourceTransaction);
            this.instructions.Enqueue(expectedResourceTransactonEvent);*/

            this.instructions_old.Enqueue(new ResourcesGainedEventInstruction()
            {
                ReceivingPlayerName = receivingPlayerName,
                GivingPlayerName = givingPlayerName,
                ExpectedResources = expectedResources
            });

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
            return null;
            //return new PlayerResponseBuilder(this, this.runner.PlayersByName);
        }

        public PlayerStateBuilder OldState(string playerName)
        {
            throw new NotImplementedException();
        }

        public PlayerStateInstruction State(string playerName)
        {
            var playerState = new PlayerStateInstruction(playerName, this);

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

        internal void PlaceStartingInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            var instruction = new ActionInstruction(this.PlayerName,
                ActionInstruction.OperationTypes.PlaceStartingInfrastructure,
                new object[] { settlementLocation, roadEndLocation });
            this.instructions.Enqueue(instruction);
        }

        internal void PlaceSetupInfrastructureEvent()
        {
            var instruction = new PlaceSetupInfrastructureEventInstruction(this.PlayerName);
            this.instructions.Enqueue(instruction);
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

        private Queue<Instruction> instructions = new Queue<Instruction>();
        internal void PlayerSetupEvent(IDictionary<string, Guid> playerIdsByName)
        {
            this.instructions.Enqueue(new PlayerSetupEventInstruction(this.PlayerName, playerIdsByName));
        }

        internal void InitialBoardSetupEvent()
        {
            // TODO: Put in the real expected GameBoardSetup data
            this.instructions.Enqueue(new InitialBoardSetupEventInstruction(this.PlayerName, null));
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

        public bool IsVerified { get { return this.expectedEventIndex >= this.expectedEvents.Count; } }

        public bool HasInstructions { get { return this.instructions_old.Count > 0; } }

        public Queue<object> Instructions { get { return this.instructions_old; } }

        public bool IsFinished { get; set; }

        public int InstructionIndex;

        public virtual void Process(Dictionary<string, IPlayer> playersByName)
        {
            this.CheckEvents();
            while (this.instructions_old.Count > 0)
            {
                var instruction = this.instructions_old.Peek();
                if (instruction is EventInstruction_Old eventInstruction)
                {
                    this.instructions_old.Dequeue();
                    this.expectedEvents.Add(eventInstruction.Event(playersByName));
                }
                else if (instruction is GameEvent expectedEvent)
                {
                    this.instructions_old.Dequeue();
                    this.expectedEvents.Add(expectedEvent);
                    //this.CheckEvents();
                }
                else if (instruction is ComputerPlayerAction action)
                {
                    // Still got unmatched expected events so don't perform action yet
                    if (!this.IsVerified)
                        return;

                    this.instructions_old.Dequeue();
                    var player = playersByName[this.PlayerName];
                    if (player is ScenarioComputerPlayer computerPlayer)
                    {
                        computerPlayer.AddAction(action);
                    }
                    else if (player is ScenarioPlayer humanPlayer)
                    {
                        this.LocalGameController.EndPlayerTurn(this.TurnToken);
                    }
                    //this.actionResolversByPlayer[this.player]?.Invoke(action);
                }
                else if (instruction is PlayerState playerState)
                {
                    // Still got unmatched expected events so don't verify the player state yet
                    if (!this.IsVerified)
                        return;

                    this.instructions_old.Dequeue();

                    var player = playerState.Player;
                    if (player is ScenarioComputerPlayer computerPlayer)
                    {
                        playerState.Verify();
                    }
                    else if (player is ScenarioPlayer humanPlayer)
                    {
                        playerState.Verify();
                    }
                }
            }
        }

        public void FinishProcessing()
        {
            // If there are expected events yet to check then do it now
            this.CheckEvents();

            // At least one expected event was not matched with an actual event.
            if (this.expectedEventIndex < this.expectedEvents.Count)
            {
                //this.LocalGameController.Quit();
                var expectedEvent = this.expectedEvents[this.expectedEventIndex];
                Assert.Fail($"Did not find {expectedEvent.GetType()} event for '{this.PlayerName}' in round {this.RoundNumber}, turn {this.TurnNumber}.\r\n{this.GetEventDetails(expectedEvent)}");
            }
        }

        private void CheckEvents()
        {
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
            }
        }

        protected Queue<object> instructions_old = new Queue<object>();
        public BasePlayerTurn DiceRollEvent(uint dice1, uint dice2)
        {
            this.instructions_old.Enqueue(new DiceRollEventInstruction
            {
                Dice1 = dice1, Dice2 = dice2, PlayerName = this.PlayerName
            });
            return this;
        }

        public BasePlayerTurn DropResources(string playerName, ResourceClutch resourcesToDrop)
        {
            this.instructions_old.Enqueue(new ScenarioDropResourcesAction(playerName, resourcesToDrop));
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

            this.instructions_old.Enqueue(new ResourceUpdateEvent(dict));

            return this;
        }

        public BasePlayerTurn MakeDirectTradeOffer(ResourceClutch wantedResources, params Tuple<string, ResourceClutch>[] playerAnswers)
        {
            this.instructions_old.Enqueue(new ScenarioMakeDirectTradeOfferAction(this.player.Name, wantedResources));
            return this;
        }

        public BasePlayerTurn MakeDirectTradeOffer(ResourceClutch wantedResources)
        {
            var instruction = new ActionInstruction(this.PlayerName, 
                ActionInstruction.OperationTypes.MakeDirectTradeOffer,
                new object[] { wantedResources });
            this.instructions.Enqueue(instruction);
            
            return this;
        }

        public BasePlayerTurn FinaliseTrade(ResourceClutch givenResources, string playerName, ResourceClutch receivedResources)
        {
            throw new NotImplementedException();
        }

        public BasePlayerTurn TradeWithPlayerCompletedEvent(string playerName, string buyingPlayerName, ResourceClutch buyingResources, string sellingPlayerName, ResourceClutch sellingResources)
        {
            this.instructions.Enqueue(new TradeWithPlayerCompletedEventInstruction
            (
                playerName,
                buyingPlayerName,
                buyingResources,
                sellingPlayerName,
                sellingResources
            ));

            return this;
        }

        public BasePlayerTurn MakeDirectTradeOfferEvent(string playerName, string buyingPlayerName, ResourceClutch resources)
        {
            var instruction = new MakeDirectTradeOfferEventInstruction(playerName, buyingPlayerName, resources);
            this.instructions.Enqueue(instruction);

            return this;
        }

        public BasePlayerTurn AnswerDirectTradeOffer(string playerName, ResourceClutch resources)
        {
            this.instructions.Enqueue(new ActionInstruction(playerName, 
                ActionInstruction.OperationTypes.AnswerDirectTradeOffer,
                new object[] { resources }));

            return this;
        }

        public BasePlayerTurn AnswerDirectTradeOfferEvent(string playerName, string buyingPlayerName, ResourceClutch wantedResources)
        {
            var instruction = new AnswerDirectTradeOfferEventInstruction(playerName, buyingPlayerName, wantedResources);
            this.instructions.Enqueue(instruction);

            return this;
        }
        #endregion

        #region Structures
        private abstract class EventInstruction_Old
        {
            public abstract GameEvent Event(Dictionary<string, IPlayer> playersByName);
        }

        private class DiceRollEventInstruction : EventInstruction_Old
        {
            public uint Dice1, Dice2;
            public string PlayerName;

            public override GameEvent Event(Dictionary<string, IPlayer> playersByName)
            {
                return new DiceRollEvent(playersByName[this.PlayerName].Id, this.Dice1, this.Dice2);
            }
        }

        /*private class MakeDirectTradeOfferEventInstruction : EventInstruction_Old
        {
            public string PlayerName;
            public string BuyingPlayerName;
            public ResourceClutch Resources;

            public override GameEvent Event(Dictionary<string, IPlayer> playersByName)
            {
                return new MakeDirectTradeOfferEvent(
                    playersByName[this.PlayerName].Id, 
                    playersByName[this.BuyingPlayerName].Id, 
                    this.Resources);
            }
        }*/

        private class ResourcesCollectedEventInstruction : EventInstruction_Old
        {
            public string PlayerName;
            public ResourceCollection[] ResourceCollections;

            public override GameEvent Event(Dictionary<string, IPlayer> playersByName)
            {
                return new ResourcesCollectedEvent(playersByName[this.PlayerName].Id, this.ResourceCollections);
            }
        }

        
        #endregion
    }

    internal class ScenarioInitialBoardSetupEvent : GameEvent
    {
        // TODO: Remove this and use InitialBoardSetupEvent with the game board data
        public ScenarioInitialBoardSetupEvent() : base(Guid.Empty)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null || obj.GetType() != typeof(InitialBoardSetupEventArgs))
                return false;

            return ((InitialBoardSetupEventArgs)obj).Item != null;
        }
    }

    internal class PlayerSetupTurn : BasePlayerTurn
    {
        public PlayerSetupTurn(string playerName, LocalGameControllerScenarioRunner runner, int roundNumber, int turnNumber)
            : base(playerName, runner, roundNumber, turnNumber)
        {
        }

        public PlayerSetupTurn StartingInfrastructure(uint settlementLocation, uint roadEnd)
        {
            return this;
        }

        public PlayerSetupTurn InitalBoardSetupEvent()
        {
            return this;
        }
    }
}

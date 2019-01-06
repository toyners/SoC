using System;
using System.Collections.Generic;
using System.Linq;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
using Jabberwocky.SoC.Library.Enums;
using Jabberwocky.SoC.Library.GameActions;
using Jabberwocky.SoC.Library.GameBoards;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;
using NUnit.Framework;
using SoC.Library.ScenarioTests.PlayerTurn;

namespace SoC.Library.ScenarioTests
{
    internal class LocalGameControllerScenarioRunner
    {
        #region Enums
        public enum GameEventTypes
        {
            LargestArmyEvent
        }

        public enum EventPositions
        {
            Any,
            Last
        }
        #endregion

        #region Fields
        private static LocalGameControllerScenarioRunner localGameControllerScenarioBuilder;
        private readonly ScenarioDevelopmentCardHolder developmentCardHolder = new ScenarioDevelopmentCardHolder();
        private readonly Dictionary<Guid, List<DevelopmentCard>> developmentCardsByPlayerId = new Dictionary<Guid, List<DevelopmentCard>>();
        private readonly List<PlayerSetupAction> firstRoundSetupActions = new List<PlayerSetupAction>(4);
        private readonly Dictionary<Type, GameEvent> lastEventsByType = new Dictionary<Type, GameEvent>();
        private readonly Dictionary<string, IPlayer> playersByName = new Dictionary<string, IPlayer>();
        private readonly Queue<Instruction> playerInstructions = new Queue<Instruction>();
        private readonly ScenarioPlayerPool playerPool = new ScenarioPlayerPool();
        private readonly List<BasePlayerTurn> playerTurns = new List<BasePlayerTurn>();
        private readonly List<PlayerSetupAction> secondRoundSetupActions = new List<PlayerSetupAction>(4);
        private readonly Dictionary<string, ScenarioComputerPlayer> computerPlayersByName = new Dictionary<string, ScenarioComputerPlayer>();
        private readonly List<IPlayer> players = new List<IPlayer>(4);
        private List<GameEvent> actualEvents = null;
        private TurnToken currentToken;
        private Dictionary<GameEventTypes, Delegate> eventHandlersByGameEventType;
        private int expectedEventCount;
        private GameBoard gameBoard;
        private LocalGameController localGameController = null;
        private Queue<GameEvent> relevantEvents = null;
        #endregion 

        #region Construction
        private LocalGameControllerScenarioRunner()
        {
            this.NumberGenerator = new ScenarioNumberGenerator();
        }
        #endregion

        #region Properties
        internal ScenarioNumberGenerator NumberGenerator { get; }
        #endregion

        #region Methods
        public static LocalGameControllerScenarioRunner LocalGameController()
        {
            return new LocalGameControllerScenarioRunner();
        }

        public LocalGameControllerScenarioRunner Build(Dictionary<GameEventTypes, Delegate> eventHandlersByGameEventType = null, int expectedEventCount = -1)
        {
            if (this.gameBoard == null)
                this.gameBoard = new GameBoard(BoardSizes.Standard);

            this.localGameController = new LocalGameController(
                this.NumberGenerator, 
                this.playerPool, 
                this.gameBoard, 
                this.developmentCardHolder);
            this.localGameController.DiceRollEvent = (Guid playerId, uint dice1, uint dice2) =>
            {
                this.actualEvents.Add(new DiceRollEvent(playerId, dice1, dice2));
                //this.DiceRollEventHandler2();
            };
            this.localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard c) => { this.actualEvents.Add(new BuyDevelopmentCardEvent(this.players[0].Id)); };
            this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { Assert.Fail(e.Message); };
            this.localGameController.GameEvents = this.GameEventsHandler;
            this.localGameController.LargestArmyEvent = (newPlayerId, previousPlayerId) => { this.actualEvents.Add(new LargestArmyChangedEvent(newPlayerId, previousPlayerId)); };
            this.localGameController.PlayKnightCardEvent = (PlayKnightCardEvent p) => { this.actualEvents.Add(p); };
            this.localGameController.ResourcesTransferredEvent = (ResourceTransactionList list) =>
            {
                this.actualEvents.Add(new ResourceTransactionEvent(this.players[0].Id, list));
            };
            this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { this.currentToken = t; StartOfTurn(); };
            this.localGameController.StartOpponentTurnEvent = (Guid g) => { this.StartOfTurn(); };

            this.eventHandlersByGameEventType = eventHandlersByGameEventType;
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

        private ScenarioPlayer expectedPlayer = null;
        private readonly Dictionary<string, ScenarioPlayer> expectedPlayersByName = new Dictionary<string, ScenarioPlayer>();
        public ScenarioPlayer ExpectPlayer(string mainPlayerName)
        {
            var expectedPlayer = new ScenarioPlayer(mainPlayerName, this);
            return expectedPlayer;
        }

        public IPlayer GetPlayerFromName(string playerName)
        {
            return this.playersByName[playerName];
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

        public LocalGameControllerScenarioRunner LargestArmyChangedEvent(string newPlayerName, string previousPlayerName = null, EventPositions eventPosition = EventPositions.Any)
        {
            var newPlayer = this.playersByName[newPlayerName];
            Guid previousPlayerId = Guid.Empty;
            if (previousPlayerName != null)
                previousPlayerId = this.playersByName[previousPlayerName].Id;
            var expectedLargestArmyChangedEvent = new LargestArmyChangedEvent(newPlayer.Id, previousPlayerId);
            this.relevantEvents.Enqueue(expectedLargestArmyChangedEvent);

            if (eventPosition == EventPositions.Last)
                this.lastEventsByType.Add(expectedLargestArmyChangedEvent.GetType(), expectedLargestArmyChangedEvent);

            return this;
        }

        public LocalGameControllerScenarioRunner LongestRoadBuiltEvent(string newPlayerName)
        {
            var newPlayer = this.playersByName[newPlayerName];
            var expectedLongestRoadBuiltEvent = new LongestRoadBuiltEvent(newPlayer.Id, Guid.Empty);
            this.relevantEvents.Enqueue(expectedLongestRoadBuiltEvent);
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
            this.relevantEvents.Enqueue(expectedDiceRollEvent);
            return this;
        }

        public LocalGameControllerScenarioRunner ResourcesGainedEvent(string receivingPlayerName, string givingPlayerName, ResourceClutch expectedResources)
        {
            var receivingPlayer = this.playersByName[receivingPlayerName];
            var givingPlayer = this.playersByName[givingPlayerName];
            var resourceTransaction = new ResourceTransaction(receivingPlayer.Id, givingPlayer.Id, expectedResources);
            var expectedResourceTransactonEvent = new ResourceTransactionEvent(receivingPlayer.Id, resourceTransaction);
            this.relevantEvents.Enqueue(expectedResourceTransactonEvent);
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

            //this.currentGameRound = null;

            //playerTurnsToReview.Enqueue(null);

            for (var gameRoundIndex = 0; gameRoundIndex < this.gameRounds.Count; gameRoundIndex++)
            {
                var gameRound = this.gameRounds[gameRoundIndex];
                for (var playerTurnIndex = 0; playerTurnIndex < gameRound.PlayerTurns.Count; playerTurnIndex++)
                {
                    var playerTurn = gameRound.PlayerTurns[playerTurnIndex];

                    this.NumberGenerator.AddTwoDiceRoll(playerTurn.Dice1, playerTurn.Dice2);

                    if (gameRoundIndex == 0 && playerTurnIndex == 0)
                        this.localGameController.StartGamePlay();

                    var runnerActions = playerTurn.GetRunnerActions();
                    if (runnerActions != null && runnerActions.Count == 1)
                    {
                        this.AddDevelopmentCardToBuy(Guid.Empty, ((InsertDevelopmentCardAction)runnerActions[0]).DevelopmentCardType);
                    }

                    playerTurn.ResolveActions(this.currentToken, this.localGameController);
                    playerTurnsToReview.Enqueue(playerTurn);
                }

                if (gameRoundIndex == this.gameRounds.Count - 1)
                {
                    var fillDiceRollCount = (4 - gameRound.PlayerTurns.Count) + 1;
                    while (fillDiceRollCount-- > 0)
                        this.NumberGenerator.AddTwoDiceRoll(1, 1);
                }

                this.localGameController.EndTurn(this.currentToken);
            }

            //this.CompleteGamePlay();

            // Set up first dice roll
            

            //this.ComepleteGamePlay2();

            

            /*if (this.relevantEvents != null && this.actualEvents != null)
            {
                if (this.expectedEventCount != -1)
                    Assert.AreEqual(this.expectedEventCount, this.actualEvents.Count, $"Expected event count {this.expectedEventCount} but found actual event count {this.actualEvents.Count}");

                var actualEventIndex = 0;
                while (this.relevantEvents.Count > 0)
                {
                    GameEvent lastEvent = null;
                    var expectedEvent = this.relevantEvents.Dequeue();
                    var foundEvent = false;
                    while (actualEventIndex < this.actualEvents.Count)
                    {
                        var actualEvent = this.actualEvents[actualEventIndex++];

                        if (this.lastEventsByType.TryGetValue(actualEvent.GetType(), out lastEvent) && lastEvent == null)
                        {
                            Assert.Fail($"{actualEvent} event found after last event of type {actualEvent.GetType()} was matched.");
                        }

                        if (expectedEvent.Equals(actualEvent))
                        {
                            if (this.lastEventsByType.TryGetValue(actualEvent.GetType(), out lastEvent) && lastEvent != null)
                            {
                                this.lastEventsByType[actualEvent.GetType()] = null;
                            }

                            foundEvent = true;
                            break;
                        }
                    }

                    if (!foundEvent)
                        Assert.Fail(this.ToMessage(expectedEvent));
                }
            }

            foreach (var expectedPlayerPair in this.expectedPlayersByName)
            {
                var expectedPlayer = expectedPlayerPair.Value;
                var actualPlayer = this.playersByName[expectedPlayer.Name];

                Assert.AreEqual(expectedPlayer.VictoryPoints, actualPlayer.VictoryPoints, $"Expected player '{actualPlayer.Name}' to have {expectedPlayer.VictoryPoints} victory points but has {actualPlayer.VictoryPoints} victory points");
            }*/

            return this.localGameController;
        }

        public LocalGameControllerScenarioRunner GameWinEvent(string firstOpponentName, uint expectedVictoryPoints)
        {
            var playerId = this.playersByName[firstOpponentName].Id;
            var expectedGameWonEvent = new GameWinEvent(playerId, expectedVictoryPoints);
            this.relevantEvents.Enqueue(expectedGameWonEvent);
            return this;
        }

        private GameRound currentGameRound = null;
        private List<GameRound> gameRounds = new List<GameRound>();
        public BasePlayerTurn PlayerTurn(string playerName, uint dice1, uint dice2)
        {
            if (currentGameRound == null || currentGameRound.IsComplete)
            {
                currentGameRound = new GameRound();
                this.gameRounds.Add(currentGameRound);
            }

            BasePlayerTurn playerTurn;
            var player = this.playersByName[playerName];
            if (player.IsComputer)
                playerTurn = new ComputerPlayerTurn(player, dice1, dice2, this);
            else
                playerTurn = new HumanPlayerTurn(player, dice1, dice2, this);

            currentGameRound.Add(playerTurn);

            return playerTurn;
        }

        public LocalGameControllerScenarioRunner PlayKnightCardEvent(string playerName)
        {
            var player = this.playersByName[playerName];
            var expectedPlayKnightCardEvent = new PlayKnightCardEvent(player.Id);
            this.relevantEvents.Enqueue(expectedPlayKnightCardEvent);
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

        public LocalGameControllerScenarioRunner WithNoResourceCollection()
        {
            this.gameBoard = new ScenarioGameBoardWithNoResourcesCollected();
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
                this.NumberGenerator.AddTwoDiceRoll(roll / 2, roll / 2);

            return this;
        }

        public BasePlayerTurn DuringPlayerTurn(string playerName, uint dice1, uint dice2)
        {
            this.NumberGenerator.AddTwoDiceRoll(dice1, dice2);

            /*BasePlayerTurn playerTurn = null;

            if (playerName == this.players[0].Name)
            {
                playerTurn = new HumanPlayerTurn(this, this.players[0]);
            }
            else
            {
                playerTurn = new ComputerPlayerTurn(this, this.computerPlayersByName[playerName]);
            }

            this.playerTurns.Add(playerTurn);
            return playerTurn;*/
            throw new NotImplementedException();
        }

        public LocalGameControllerScenarioRunner BuyDevelopmentCardEvent(string playerName, DevelopmentCardTypes developmentCardType)
        {
            var player = this.playersByName[playerName];
            if (player is ScenarioPlayer mockPlayer)
            {

            }
            else if (player is ScenarioComputerPlayer mockComputerPlayer)
            {
                var expectedBuyDevelopmentCardEvent = new ScenarioBuyDevelopmentCardEvent(mockComputerPlayer, developmentCardType);
                this.relevantEvents.Enqueue(expectedBuyDevelopmentCardEvent);
            }

            return this;
        }

        public LocalGameControllerScenarioRunner WithStartingResourcesForPlayer(string playerName, ResourceClutch playerResources)
        {
            var player = this.playersByName[playerName];
            player.AddResources(playerResources);
            return this;
        }

        public LocalGameControllerScenarioRunner VictoryPoints(uint expectedVictoryPoints)
        {
            this.expectedPlayer.VictoryPoints(expectedVictoryPoints);
            return this;
        }

        internal void AddDevelopmentCardToBuy(Guid playerId, DevelopmentCardTypes developmentCardType)
        {
            DevelopmentCard developmentCard = null;
            switch (developmentCardType)
            {
                case DevelopmentCardTypes.Knight: developmentCard = new KnightDevelopmentCard(); break;
                default: throw new Exception($"Development card type {developmentCardType} not recognised");
            }

            this.developmentCardHolder.AddDevelopmentCard(developmentCard);

            if (!this.developmentCardsByPlayerId.TryGetValue(playerId, out var developmentCardsForPlayerId))
            {
                developmentCardsForPlayerId = new List<DevelopmentCard>();
                this.developmentCardsByPlayerId.Add(playerId, developmentCardsForPlayerId);
            }

            developmentCardsForPlayerId.Add(developmentCard);
        }

        private void CompleteGamePlay()
        {
            for (var index = 0; index < this.playerTurns.Count; index++)
            {
                var turn = this.playerTurns[index];
                if (turn is HumanPlayerTurn && index > 0)
                    this.localGameController.EndTurn(this.currentToken);

                turn.ResolveActions(this.currentToken, this.localGameController);
            }

            this.localGameController.EndTurn(this.currentToken);
        }

        private void ComepleteGamePlay2()
        {
            var actualEventIndex = 0;

            foreach (var gameRound in this.gameRounds)
            {
                foreach (var playerTurn in gameRound.PlayerTurns)
                {
                    this.NumberGenerator.AddTwoDiceRoll(playerTurn.Dice1, playerTurn.Dice2);

                    // Run player actions (if any)
                    var runnerActions = playerTurn.GetRunnerActions();
                    if (runnerActions != null)
                    {
                        foreach (var runnerAction in runnerActions)
                        {
                            this.AddDevelopmentCardToBuy(playerTurn.PlayerId, ((InsertDevelopmentCardAction)runnerAction).DevelopmentCardType);
                        }
                    }

                    playerTurn.ResolveActions(this.currentToken, this.localGameController);

                    // Check actual events against expected events (if any)
                    var expectedEvents = playerTurn.GetExpectedEvents();

                    foreach (var expectedEvent in expectedEvents)
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
                            Assert.Fail(this.ToMessage(expectedEvent));
                    }

                    actualEventIndex = this.actualEvents.Count;

                    // Check player state

                }
            }
        }

        private IPlayer CreatePlayer(string name, bool isComputerPlayer)
        {
            IPlayer player = isComputerPlayer
                ? new ScenarioComputerPlayer(name, this.NumberGenerator) as IPlayer
                : new ScenarioPlayer(name, null) as IPlayer;

            this.players.Add(player);
            this.playersByName.Add(name, player);
            this.playerPool.AddPlayer(player);
            if (isComputerPlayer)
                this.computerPlayersByName.Add(name, (ScenarioComputerPlayer)player);

            return player;
        }

        private void DiceRollEventHandler(Guid playerId, uint dice1, uint dice2)
        {
            this.actualEvents.Add(new DiceRollEvent(playerId, dice1, dice2));
        }

        private void StartOfTurn()
        {
            if (this.playerTurnsToReview.Count == 0)
                return;

            var previousTurn = playerTurnsToReview.Dequeue();
            if (previousTurn == null)
                return;

            var expectedEvents = previousTurn.GetExpectedEvents();
            if (expectedEvents != null)
            {
                foreach (var expectedEvent in expectedEvents)
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
                        Assert.Fail(this.ToMessage(expectedEvent));
                }

                actualEventIndex = this.actualEvents.Count;
            }

            previousTurn.CompareSnapshot();
        }

        Queue<BasePlayerTurn> playerTurnsToReview = new Queue<BasePlayerTurn>();
        BasePlayerTurn previousTurn;
        BasePlayerTurn currentTurn;
        int actualEventIndex = 0;
        private void DiceRollEventHandler2()
        {
            if (currentTurn != null)
            {
                previousTurn = currentTurn;
            }

            currentTurn = this.GetNextPlayerTurn();

            if (previousTurn != null)
            {
                var expectedEvents = previousTurn.GetExpectedEvents();
                if (expectedEvents != null)
                {
                    foreach (var expectedEvent in expectedEvents)
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
                            Assert.Fail(this.ToMessage(expectedEvent));
                    }

                    actualEventIndex = this.actualEvents.Count;
                }

                var snapshot = previousTurn.GetPlayerSnapshot();
                if (snapshot != null)
                {

                }
            }

            //currentTurn.ResolveActions(this.currentToken, this.localGameController);

            /*if (currentTurn is HumanPlayerTurn)
                this.localGameController.EndTurn(this.currentToken);
            else
                this.NumberGenerator.AddTwoDiceRoll(currentTurn.Dice1, currentTurn.Dice2);*/
        }

        int gameRoundIndex = 0;
        int playerTurnIndex = 0;
        private BasePlayerTurn GetNextPlayerTurn()
        {
            if (this.currentGameRound == null)
            {
                this.currentGameRound = this.gameRounds[gameRoundIndex++];
                playerTurnIndex = 0;
            }
            else if (playerTurnIndex == this.currentGameRound.PlayerTurns.Count)
            {
                if (gameRoundIndex == this.gameRounds.Count)
                    return null;

                this.currentGameRound = this.gameRounds[gameRoundIndex++];
                playerTurnIndex = 0;
            }

            return this.currentGameRound.PlayerTurns[playerTurnIndex++];
        }

        private void GameEventsHandler(List<GameEvent> gameEvents)
        {
            this.actualEvents.AddRange(gameEvents);

            if (this.eventHandlersByGameEventType != null)
            {
                foreach (var gameEvent in gameEvents)
                {
                    if (gameEvent is LargestArmyChangedEvent && this.eventHandlersByGameEventType.TryGetValue(GameEventTypes.LargestArmyEvent, out var eventHandler))
                        ((Action<LargestArmyChangedEvent>)eventHandler).Invoke((LargestArmyChangedEvent)gameEvent);
                }
            }
        }

        private string ToMessage(GameEvent gameEvent)
        {
            var player = this.players.Where(p => p.Id.Equals(gameEvent.PlayerId)).FirstOrDefault();

            var message = $"Did not find {gameEvent.GetType()} event for '{player.Name}'.";
            
            if (gameEvent is DiceRollEvent diceRollEvent)
            {
                message += $"\r\nDice 1 is {diceRollEvent.Dice1}, Dice roll 2 is {diceRollEvent.Dice2}";
            }
            else if (gameEvent is ResourceTransactionEvent resourceTransactionEvent)
            {
                message += $"\r\nResource transaction count is {resourceTransactionEvent.ResourceTransactions.Count}";
                for (var index = 0; index < resourceTransactionEvent.ResourceTransactions.Count; index++)
                {
                    var resourceTransaction = resourceTransactionEvent.ResourceTransactions[index];
                    var receivingPlayer = this.players.Where(p => p.Id.Equals(resourceTransaction.ReceivingPlayerId)).FirstOrDefault();
                    var givingPlayer = this.players.Where(p => p.Id.Equals(resourceTransaction.GivingPlayerId)).FirstOrDefault();
                    message += $"\r\nTransaction is: Receiving player '{receivingPlayer.Name}', Giving player '{givingPlayer.Name}', Resources {resourceTransaction.Resources}";
                }
            }
            

            return message;
        }

        private List<GameRound> rounds = new List<GameRound>();
        #endregion
    }

    internal class GameRound
    {
        public List<BasePlayerTurn> PlayerTurns = new List<BasePlayerTurn>();

        public bool IsComplete { get { return this.PlayerTurns.Count == 4; } }

        public void Add(BasePlayerTurn playerTurn)
        {
            this.PlayerTurns.Add(playerTurn);
        }
    }


    internal abstract class RunnerAction { }

    internal class InsertDevelopmentCardAction : RunnerAction
    {
        public DevelopmentCardTypes DevelopmentCardType;
    }

    internal class PlayerActionBuilder
    {
        private BasePlayerTurn playerTurn;
        public List<RunnerAction> runnerActions = new List<RunnerAction>();
        public List<ComputerPlayerAction> playerActions = new List<ComputerPlayerAction>();
        public PlayerActionBuilder(BasePlayerTurn playerTurn)
        {
            this.playerTurn = playerTurn;
        }

        public BasePlayerTurn End()
        {
            return this.playerTurn;
        }

        public PlayerActionBuilder BuyDevelopmentCard(DevelopmentCardTypes developmentCardType)
        {
            this.runnerActions.Add(new InsertDevelopmentCardAction { DevelopmentCardType = developmentCardType });
            this.playerActions.Add(new ComputerPlayerAction(ComputerPlayerActionTypes.BuyDevelopmentCard));
            return this;
        }
    }

    internal class ExpectedEventsBuilder
    {
        private BasePlayerTurn playerTurn;
        public List<GameEvent> expectedEvents = new List<GameEvent>();
        public ExpectedEventsBuilder(BasePlayerTurn playerTurn)
        {
            this.playerTurn = playerTurn;
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

        internal ExpectedEventsBuilder DiceRollEvent(uint dice1, uint dice2)
        {
            this.expectedEvents.Add(new DiceRollEvent(this.playerTurn.PlayerId, dice1, dice2));
            return this;
        }
    }

    internal class PlayerStateBuilder
    {
        private BasePlayerTurn playerTurn;
        public PlayerSnapshot playerSnapshot;
        public PlayerStateBuilder(BasePlayerTurn playerTurn)
        {
            this.playerTurn = playerTurn;
        }

        public PlayerStateBuilder HeldCards(params DevelopmentCardTypes[] cards)
        {
            playerSnapshot = new PlayerSnapshot();
            playerSnapshot.heldCards = new List<DevelopmentCardTypes>(cards);
            return this;
        }

        public BasePlayerTurn End()
        {
            return this.playerTurn;
        }
    }
}
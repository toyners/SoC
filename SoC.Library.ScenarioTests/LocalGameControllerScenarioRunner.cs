using System;
using System.Collections.Generic;
using System.Linq;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
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
        //private readonly Dictionary<Guid, List<DevelopmentCard>> developmentCardsByPlayerId = new Dictionary<Guid, List<DevelopmentCard>>();
        private readonly List<PlayerSetupAction> firstRoundSetupActions = new List<PlayerSetupAction>(4);
        private readonly Dictionary<Type, GameEvent> lastEventsByType = new Dictionary<Type, GameEvent>();
        public readonly Dictionary<string, IPlayer> playersByName = new Dictionary<string, IPlayer>();
        private readonly Queue<Instruction> playerInstructions = new Queue<Instruction>();
        private readonly ScenarioPlayerPool playerPool = new ScenarioPlayerPool();
        private readonly List<BasePlayerTurn> playerTurns = new List<BasePlayerTurn>();
        private readonly List<PlayerSetupAction> secondRoundSetupActions = new List<PlayerSetupAction>(4);
        private readonly Dictionary<string, ScenarioComputerPlayer> computerPlayersByName = new Dictionary<string, ScenarioComputerPlayer>();
        private readonly List<IPlayer> players = new List<IPlayer>(4);
        //private List<GameEvent> actualEvents = null;
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
                this.currentTurn?.AddEvent(new DiceRollEvent(playerId, dice1, dice2));
            };
            this.localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard c) => { this.currentTurn?.AddEvent(new BuyDevelopmentCardEvent(this.players[0].Id)); };
            this.localGameController.ErrorRaisedEvent = (ErrorDetails e) => { Assert.Fail(e.Message); };
            this.localGameController.GameEvents = this.GameEventsHandler;
            this.localGameController.LargestArmyEvent = (newPlayerId, previousPlayerId) => { this.currentTurn?.AddEvent(new LargestArmyChangedEvent(newPlayerId, previousPlayerId)); };
            this.localGameController.PlayKnightCardEvent = (PlayKnightCardEvent p) => { this.currentTurn?.AddEvent(p); };
            this.localGameController.ResourcesTransferredEvent = (ResourceTransactionList list) =>
            {
                this.currentTurn?.AddEvent(new ResourceTransactionEvent(this.players[0].Id, list));
            };
            this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { this.currentToken = t; StartOfTurn(); };
            this.localGameController.StartOpponentTurnEvent = (Guid g) => { this.StartOfTurn(); };

            this.eventHandlersByGameEventType = eventHandlersByGameEventType;
            this.expectedEventCount = expectedEventCount;
            this.relevantEvents = new Queue<GameEvent>();

            // Flatten game rounds into queue of turns. Fill out the dice rolls
            for (var gameRoundIndex = 0; gameRoundIndex < this.gameRounds.Count; gameRoundIndex++)
            {
                var gameRound = this.gameRounds[gameRoundIndex];
                
                for (var playerTurnIndex = 0; playerTurnIndex < gameRound.PlayerTurns.Count; playerTurnIndex++)
                {
                    var playerTurn = gameRound.PlayerTurns[playerTurnIndex];
                    this.turns.Add(playerTurn);
                }

                if (gameRoundIndex == this.gameRounds.Count - 1)
                {
                    var fillDiceRollCount = (4 - gameRound.PlayerTurns.Count) + 1;
                    while (fillDiceRollCount-- > 0)
                        this.NumberGenerator.AddTwoDiceRoll(1, 1);
                }
            }

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

        int actionIndex = 0;
        int reviewIndex = 0;
        List<BasePlayerTurn> turns = new List<BasePlayerTurn>();
        public LocalGameController Run()
        {
            this.localGameController.JoinGame();
            this.localGameController.LaunchGame();
            this.localGameController.StartGameSetup();

            var placeInfrastructureInstruction = (PlaceInfrastructureInstruction)this.playerInstructions.Dequeue();
            this.localGameController.ContinueGameSetup(placeInfrastructureInstruction.SettlementLocation, placeInfrastructureInstruction.RoadEndLocation);

            placeInfrastructureInstruction = (PlaceInfrastructureInstruction)this.playerInstructions.Dequeue();
            this.localGameController.CompleteGameSetup(placeInfrastructureInstruction.SettlementLocation, placeInfrastructureInstruction.RoadEndLocation);

            for (; actionIndex < this.turns.Count; actionIndex += 4)
            {
                if (actionIndex == 0)
                    this.localGameController.StartGamePlay();

                var index = actionIndex;
                var endIndex = actionIndex + 3;
                while (index <= endIndex && index < this.turns.Count)
                {
                    var playerTurn = this.turns[index];
                    var runnerActions = playerTurn.GetRunnerActions();
                    if (runnerActions != null && runnerActions.Count > 0)
                    {
                        foreach (var runnerAction in runnerActions)
                            this.AddDevelopmentCardToBuy(((InsertDevelopmentCardAction)runnerAction).DevelopmentCardType);
                    }

                    playerTurn.ResolveActions(this.currentToken, this.localGameController);
                    index++;
                }

                this.localGameController.EndTurn(this.currentToken);
            }

            return this.localGameController;
        }

        internal IPlayer GetPlayer(Guid playerId)
        {
            return this.players.Where(p => p.Id == playerId).First();
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
            this.NumberGenerator.AddTwoDiceRoll(dice1, dice2);

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

        internal void AddDevelopmentCardToBuy(DevelopmentCardTypes developmentCardType)
        {
            DevelopmentCard developmentCard = null;
            switch (developmentCardType)
            {
                case DevelopmentCardTypes.Knight: developmentCard = new KnightDevelopmentCard(); break;
                default: throw new Exception($"Development card type {developmentCardType} not recognised");
            }

            this.developmentCardHolder.AddDevelopmentCard(developmentCard);
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
            this.currentTurn.AddEvent(new DiceRollEvent(playerId, dice1, dice2));
        }

        int currentIndex = 0;
        private void StartOfTurn()
        {
            if (this.currentIndex < this.turns.Count)
                this.currentTurn = this.turns[this.currentIndex];
            else
                this.currentTurn = null;

            var previousIndex = this.currentIndex - 1;
            if (previousIndex >= 0 && previousIndex < this.turns.Count)
            {
                var previousTurn = this.turns[previousIndex];
                previousTurn.VerifyEvents();
                previousTurn.CompareSnapshot();
            }

            this.currentIndex++;
        }

        Queue<BasePlayerTurn> playerTurnsToReview = new Queue<BasePlayerTurn>();
        BasePlayerTurn previousTurn;
        BasePlayerTurn currentTurn = new GameSetupTurn();
        int actualEventIndex = 0;

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
            this.currentTurn?.AddEvents(gameEvents);

            if (this.eventHandlersByGameEventType != null)
            {
                foreach (var gameEvent in gameEvents)
                {
                    if (gameEvent is LargestArmyChangedEvent && this.eventHandlersByGameEventType.TryGetValue(GameEventTypes.LargestArmyEvent, out var eventHandler))
                        ((Action<LargestArmyChangedEvent>)eventHandler).Invoke((LargestArmyChangedEvent)gameEvent);
                }
            }
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
}
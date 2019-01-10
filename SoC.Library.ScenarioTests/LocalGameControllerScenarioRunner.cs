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
        private readonly ScenarioDevelopmentCardHolder developmentCardHolder = new ScenarioDevelopmentCardHolder();
        private readonly List<PlayerSetupAction> firstRoundSetupActions = new List<PlayerSetupAction>(4);
        private readonly Dictionary<Type, GameEvent> lastEventsByType = new Dictionary<Type, GameEvent>();
        public readonly Dictionary<string, IPlayer> playersByName = new Dictionary<string, IPlayer>();
        private readonly Queue<Instruction> playerInstructions = new Queue<Instruction>();
        private readonly ScenarioPlayerPool playerPool = new ScenarioPlayerPool();
        private readonly List<BasePlayerTurn> playerTurns = new List<BasePlayerTurn>();
        private readonly List<PlayerSetupAction> secondRoundSetupActions = new List<PlayerSetupAction>(4);
        private readonly Dictionary<string, ScenarioComputerPlayer> computerPlayersByName = new Dictionary<string, ScenarioComputerPlayer>();
        private readonly List<IPlayer> players = new List<IPlayer>(4);
        private readonly List<BasePlayerTurn> turns = new List<BasePlayerTurn>();
        private GameRound currentGameRound = null;
        private int currentIndex = 0;
        private TurnToken currentToken;
        private BasePlayerTurn currentTurn = new GameSetupTurn();
        private Dictionary<GameEventTypes, Delegate> eventHandlersByGameEventType;
        private GameBoard gameBoard;
        private List<GameRound> gameRounds = new List<GameRound>();
        private LocalGameController localGameController = null;
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

        public LocalGameControllerScenarioRunner Build(Dictionary<GameEventTypes, Delegate> eventHandlersByGameEventType = null)
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
            this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { this.currentToken = t; this.StartOfTurn(); };
            this.localGameController.StartOpponentTurnEvent = (Guid g) => { this.StartOfTurn(); };

            this.eventHandlersByGameEventType = eventHandlersByGameEventType;

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

        public IPlayer GetPlayerFromName(string playerName)
        {
            return this.playersByName[playerName];
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

            for (var index = 0; index < this.turns.Count; index += 4)
            {
                if (index == 0)
                    this.localGameController.StartGamePlay();

                var workingIndex = index;
                var endIndex = index + 3;
                while (workingIndex <= endIndex && workingIndex < this.turns.Count)
                {
                    var playerTurn = this.turns[workingIndex];
                    var runnerActions = playerTurn.GetRunnerActions();
                    if (runnerActions != null && runnerActions.Count > 0)
                    {
                        foreach (var runnerAction in runnerActions)
                            this.AddDevelopmentCardToBuy(((InsertDevelopmentCardAction)runnerAction).DevelopmentCardType);
                    }

                    playerTurn.ResolveActions(this.currentToken, this.localGameController);
                    workingIndex++;
                }

                this.localGameController.EndTurn(this.currentToken);
            }

            return this.localGameController;
        }

        internal IPlayer GetPlayer(Guid playerId)
        {
            return this.players.Where(p => p.Id == playerId).First();
        }

        public BasePlayerTurn PlayerTurn(string playerName, uint dice1, uint dice2)
        {
            this.NumberGenerator.AddTwoDiceRoll(dice1, dice2);

            if (this.currentGameRound == null || this.currentGameRound.IsComplete)
            {
                this.currentGameRound = new GameRound();
                this.gameRounds.Add(this.currentGameRound);
            }

            BasePlayerTurn playerTurn;
            var player = this.playersByName[playerName];
            if (player.IsComputer)
                playerTurn = new ComputerPlayerTurn(player, dice1, dice2, this);
            else
                playerTurn = new HumanPlayerTurn(player, dice1, dice2, this);

            this.currentGameRound.Add(playerTurn);

            return playerTurn;
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

        public LocalGameControllerScenarioRunner WithStartingResourcesForPlayer(string playerName, ResourceClutch playerResources)
        {
            var player = this.playersByName[playerName];
            player.AddResources(playerResources);
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
                : new ScenarioPlayer(name) as IPlayer;

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
                previousTurn.VerifyState(this.playersByName);
            }

            this.currentIndex++;
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
        #endregion
    }

    internal abstract class RunnerAction { }

    internal class InsertDevelopmentCardAction : RunnerAction
    {
        public DevelopmentCardTypes DevelopmentCardType;
    }
}
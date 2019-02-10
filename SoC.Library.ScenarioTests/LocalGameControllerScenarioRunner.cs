using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.DevelopmentCards;
using Jabberwocky.SoC.Library.GameBoards;
using Jabberwocky.SoC.Library.GameEvents;
using Jabberwocky.SoC.Library.Interfaces;
using NUnit.Framework;
using SoC.Library.ScenarioTests.PlayerTurn;
using SoC.Library.ScenarioTests.ScenarioEvents;

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
        //public readonly Dictionary<string, IPlayer> PlayersByName = new Dictionary<string, IPlayer>();
        private readonly Queue<PlaceInfrastructureAction> playerSetupActions = new Queue<PlaceInfrastructureAction>();
        private readonly ScenarioPlayerPool playerPool = new ScenarioPlayerPool();
        private readonly List<BasePlayerTurn> playerTurns = new List<BasePlayerTurn>();
        private readonly List<PlayerSetupAction> secondRoundSetupActions = new List<PlayerSetupAction>(4);
        private readonly Dictionary<string, ScenarioComputerPlayer> computerPlayersByName = new Dictionary<string, ScenarioComputerPlayer>();
        private readonly List<IPlayer> players = new List<IPlayer>(4);
        private readonly List<BasePlayerTurn> turns = new List<BasePlayerTurn>();
        private int currentIndex = 0;
        private TurnToken currentToken;
        private BasePlayerTurn currentTurn;
        private Dictionary<GameEventTypes, Delegate> eventHandlersByGameEventType;
        private GameBoard gameBoard;
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

            foreach (var turn in this.turns)
            {
                turn.Initialise(this.localGameController);
                if (turn.DevelopmentCardTypes != null)
                {
                    foreach (var developmentCardType in turn.DevelopmentCardTypes)
                        this.developmentCardHolder.AddDevelopmentCard(this.CreateDevelopmentCardToBuy(developmentCardType));
                }

                if (turn.ScenarioSelectResourceFromPlayerActions != null)
                {
                    foreach (var s in turn.ScenarioSelectResourceFromPlayerActions)
                    {
                        var selectedPlayer = this.PlayersByName[s.SelectedPlayerName];
                        this.NumberGenerator.AddRandomNumber(this.GetResourceSelectionForPlayer(selectedPlayer, s.ExpectedSingleResource));
                    }
                }
            }

            this.localGameController.CityBuiltEvent = cityBuiltEvent => this.currentTurn?.AddEvent(cityBuiltEvent);
            this.localGameController.DiceRollEvent = (Guid playerId, uint dice1, uint dice2) =>
            {
                this.currentTurn?.AddEvent(new DiceRollEvent(playerId, dice1, dice2));
            };
            //this.localGameController.DevelopmentCardPurchasedEvent = (DevelopmentCard c) => { this.currentTurn?.AddEvent(new BuyDevelopmentCardEvent(this.players[0].Id)); };
            this.localGameController.DevelopmentCardBoughtEvent = gameEvent => { this.currentTurn?.AddEvent(gameEvent); };
            this.localGameController.ErrorRaisedEvent = (ErrorDetails e) =>
            {
                if (this.currentTurn.TreatErrorsAsEvents)
                    this.currentTurn.AddEvent(new ScenarioErrorMessageEvent(e.Message));
                else
                    Assert.Fail(e.Message);
            };
            this.localGameController.GameEvents = this.GameEventsHandler;
            this.localGameController.LargestArmyEvent = (newPlayerId, previousPlayerId) => { this.currentTurn?.AddEvent(new LargestArmyChangedEvent(newPlayerId, previousPlayerId)); };
            //this.localGameController.PlayKnightCardEvent = (PlayKnightCardEvent p) => { this.currentTurn?.AddEvent(p); };
            this.localGameController.KnightCardPlayedEvent = e => { this.currentTurn?.AddEvent(e); };
            this.localGameController.ResourcesLostEvent = (r) => this.currentTurn?.AddEvent(r);
            this.localGameController.ResourcesTransferredEvent = (ResourceTransactionList list) =>
            {
                this.currentTurn?.AddEvent(new ResourceTransactionEvent(this.players[0].Id, list));
            };
            this.localGameController.RoadSegmentBuiltEvent = (RoadSegmentBuiltEvent r) => this.currentTurn?.AddEvent(r);
            this.localGameController.RobberEvent = (int r) =>
            {
                this.currentTurn.AddEvent(new ScenarioRobberEvent(r));
            };
            this.localGameController.RobbingChoicesEvent = robbingChoices =>
            {
                this.currentTurn.AddEvent(new ScenarioRobbingChoicesEvent(robbingChoices));
            };
            this.localGameController.SettlementBuiltEvent = settlementBuiltEvent => this.currentTurn?.AddEvent(settlementBuiltEvent);
            this.localGameController.StartPlayerTurnEvent = (TurnToken t) => { this.currentToken = t; this.StartOfTurn(); };
            this.localGameController.StartOpponentTurnEvent = (Guid g) => { this.StartOfTurn(); };

            this.eventHandlersByGameEventType = eventHandlersByGameEventType;

            // Ensure that there is enough dice rolls
            var fillDiceRollCount = (4 - (this.turns.Count % 4)) + 1;
            while (fillDiceRollCount > 0)
            {
                this.NumberGenerator.AddTwoDiceRoll(1, 1);
                fillDiceRollCount--;
            }

            return this;
        }

        public IPlayer GetPlayerFromName(string playerName)
        {
            return this.PlayersByName[playerName];
        }

        public LocalGameController Run()
        {
            this.localGameController.JoinGame(this.gameOptions);

            PlaceInfrastructureAction[] placeInfrastructureActionsForHumanPlayer = null;
            foreach(var kv in this.setupActionsByPlayerName)
            {
                var player = this.playerPool.PlayersByName[kv.Key];
                if (player is ScenarioComputerPlayer computerPlayer)
                {
                    foreach (var action in kv.Value)
                        computerPlayer.AddSetupInstructions(action.SettlementLocation, action.RoadEndLocation);
                }
                else
                {
                    // TODO: Else branch is weak and assumes there is only one human player
                    placeInfrastructureActionsForHumanPlayer = kv.Value;
                }
            }

            this.localGameController.LaunchGame();
            this.localGameController.StartGameSetup();

            this.localGameController.ContinueGameSetup(
                placeInfrastructureActionsForHumanPlayer[0].SettlementLocation,
                placeInfrastructureActionsForHumanPlayer[0].RoadEndLocation);

            this.localGameController.CompleteGameSetup(
                placeInfrastructureActionsForHumanPlayer[1].SettlementLocation,
                placeInfrastructureActionsForHumanPlayer[1].RoadEndLocation);

            this.ProcessGame();

            return this.localGameController;
        }

        private void ProcessGame()
        {
            try
            {
                this.localGameController.StartGamePlay();

                var index = 0;
                BasePlayerTurn turn = this.turns[index++];
                do
                {
                    Thread.Sleep(50);
                    if (turn.IsFinished)
                    {
                        turn.FinishProcessing();
                        if (index < this.turns.Count)
                            turn = this.turns[index++];
                        else
                            break;
                    }
                    else if (!turn.IsVerified || turn.HasInstructions)
                    {
                        turn.Process(this.PlayersByName);
                    }

                } while (true);
            }
            finally {
                this.localGameController.Quit();
            }

            /*for (var index = 0; index < this.turns.Count; index++)
            {
                this.turns[index].CompleteProcessing(this.currentToken, this.localGameController);
                if (this.turns[index].IsHumanPlayer)
                    this.localGameController.EndTurn(this.currentToken);
            }*/

            /*for (var index = 0; index < this.turns.Count; index += 4)
            {
                var workingIndex = index;
                var endIndex = index + 3;
                /*while (workingIndex <= endIndex && workingIndex < this.turns.Count)
                {
                    var playerTurn = this.turns[workingIndex];
                    playerTurn.ResolveResponses(this.localGameController);
                    workingIndex++;
                }

                if (index == 0)
                    this.localGameController.StartGamePlay();
                //workingIndex = index;
                while (workingIndex <= endIndex && workingIndex < this.turns.Count)
                {
                    var playerTurn = this.turns[workingIndex];
                    if (playerTurn.RunnerActions != null && playerTurn.RunnerActions.Count > 0)
                    {
                        foreach (var runnerAction in playerTurn.RunnerActions)
                            this.AddDevelopmentCardToBuy(((InsertDevelopmentCardAction)runnerAction).DevelopmentCardType);
                    }

                    playerTurn.ResolveActions(this.currentToken, this.localGameController);
                    workingIndex++;
                }

                this.localGameController.EndTurn(this.currentToken);
            }*/
        }

        internal IPlayer GetPlayer(Guid playerId)
        {
            return this.players.Where(p => p.Id == playerId).First();
        }

        private int roundNumber = 1;
        private int turnNumber = 1;
        public BasePlayerTurn PlayerTurn_Old(string playerName, uint dice1, uint dice2)
        {
            this.NumberGenerator.AddTwoDiceRoll(dice1, dice2);

            BasePlayerTurn playerTurn;
            var player = this.PlayersByName[playerName];
            if (player.IsComputer)
                playerTurn = new ComputerPlayerTurn(player, this, this.roundNumber, this.turnNumber);
            else
                playerTurn = new HumanPlayerTurn(player, this, this.roundNumber, this.turnNumber);

            this.turns.Add(playerTurn);

            this.turnNumber++;
            if (this.turnNumber > 4)
            {
                this.roundNumber++;
                this.turnNumber = 1;
            }

            return playerTurn;
        }

        private GameOptions gameOptions = new GameOptions();
        public LocalGameControllerScenarioRunner WithComputerPlayer(string name)
        {
            this.initialPlayerOrder.Add(name);
            this.playerPool.AddPlayer(name);
            return this;
        }

        public LocalGameControllerScenarioRunner WithMainPlayer(string name)
        {
            this.initialPlayerOrder.Add(name);
            this.playerPool.AddPlayer(name);
            return this;
        }

        public LocalGameControllerScenarioRunner WithNoResourceCollection()
        {
            this.gameBoard = new ScenarioGameBoardWithNoResourcesCollected();
            return this;
        }

        private Dictionary<string, IPlayer> PlayersByName { get { return this.playerPool.PlayersByName; } }
        private Dictionary<string, PlaceInfrastructureAction[]> setupActionsByPlayerName = new Dictionary<string, PlaceInfrastructureAction[]>();
        public LocalGameControllerScenarioRunner WithPlayerSetup(string playerName, uint firstSettlementLocation, uint firstRoadEndLocation, uint secondSettlementLocation, uint secondRoadEndLocation)
        {
            this.setupActionsByPlayerName.Add(playerName, new PlaceInfrastructureAction[]
            {
                new PlaceInfrastructureAction(firstSettlementLocation, firstRoadEndLocation),
                new PlaceInfrastructureAction(secondSettlementLocation, secondRoadEndLocation)
            });

            return this;
        }

        private List<string> initialPlayerOrder = new List<string>();
        public LocalGameControllerScenarioRunner WithTurnOrder(string firstPlayerName, string secondPlayerName, string thirdPlayerName, string fourthPlayerName)
        {
            var rolls = new uint[4];
            for (var index = 0; index < this.initialPlayerOrder.Count; index++)
            {
                var playerName = this.initialPlayerOrder[index];
                if (firstPlayerName == playerName)
                    rolls[index] = 12;
                else if (secondPlayerName == playerName)
                    rolls[index] = 10;
                else if (thirdPlayerName == playerName)
                    rolls[index] = 8;
                else
                    rolls[index] = 6;
            }

            foreach (var roll in rolls)
                this.NumberGenerator.AddTwoDiceRoll(roll / 2, roll / 2);

            return this;
        }

        private Dictionary<string, ResourceClutch> startingResourcesByName = new Dictionary<string, ResourceClutch>();
        public LocalGameControllerScenarioRunner WithStartingResourcesForPlayer(string playerName, ResourceClutch playerResources)
        {
            this.startingResourcesByName.Add(playerName, playerResources);
            return this;
        }

        internal DevelopmentCard CreateDevelopmentCardToBuy(DevelopmentCardTypes developmentCardType)
        {
            DevelopmentCard developmentCard = null;
            switch (developmentCardType)
            {
                case DevelopmentCardTypes.Knight: developmentCard = new KnightDevelopmentCard(); break;
                default: throw new Exception($"Development card type {developmentCardType} not recognised");
            }

            return developmentCard;
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

        private void DiceRollEventHandler(Guid playerId, uint dice1, uint dice2)
        {
            this.currentTurn.AddEvent(new DiceRollEvent(playerId, dice1, dice2));
        }

        private BasePlayerTurn previousTurn;
        private void StartOfTurn()
        {
            if (this.currentIndex > 0 && this.currentTurn != null)
                this.currentTurn.IsFinished = true;

            if (this.currentIndex < this.turns.Count)
            {
                this.currentTurn = this.turns[this.currentIndex++];
                this.currentTurn.TurnToken = this.currentToken;
            }
            else
            {
                this.currentTurn = null;
            }
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

        private int GetResourceSelectionForPlayer(IPlayer selectedPlayer, ResourceTypes expectedSingleResource)
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

            return randomNumber;
        }

        internal BasePlayerTurn PlayerTurn(string playerName, uint dice1, uint dice2)
        {
            this.NumberGenerator.AddTwoDiceRoll(dice1, dice2);

            var playerTurn = new BasePlayerTurn(playerName, this, this.roundNumber, this.turnNumber);
            this.turns.Add(playerTurn);

            this.turnNumber++;
            if (this.turnNumber > 4)
            {
                this.roundNumber++;
                this.turnNumber = 1;
            }

            return playerTurn;
        }

        
        #endregion
    }

    internal abstract class RunnerAction { }

    internal class InsertDevelopmentCardAction : RunnerAction
    {
        public DevelopmentCardTypes DevelopmentCardType;
    }

    public interface IActionProcessor
    {
        void Add(Jabberwocky.SoC.Library.GameActions.ComputerPlayerAction action);
    }
}
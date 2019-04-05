
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using SoC.Library.ScenarioTests.Instructions;

    internal class ScenarioRunner
    {
        #region Fields
        private readonly ScenarioDevelopmentCardHolder developmentCardHolder = new ScenarioDevelopmentCardHolder();
        private readonly List<PlayerAgent> playerAgents = new List<PlayerAgent>();
        private readonly Dictionary<string, Guid> playerIdsByName = new Dictionary<string, Guid>();
        private readonly Dictionary<string, ResourceClutch> startingResourcesByName = new Dictionary<string, ResourceClutch>();
        private GameBoard gameBoard;
        private List<Instruction> instructions = new List<Instruction>();
        private ScenarioNumberGenerator numberGenerator;
        private readonly bool requestStateActionsMustHaveToken = true;
        private readonly bool useServerTimer = true;
        #endregion

        #region Construction
        private ScenarioRunner(string[] args)
        {
            if (args != null)
            {
                if (args.Contains("NoTimer"))
                    this.useServerTimer = false;
                if (args.Contains("NoTokenRequiredForRequestState"))
                    this.requestStateActionsMustHaveToken = false;
            }

            this.numberGenerator = new ScenarioNumberGenerator();
        }
        #endregion

        #region Properties
        private Instruction LastInstruction
        {
            get { return this.instructions[this.instructions.Count - 1]; }
        }
        #endregion

        #region Methods
        public static ScenarioRunner CreateScenarioRunner(string[] args = null)
        {
            return new ScenarioRunner(args);
        }

        public ScenarioRunner AnswerDirectTradeOffer(ResourceClutch wantedResources)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.AnswerDirectTradeOffer, 
                new object[] { wantedResources });
            return this;
        }

        public ScenarioRunner AcceptTrade(string sellerName)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.AcceptTrade,
                new object[] { sellerName });
            return this;
        }

        public ScenarioRunner EndTurn()
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.EndOfTurn, null);
            return this;
        }

        public ScenarioRunner Label(string playerName, string label)
        {
            this.instructions.Add(new LabelInstruction(playerName, label));
            return this;
        }
        
        public ScenarioRunner MakeDirectTradeOffer(ResourceClutch wantedResources)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.MakeDirectTradeOffer, 
                new object[] { wantedResources });
            return this;
        }

        public ScenarioRunner PlaceStartingInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.PlaceStartingInfrastructure,
                new object[] { settlementLocation, roadEndLocation });
            return this;
        }

        public void Run()
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "Scenario Runner";

            var playerIds = new Queue<Guid>(this.playerAgents.Select(agent => agent.Id));

            var playerAgentsByName = this.playerAgents.ToDictionary(playerAgent => playerAgent.Name, playerAgent => playerAgent);
            this.instructions.ForEach(instruction => playerAgentsByName[instruction.PlayerName].AddInstruction(instruction));

            if (this.gameBoard == null)
                this.gameBoard = new GameBoard(BoardSizes.Standard);

            var gameServer = new LocalGameServer(
                this.numberGenerator,
                this.gameBoard,
                this.developmentCardHolder
            );

            if (!this.useServerTimer)
                gameServer.SetTurnTimer(new MockTurnTimer());

            gameServer.SetIdGenerator(() => { return playerIds.Dequeue(); });

            gameServer.SetRequestStateExemption(this.requestStateActionsMustHaveToken);

            gameServer.LaunchGame();

            this.playerAgents.ForEach(playerAgent =>
            {
                playerAgent.JoinGame(gameServer);
                playerAgent.StartAsync();
            });

            foreach (var kv in this.startingResourcesByName)
                gameServer.AddResourcesToPlayer(kv.Key, kv.Value);

            gameServer.StartGameAsync();

            var tickCount = 200;
            var playerAgentFaulted = false;
            while (tickCount > 0)
            {
                Thread.Sleep(100);
                tickCount--;
                if (this.playerAgents.All(p => p.IsFinished) ||
                    (playerAgentFaulted = this.playerAgents.Any(p => p.GameException != null)))
                    break;
            }

            this.QuitGame(gameServer);

            gameServer.SaveLog(@"GameServer.log");
            this.playerAgents.ForEach(playerAgent => {
                playerAgent.SaveEvents($"{playerAgent.Name}.events");
                playerAgent.SaveLog($"{playerAgent.Name}.log");
            });

            if (playerAgentFaulted)
            {
                string message = null;
                foreach (var playerAgent in this.playerAgents.Where(p => p.GameException != null))
                {
                    var exception = playerAgent.GameException;
                    while (exception.InnerException != null)
                        exception = exception.InnerException;

                    message += $"{playerAgent.Name}: {exception.Message}\r\n";
                }

                throw new Exception(message);
            }

            string timeOutMessage = string.Join("\r\n",
                this.playerAgents
                    .Where(playerAgent => !playerAgent.IsFinished)
                    .Select(playerAgent => {
                        var message = $"{playerAgent.Name} did not finish.";
                        playerAgent.GetEventResults().ForEach(tuple => {
                            message += $"\t{tuple.Item1} - {tuple.Item2}\r\n";
                        });
                        return message;
                    })
                );


            if (!string.IsNullOrEmpty(timeOutMessage))
                throw new TimeoutException(timeOutMessage);
        }

        internal ScenarioRunner WhenResourceCollectionEvent(string playerName)
        {
            throw new NotImplementedException();
        }

        private void QuitGame(LocalGameServer gameServer)
        {
            gameServer.Quit();

            while (!gameServer.IsFinished)
                Thread.Sleep(50);
        }

        public ScenarioRunner VerboseLogging()
        {
            ((EventInstruction)this.LastInstruction).Verbose = true;
            return this;
        }

        public PlayerStateInstruction State(string playerName)
        {
            var playerState = new PlayerStateInstruction(playerName, this);
            this.instructions.Add(playerState);
            return playerState;
        }

        public ScenarioRunner WhenAcceptDirectTradeEvent(string playerName, string buyerName, ResourceClutch buyingResources, string sellerName, ResourceClutch sellingResources)
        {
            var gameEvent = new AcceptTradeEvent(this.playerIdsByName[buyerName], buyingResources, this.playerIdsByName[sellerName], sellingResources);
            var eventInstruction = new AcceptDirectTradeEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenAnswerDirectTradeOfferEvent(string playerName, string buyingPlayerName, ResourceClutch wantedResources)
        {
            var gameEvent = new AnswerDirectTradeOfferEvent(this.playerIdsByName[buyingPlayerName], wantedResources);
            var eventInstruction = new AnswerDirectTradeOfferEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenDiceRollEvent(string playerName, uint dice1, uint dice2)
        {
            this.numberGenerator.AddTwoDiceRoll(dice1, dice2);
            var gameEvent = new DiceRollEvent(this.playerIdsByName[playerName], dice1, dice2);
            var eventInstruction = new DiceRollEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenGameJoinedEvent(string playerName)
        {
            var gameEvent = new GameJoinedEvent(this.playerIdsByName[playerName]);
            var eventInstruction = new GameJoinedEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenInitialBoardSetupEvent(string playerName, GameBoardSetup gameBoardSetup)
        {
            var gameEvent = new InitialBoardSetupEvent(gameBoardSetup);
            var eventInstruction = new InitialBoardSetupEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenMakeDirectTradeOfferEvent(string playerName, string buyingPlayerName, ResourceClutch wantedResources)
        {
            var gameEvent = new MakeDirectTradeOfferEvent(this.playerIdsByName[buyingPlayerName], wantedResources);
            var eventInstruction = new MakeDirectTradeOfferEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenPlaceInfrastructureSetupEvent(string playerName)
        {
            var gameEvent = new PlaceSetupInfrastructureEvent();
            var eventInstruction = new PlaceSetupInfrastructureEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenPlayerOrderEvent(string playerName, string[] playerNames)
        {
            var playerIds = this.playerAgents.Select(playerAgent => playerAgent.Id).ToArray();
            var gameEvent = new PlayerOrderEvent(playerIds);
            var eventInstruction = new PlayerOrderEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenPlayerSetupEvent(string playerName)
        {
            var playerIdsByName = this.playerAgents.ToDictionary(playerAgent => playerAgent.Name, playerAgent => playerAgent.Id);
            var gameEvent = new PlayerSetupEvent(playerIdsByName);
            var eventInstruction = new PlayerSetupEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WithNoResourceCollection()
        {
            this.gameBoard = new ScenarioGameBoardWithNoResourcesCollected();
            return this;
        }

        public ScenarioRunner WithPlayer(string playerName, bool verboseLogging = false)
        {
            var playerAgent = new PlayerAgent(playerName, verboseLogging);
            this.playerAgents.Add(playerAgent);
            this.playerIdsByName.Add(playerAgent.Name, playerAgent.Id);
            return this;
        }

        public ScenarioRunner WithStartingResourcesForPlayer(string playerName, ResourceClutch playerResources)
        {
            this.startingResourcesByName.Add(playerName, playerResources);
            return this;
        }

        public ScenarioRunner WithTurnOrder(string[] playerNames)
        {
            var rolls = new uint[4];
            for (var index = 0; index < this.playerAgents.Count; index++)
            {
                var playerName = this.playerAgents[index].Name;
                if (playerNames[0] == playerName)
                    rolls[index] = 12;
                else if (playerNames[1] == playerName)
                    rolls[index] = 10;
                else if (playerNames[3] == playerName)
                    rolls[index] = 8;
                else
                    rolls[index] = 6;
            }

            foreach (var roll in rolls)
                this.numberGenerator.AddTwoDiceRoll(roll / 2, roll / 2);

            return this;
        }

        private void AddActionInstruction(ActionInstruction.OperationTypes operation, object[] arguments)
        {
            var actionInstruction = new ActionInstruction(
                this.LastInstruction.PlayerName,
                operation,
                arguments);
            this.instructions.Add(actionInstruction);
        }
        #endregion
    }
}

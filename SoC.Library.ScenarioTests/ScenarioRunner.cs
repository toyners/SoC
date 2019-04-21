
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using SoC.Library.ScenarioTests.Instructions;

    internal class ScenarioRunner
    {
        #region Fields
        private readonly ScenarioDevelopmentCardHolder developmentCardHolder = new ScenarioDevelopmentCardHolder();
        private readonly List<PlayerAgent> playerAgents = new List<PlayerAgent>();
        private readonly Dictionary<string, PlayerAgent> playerAgentsByName = new Dictionary<string, PlayerAgent>();
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

        public ScenarioRunner ThenConfirmGameStart()
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.ConfirmStart, null);
            return this;
        }

        public ScenarioRunner EndTurn(string playerName)
        {
            this.AddActionInstruction(playerName, ActionInstruction.OperationTypes.EndOfTurn, null);
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

        public ScenarioRunner ThenPlaceStartingInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.PlaceStartingInfrastructure,
                new object[] { settlementLocation, roadEndLocation });
            return this;
        }

        public ScenarioRunner ThenQuitGame()
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.QuitGame, null);
            return this;
        }

        public ScenarioRunner ThenDoNothing()
        {
            return this;
        }

        public void Run()
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "Scenario Runner";

            //this.instructions.ForEach(instruction => this.playerAgentsByName[instruction.PlayerName].AddInstruction(instruction));

            if (this.gameBoard == null)
                this.gameBoard = new GameBoard(BoardSizes.Standard);

            var gameServer = new LocalGameServer(
                this.numberGenerator,
                this.gameBoard,
                this.developmentCardHolder
            );

            if (!this.useServerTimer)
                gameServer.SetTurnTimer(new MockTurnTimer());

            var playerIds = new Queue<Guid>(this.playerAgents.Select(agent => agent.Id));
            gameServer.SetIdGenerator(() => { return playerIds.Dequeue(); });

            gameServer.SetRequestStateExemption(this.requestStateActionsMustHaveToken);

            gameServer.LaunchGame();

            var tasks = new List<Task>();
            this.playerAgents.ForEach(playerAgent =>
            {
                playerAgent.JoinGame(gameServer);
                tasks.Add(playerAgent.StartAsync());
            });

            foreach (var kv in this.startingResourcesByName)
                gameServer.AddResourcesToPlayer(kv.Key, kv.Value);

            Task gameServerTask = gameServer.StartGameAsync();
            tasks.Add(gameServerTask);

            Task.WaitAll(tasks.ToArray(), 20000);

            if (!gameServerTask.IsCompleted)
                this.QuitGame(gameServer);

            gameServer.SaveLog(@"GameServer.log");
            this.playerAgents.ForEach(playerAgent => {
                playerAgent.SaveEvents($"{playerAgent.Name}.events");
                playerAgent.SaveLog($"{playerAgent.Name}.log");
            });

            if (gameServerTask.IsFaulted)
            {
                var exception = gameServerTask.Exception;
                var flattenedException = exception.Flatten();
                throw new Exception($"Game server: {flattenedException.InnerException.Message}");
            }

            string message = string.Join("\r\n",
                tasks
                    .Where(task => task.IsFaulted)
                    .Select(playerAgentTask => {
                        var exception = playerAgentTask.Exception;
                        var flattenedException = exception.Flatten();
                        var playerAgent = (PlayerAgent)playerAgentTask.AsyncState;
                        return $"{playerAgent.Name}: {flattenedException.InnerException.Message}";
                    })
                );
            
            if (!string.IsNullOrEmpty(message))
                throw new Exception(message);

            string timeOutMessage = string.Join("\r\n",
                this.playerAgents
                    .Where(playerAgent => !playerAgent.IsFinished)
                    .Select(playerAgent => {
                        var playerAgentMessage = $"{playerAgent.Name} did not finish.\r\n";
                        playerAgent.GetEventResults().ForEach(tuple => {
                            playerAgentMessage += $"\t{tuple.Item1} => {tuple.Item3}\r\n";
                        });
                        return playerAgentMessage;
                    })
                );

            if (!string.IsNullOrEmpty(timeOutMessage))
                throw new TimeoutException(timeOutMessage);
        }

        public PlayerStateInstruction State(string playerName)
        {
            var playerAgent = this.playerAgents.First(p => p.Name == playerName);
            var playerState = new PlayerStateInstruction(playerAgent, this);
            this.instructions.Add(playerState);
            return playerState;
        }

        public ScenarioRunner VerboseLogging()
        {
            ((EventInstruction)this.LastInstruction).Verbose = true;
            return this;
        }

        public ScenarioRunner VerifyInfrastructurePlacedEventForAllPlayers(string playerName, uint settlementLocation, uint roadEndLocation)
        {
            this.playerAgents.ForEach(playerAgent =>
            {
                var gameEvent = new InfrastructurePlacedEvent(this.GetPlayerId(playerName), settlementLocation, roadEndLocation);
                this.instructions.Add(new EventInstruction(playerAgent.Name, gameEvent));
            });
            
            return this;
        }

        public ScenarioRunner WhenAcceptDirectTradeEvent(string playerName, string buyerName, ResourceClutch buyingResources, string sellerName, ResourceClutch sellingResources)
        {
            var gameEvent = new AcceptTradeEvent(this.GetPlayerId(buyerName), buyingResources, this.GetPlayerId(sellerName), sellingResources);
            var eventInstruction = new AcceptDirectTradeEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenAnswerDirectTradeOfferEvent(string playerName, string buyingPlayerName, ResourceClutch wantedResources)
        {
            var gameEvent = new AnswerDirectTradeOfferEvent(this.GetPlayerId(buyingPlayerName), wantedResources);
            var eventInstruction = new AnswerDirectTradeOfferEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesConfirmGameStartEvent()
        {
            var gameEvent = new ConfirmGameStartEvent();
            var eventInstruction = new EventInstruction(this.currentPlayerAgent.Name, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenDiceRollEvent(string playerName, uint dice1, uint dice2)
        {
            this.numberGenerator.AddTwoDiceRoll(dice1, dice2);
            var gameEvent = new DiceRollEvent(this.GetPlayerId(playerName), dice1, dice2);
            var eventInstruction = new DiceRollEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesGameJoinedEvent()
        {
            var gameEvent = new GameJoinedEvent(this.GetPlayerId(this.currentPlayerAgent.Name));
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesInitialBoardSetupEvent(GameBoardSetup gameBoardSetup)
        {
            var gameEvent = new InitialBoardSetupEvent(gameBoardSetup);
            var eventInstruction = new EventInstruction(this.currentPlayerAgent.Name, gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner WhenMakeDirectTradeOfferEvent(string playerName, string buyingPlayerName, ResourceClutch wantedResources)
        {
            var gameEvent = new MakeDirectTradeOfferEvent(this.GetPlayerId(buyingPlayerName), wantedResources);
            var eventInstruction = new MakeDirectTradeOfferEventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesPlaceInfrastructureSetupEvent()
        {
            var gameEvent = new PlaceSetupInfrastructureEvent();
            var eventInstruction = new EventInstruction(this.currentPlayerAgent.Name, gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesPlayerOrderEvent(string[] playerNames)
        {
            var playerIds = this.playerAgents.Select(playerAgent => playerAgent.Id).ToArray();
            var gameEvent = new PlayerOrderEvent(playerIds);
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesPlayerSetupEvent()
        {
            var playerIdsByName = this.playerAgents.ToDictionary(playerAgent => playerAgent.Name, playerAgent => playerAgent.Id);
            var gameEvent = new PlayerSetupEvent(playerIdsByName);
            var eventInstruction = new EventInstruction(this.currentPlayerAgent.Name, gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ThenEndTurn()
        {
            this.currentPlayerAgent.AddInstruction(this.CreateActionInstruction(
                ActionInstruction.OperationTypes.EndOfTurn, null));
            return this;
        }

        public ScenarioRunner ReceivesDiceRollEvent(uint dice1, uint dice2)
        {
            this.numberGenerator.AddTwoDiceRoll(dice1, dice2);
            var gameEvent = new DiceRollEvent(this.GetPlayerId(this.currentPlayerAgent.Name), dice1, dice2);
            var eventInstruction = new EventInstruction(this.currentPlayerAgent.Name, gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            //this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesPlayerWonEvent(string winningPlayerName)
        {
            throw new NotImplementedException();
        }

        private PlayerAgent currentPlayerAgent;
        public ScenarioRunner WhenPlayer(string playerName)
        {
            this.currentPlayerAgent = this.playerAgentsByName[playerName];
            return this;
        }

        public ScenarioRunner WithNoResourceCollection()
        {
            this.gameBoard = new ScenarioGameBoardWithNoResourcesCollected();
            return this;
        }

        private Guid GetPlayerId(string playerName)
        {
            if (!this.playerAgentsByName.ContainsKey(playerName))
                throw new Exception($"Player name {playerName} not recognised.");

            return this.playerAgentsByName[playerName].Id;
        }

        public ScenarioRunner WhenResourceCollectedEvent(string playerName, Dictionary<string, ResourceCollection[]> resourcesCollectedByPlayerName)
        {
            var resourcesCollectedByPlayerId = new Dictionary<Guid, ResourceCollection[]>();
            foreach (var kv in resourcesCollectedByPlayerName)
                resourcesCollectedByPlayerId.Add(this.GetPlayerId(kv.Key), kv.Value);
            var gameEvent = new ResourcesCollectedEvent(resourcesCollectedByPlayerId);
            var eventInstruction = new EventInstruction(playerName, gameEvent);
            this.instructions.Add(eventInstruction);
            return this;
        }

        public ScenarioRunner WithPlayer(string playerName, bool verboseLogging = false)
        {
            var playerAgent = new PlayerAgent(playerName, verboseLogging);
            this.playerAgents.Add(playerAgent);
            this.playerAgentsByName.Add(playerAgent.Name, playerAgent);
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
                else if (playerNames[2] == playerName)
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
            this.currentPlayerAgent.AddInstruction(this.CreateActionInstruction(operation, arguments));
        }

        private void AddActionInstruction(string playerName, ActionInstruction.OperationTypes operation, object[] arguments)
        {
            var actionInstruction = new ActionInstruction(
                playerName,
                operation,
                arguments);
            this.instructions.Add(actionInstruction);
        }

        private ActionInstruction CreateActionInstruction(ActionInstruction.OperationTypes operation, object[] arguments)
        {
            return new ActionInstruction(
                null,
                operation,
                arguments);
        }

        private void QuitGame(LocalGameServer gameServer)
        {
            gameServer.Quit();

            while (!gameServer.IsFinished)
                Thread.Sleep(50);
        }
        #endregion
    }

    internal class CollectedResourcesBuilder
    {
        private Dictionary<string, List<ResourceCollection>> resourceCollectionsByPlayerName = new Dictionary<string, List<ResourceCollection>>();

        public CollectedResourcesBuilder Add(string playerName, uint location, ResourceClutch resources)
        {
            if (!this.resourceCollectionsByPlayerName.TryGetValue(playerName, out var resourcesCollection))
            {
                resourcesCollection = new List<ResourceCollection>();   
                this.resourceCollectionsByPlayerName.Add(playerName, resourcesCollection);
            }

            resourcesCollection.Add(new ResourceCollection(location, resources));

            return this;
        }

        public Dictionary<string, ResourceCollection[]> Build()
        {
            var result = new Dictionary<string, ResourceCollection[]>();

            foreach(var kv in this.resourceCollectionsByPlayerName)
            {
                var resourceCollection = kv.Value.OrderBy(k => k.Location).ToArray();
                result.Add(kv.Key, resourceCollection);
            }

            return result;
        }
    }
}

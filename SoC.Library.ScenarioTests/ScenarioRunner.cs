
namespace SoC.Library.ScenarioTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;
    using SoC.Library.ScenarioTests.Instructions;
    using SoC.Library.ScenarioTests.ScenarioEvents;

    internal class ScenarioRunner
    {
        #region Fields
        private readonly ScenarioDevelopmentCardHolder developmentCardHolder = new ScenarioDevelopmentCardHolder();
        private readonly List<PlayerAgent> playerAgents = new List<PlayerAgent>();
        private readonly Dictionary<string, PlayerAgent> playerAgentsByName = new Dictionary<string, PlayerAgent>();
        private readonly Dictionary<string, ResourceClutch> startingResourcesByName = new Dictionary<string, ResourceClutch>();
        private readonly string scenarioName;
        private PlayerAgent currentPlayerAgent;
        private GameBoard gameBoard;
        private MultipleEventInstruction multipleEventInstruction;
        private ScenarioNumberGenerator numberGenerator;
        private IPlayerFactory playerFactory;
        #endregion

        #region Construction
        private ScenarioRunner(string[] args)
        {
            if (args != null)
            {
                this.scenarioName = args[0];
            }

            this.numberGenerator = new ScenarioNumberGenerator();
        }
        #endregion

        #region Methods
        public static ScenarioRunner CreateScenarioRunner(string[] args = null)
        {
            return new ScenarioRunner(args);
        }

        public ScenarioRunner DidNotReceiveEventOfTypeAfterCount<T>(int eventCount = 0) where T : GameEvent
        {
            this.currentPlayerAgent.AddDidNotReceiveEventOfType<T>(eventCount);
            return this;
        }

        public ScenarioRunner DidNotReceiveEventOfType<T>() where T : GameEvent
        {
            this.currentPlayerAgent.AddDidNotReceiveEventOfType<T>();
            return this;
        }

        public ScenarioRunner DidNotReceiveEvent(GameEvent gameEvent)
        {
            this.currentPlayerAgent.AddDidNotReceiveEvent(gameEvent);
            return this;
        }

        public ScenarioRunner DidNotReceivePlayerQuitEvent(string quittingPlayerName)
        {
            this.currentPlayerAgent.AddDidNotReceiveEvent(new PlayerQuitEvent(this.playerAgentsByName[quittingPlayerName].Id));
            return this;
        }

        public ScenarioRunner ReceivesAll()
        {
            if (this.multipleEventInstruction != null)
                throw new Exception("Previous multiple event instruction not closed");
            this.multipleEventInstruction = new MultipleEventInstruction();
            return this;
        }

        public ScenarioRunner ReceivesAllEnd()
        {
            this.currentPlayerAgent.AddInstruction(this.multipleEventInstruction);
            this.multipleEventInstruction = null;
            return this;
        }

        public ScenarioRunner ReceivesAcceptDirectTradeEvent(string buyerName, ResourceClutch buyingResources, string sellerName, ResourceClutch sellingResources)
        {
            var gameEvent = new AcceptTradeEvent(this.GetPlayerId(buyerName), buyingResources, this.GetPlayerId(sellerName), sellingResources);
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesAnswerDirectTradeOfferEvent(string buyingPlayerName, ResourceClutch wantedResources)
        {
            var gameEvent = new AnswerDirectTradeOfferEvent(this.GetPlayerId(buyingPlayerName), wantedResources);
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesChooseLostResourcesEvent(int numberOfResourcesToLose)
        {
            var gameEvent = new ChooseLostResourcesEvent(numberOfResourcesToLose);
            this.currentPlayerAgent.AddInstruction(new EventInstruction(gameEvent));
            return this;
        }

        public ScenarioRunner ReceivesCityPlacementEvent(uint cityLocation)
        {
            var gameEvent = new CityPlacedEvent(this.playerAgentsByName[this.currentPlayerAgent.Name].Id, cityLocation);
            this.currentPlayerAgent.AddInstruction(new EventInstruction(gameEvent));
            return this;
        }

        public ScenarioRunner ReceivesCityPlacementEvent(string playerName, uint cityLocation)
        {
            var gameEvent = new CityPlacedEvent(this.playerAgentsByName[playerName].Id, cityLocation);
            this.currentPlayerAgent.AddInstruction(new EventInstruction(gameEvent));
            return this;
        }

        public ScenarioRunner ReceivesConfirmGameStartEvent()
        {
            var gameEvent = new ConfirmGameStartEvent();
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesGameJoinedEvent()
        {
            var gameEvent = new GameJoinedEvent(this.GetPlayerId(this.currentPlayerAgent.Name));
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesGameErrorEvent(string errorCode, string errorMessage)
        {
            var gameEvent = new GameErrorEvent(this.GetPlayerId(this.currentPlayerAgent.Name), errorCode, errorMessage);
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesInitialBoardSetupEvent(GameBoardSetup gameBoardSetup)
        {
            var gameEvent = new InitialBoardSetupEvent(gameBoardSetup);
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesMakeDirectTradeOfferEvent(string buyingPlayerName, ResourceClutch wantedResources)
        {
            var gameEvent = new MakeDirectTradeOfferEvent(this.GetPlayerId(buyingPlayerName), wantedResources);
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesPlaceInfrastructureSetupEvent()
        {
            var gameEvent = new PlaceSetupInfrastructureEvent();
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesPlaceRobberEvent()
        {
            this.AddEventInstruction(new PlaceRobberEvent());
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

        public ScenarioRunner ReceivesPlayerQuitEvent(string quittingPlayerName)
        {
            this.currentPlayerAgent.AddInstruction(
                new EventInstruction(
                    new PlayerQuitEvent(this.playerAgentsByName[quittingPlayerName].Id)));
            return this;
        }

        public ScenarioRunner ReceivesPlayerWonEvent(uint victoryPoints)
        {
            this.currentPlayerAgent.AddInstruction(
                new EventInstruction(
                    new GameWinEvent(this.currentPlayerAgent.Id, victoryPoints)
                ));
            return this;
        }

        public ScenarioRunner ReceivesPlayerWonEvent(string winningPlayerName, uint victoryPoints)
        {
            this.currentPlayerAgent.AddInstruction(
                new EventInstruction(
                    new GameWinEvent(this.playerAgentsByName[winningPlayerName].Id, victoryPoints)
                ));
            return this;
        }

        public ScenarioRunner ReceivesPlayerSetupEvent()
        {
            var playerIdsByName = this.playerAgents.ToDictionary(playerAgent => playerAgent.Name, playerAgent => playerAgent.Id);
            var gameEvent = new PlayerSetupEvent(playerIdsByName);
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesResourceCollectedEvent(Dictionary<string, ResourceCollection[]> resourcesCollectedByPlayerName)
        {
            var resourcesCollectedByPlayerId = new Dictionary<Guid, ResourceCollection[]>();
            foreach (var kv in resourcesCollectedByPlayerName)
                resourcesCollectedByPlayerId.Add(this.GetPlayerId(kv.Key), kv.Value);
            var gameEvent = new ResourcesCollectedEvent(resourcesCollectedByPlayerId);
            var eventInstruction = new EventInstruction(gameEvent);
            this.currentPlayerAgent.AddInstruction(eventInstruction);
            return this;
        }

        public ScenarioRunner ReceivesResourcesStolenEvent(string playerName, ResourceClutch lostResource)
        {
            var gameEvent = new ResourcesLostEvent(lostResource, this.playerAgentsByName[playerName].Id, ResourcesLostEvent.ReasonTypes.Witness);
            this.AddEventInstruction(gameEvent);
            return this;
        }

        public ScenarioRunner ReceivesRoadSegmentPlacementEvent(uint roadSegmentStartLocation, uint roadSegmentEndLocation)
        {
            var gameEvent = new RoadSegmentPlacedEvent(this.playerAgentsByName[this.currentPlayerAgent.Name].Id, roadSegmentStartLocation, roadSegmentEndLocation);
            this.currentPlayerAgent.AddInstruction(new EventInstruction(gameEvent));
            return this;
        }

        public ScenarioRunner ReceivesRoadSegmentPlacementEvent(string playerName, uint roadSegmentStartLocation, uint roadSegmentEndLocation)
        {
            var gameEvent = new RoadSegmentPlacedEvent(this.playerAgentsByName[playerName].Id, roadSegmentStartLocation, roadSegmentEndLocation);
            this.currentPlayerAgent.AddInstruction(new EventInstruction(gameEvent));
            return this;
        }

        public ScenarioRunner ReceivesRobberPlacedEvent(uint hex)
        {
            this.AddEventInstruction(new RobberPlacedEvent(this.currentPlayerAgent.Id, hex));
            return this;
        }

        public ScenarioRunner ReceivesRobberPlacedEvent(string playerName, uint hex)
        {
            this.AddEventInstruction(new RobberPlacedEvent(this.playerAgentsByName[playerName].Id, hex));
            return this;
        }

        public ScenarioRunner ReceivesRobbingChoicesEvent(Dictionary<string, int> robbingChoices)
        {
            var resolvedRobbingChoices = robbingChoices.ToDictionary(e => this.playerAgentsByName[e.Key].Id, e => e.Value);
            this.AddEventInstruction(new RobbingChoicesEvent(this.currentPlayerAgent.Id, resolvedRobbingChoices));
            return this;
        }

        public ScenarioRunner ReceivesSettlementPlacementEvent(uint settlementLocation)
        {
            var gameEvent = new SettlementPlacedEvent(this.playerAgentsByName[this.currentPlayerAgent.Name].Id, settlementLocation);
            this.currentPlayerAgent.AddInstruction(new EventInstruction(gameEvent));
            return this;
        }

        public ScenarioRunner ReceivesStartTurnEvent(uint dice1, uint dice2)
        {
            this.numberGenerator.AddTwoDiceRoll(dice1, dice2);
            return this.ReceivesStartTurnWithResourcesCollectedEvent(this.currentPlayerAgent.Id, dice1, dice2, null);
        }

        public ScenarioRunner ReceivesStartTurnEvent(string playerName, uint dice1, uint dice2)
        {
            return this.ReceivesStartTurnWithResourcesCollectedEvent(this.playerAgentsByName[playerName].Id, dice1, dice2, null);
        }

        public ScenarioRunner ReceivesStartTurnWithResourcesCollectedEvent(uint dice1, uint dice2, Dictionary<string, ResourceCollection[]> collectedResources)
        {
            this.numberGenerator.AddTwoDiceRoll(dice1, dice2);
            return this.ReceivesStartTurnWithResourcesCollectedEvent(this.currentPlayerAgent.Id, dice1, dice2, collectedResources);
        }

        public ScenarioRunner ReceivesStartTurnWithResourcesCollectedEvent(string playerName, uint dice1, uint dice2, Dictionary<string, ResourceCollection[]> collectedResources)
        {
            return this.ReceivesStartTurnWithResourcesCollectedEvent(this.playerAgentsByName[playerName].Id, dice1, dice2, collectedResources);
        }

        private ScenarioRunner ReceivesStartTurnWithResourcesCollectedEvent(Guid playerId, uint dice1, uint dice2, Dictionary<string, ResourceCollection[]> collectedResources)
        {
            this.AddEventInstruction(new ScenarioStartTurnEvent(playerId, dice1, dice2, collectedResources));
            return this;
        }

        public ScenarioRunner ReceivesSettlementPlacementEvent(string playerName, uint settlementLocation)
        {
            var gameEvent = new SettlementPlacedEvent(this.playerAgentsByName[playerName].Id, settlementLocation);
            this.currentPlayerAgent.AddInstruction(new EventInstruction(gameEvent));
            return this;
        }

        public void Run()
        {
            if (Thread.CurrentThread.Name == null)
                Thread.CurrentThread.Name = "Scenario Runner";

            if (this.gameBoard == null)
                this.gameBoard = new GameBoard(BoardSizes.Standard);

            if (this.playerFactory == null)
                this.playerFactory = new PlayerPool();

            var gameServer = new LocalGameManager(
                this.numberGenerator,
                this.gameBoard,
                this.developmentCardHolder,
                this.playerFactory
            );

            gameServer.SetTurnTimer(new MockTurnTimer());

            var playerIds = new Queue<Guid>(this.playerAgents.Select(agent => agent.Id));
            gameServer.SetIdGenerator(() => { return playerIds.Dequeue(); });

            gameServer.LaunchGame();

            var playerAgentTasks = new List<Task>();
            this.playerAgents.ForEach(playerAgent =>
            {
                playerAgent.JoinGame(gameServer);
                playerAgentTasks.Add(playerAgent.StartAsync());
            });

            foreach (var kv in this.startingResourcesByName)
                gameServer.AddResourcesToPlayer(kv.Key, kv.Value);

            Task gameServerTask = gameServer.StartGameAsync();

            var tasks = new List<Task>(playerAgentTasks);
            tasks.Add(gameServerTask);
            try
            {
                int tickCount = 200;
                while (tickCount-- > 0)
                {
                    Thread.Sleep(50);
                    if (playerAgentTasks.Any(playerAgentTask => playerAgentTask.IsFaulted))
                        break;
                    if (playerAgentTasks.All(playerAgentTask => playerAgentTask.IsCompleted))
                        break;
                    if (gameServerTask.IsFaulted)
                        break;
                    if (this.playerAgents.All(playerAgent => playerAgent.IsFinished) && tickCount > 10)
                        tickCount = 10; // Provide a small window for unwanted events to be detected 
                }
            }
            catch
            {
                // Handle exceptions below
            }

            if (!gameServerTask.IsCompleted)
            {
                gameServer.Quit();
                while (!gameServerTask.IsCompleted)
                    Thread.Sleep(50);
            }

            var logDirectory = $"C:\\Scenario Logs\\{this.scenarioName}";
            if (Directory.Exists(logDirectory))
                Directory.Delete(logDirectory, true);
            Directory.CreateDirectory(logDirectory);

            for (var i = 0; i < this.playerAgents.Count; i++)
            {
                var playerAgentTask = playerAgentTasks[i];
                var playerAgent = this.playerAgents[i];
                if (!playerAgentTask.IsCompleted)
                {
                    this.playerAgents[i].Quit();
                    while (!playerAgentTask.IsCompleted)
                        Thread.Sleep(50);
                }

                playerAgent.SaveEvents($"{logDirectory}\\{playerAgent.Name}.events");
                playerAgent.SaveLog($"{logDirectory}\\{playerAgent.Name}.log");
            }

            gameServer.SaveLog($"{logDirectory}\\GameServer.log");

            if (gameServerTask.IsFaulted)
            {
                var exception = gameServerTask.Exception;
                var flattenedException = exception.Flatten();
                throw new Exception($"Game server: {flattenedException.InnerException.Message}");
            }

            string message = string.Join("\r\n",
                playerAgentTasks
                    .Where(playerAgentTask => playerAgentTask.IsFaulted)
                    .Select(playerAgentTask =>
                    {
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
                    .Select(playerAgent => $"\r\n{playerAgent.Name} did not finish.\r\n{playerAgent.GetEventLog()}")
                );

            if (!string.IsNullOrEmpty(timeOutMessage))
                throw new TimeoutException(timeOutMessage);
        }

        public ScenarioRunner ThenAnswerDirectTradeOffer(ResourceClutch wantedResources)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.AnswerDirectTradeOffer, 
                new object[] { wantedResources });
            return this;
        }

        public ScenarioRunner ThenAcceptTradeOffer(string sellerName)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.AcceptTrade,
                new object[] { sellerName });
            return this;
        }

        public ScenarioRunner ThenChooseResourcesToLose(ResourceClutch resourcesToLose)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.ChooseResourcesToLose,
                new object[] { resourcesToLose });
            return this;
        }

        public ScenarioRunner ThenConfirmGameStart()
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.ConfirmStart, null);
            return this;
        }

        public ScenarioRunner ThenDoNothing()
        {
            return this;
        }

        public ScenarioRunner ThenEndTurn()
        {
            this.currentPlayerAgent.AddInstruction(this.CreateActionInstruction(
                ActionInstruction.OperationTypes.EndOfTurn, null));
            return this;
        }

        public ScenarioRunner ThenMakeDirectTradeOffer(ResourceClutch wantedResources)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.MakeDirectTradeOffer, 
                new object[] { wantedResources });
            return this;
        }

        public ScenarioRunner ThenPlaceCity(uint cityLocation)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.PlaceCity,
                new object[] { cityLocation});
            return this;
        }

        public ScenarioRunner ThenPlaceRoadSegment(uint startLocation, uint endLocation)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.PlaceRoadSegment,
                new object[] { startLocation, endLocation });
            return this;
        }

        public ScenarioRunner ThenPlaceRobber(uint hex)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.PlaceRobber,
                new object[] { hex });
            return this;
        }

        public ScenarioRunner ThenPlaceSettlement(uint settlementLocation)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.PlaceSettlement,
                new object[] { settlementLocation });
            return this;
        }

        public ScenarioRunner ThenPlaceStartingInfrastructure(uint settlementLocation, uint roadEndLocation)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.PlaceStartingInfrastructure,
                new object[] { settlementLocation, roadEndLocation });
            return this;
        }

        public ScenarioRunner ThenSelectRobbedPlayer(string victimPlayerName)
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.SelectResourceFromPlayer,
                new object[] { victimPlayerName });
            return this;
        }

        public ScenarioRunner VerifyAllPlayersReceivedGameWonEvent(string winningPlayerName, uint victoryPoints)
        {
            this.playerAgents.ForEach(playerAgent =>
            {
                var gameEvent = new GameWinEvent(this.GetPlayerId(winningPlayerName), victoryPoints);
                playerAgent.AddInstruction(new EventInstruction(gameEvent));
            });

            return this;
        }

        public ScenarioRunner ThenQuitGame()
        {
            this.AddActionInstruction(ActionInstruction.OperationTypes.QuitGame, null);
            return this;
        }

        public PlayerStateInstruction ThenVerifyPlayerState()
        {
            var playerState = new PlayerStateInstruction(this.currentPlayerAgent, this);
            return playerState;
        }

        public ScenarioRunner VerboseLogging()
        {
            this.currentPlayerAgent.SetVerboseLoggingOnVerificationOfPreviousEvent(true);
            return this;
        }

        public ScenarioRunner VerifyAllPlayersReceivedInfrastructurePlacedEvent(string playerName, uint settlementLocation, uint roadEndLocation)
        {
            this.playerAgents.ForEach(playerAgent =>
            {
                var gameEvent = new InfrastructurePlacedEvent(this.GetPlayerId(playerName), settlementLocation, roadEndLocation);
                playerAgent.AddInstruction(new EventInstruction(gameEvent));
            });

            return this;
        }

        public ScenarioRunner VerifyAllPlayersReceiveRoadSegmentPlacedEvent(string playerName, uint startLocation, uint endLocation)
        {
            this.playerAgents.ForEach(playerAgent =>
            {
                var gameEvent = new RoadSegmentPlacedEvent(this.GetPlayerId(playerName), startLocation, endLocation);
                playerAgent.AddInstruction(new EventInstruction(gameEvent));
            });

            return this;
        }

        public ScenarioRunner VerifyPlayer(string playerName)
        {
            this.WhenPlayer(playerName);
            this.currentPlayerAgent.FinishWhenAllEventsVerified = false;
            return this;
        }

        public ScenarioRunner WhenPlayer(string playerName)
        {
            this.currentPlayerAgent = this.playerAgentsByName[playerName];
            return this;
        }

        public ScenarioRunner WithInitialActionsFor(string playerName, params object[] actions)
        {
            throw new NotImplementedException();
        }

        public ScenarioRunner WithInitialPlayerSetupFor(string playerName, params IPlayerSetupAction[] playerSetupActions)
        {
            if (this.playerFactory == null)
                this.playerFactory = new ScenarioPlayerFactory();

            ((ScenarioPlayerFactory)this.playerFactory).AddPlayerSetup(playerName, playerSetupActions);

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
            this.playerAgentsByName.Add(playerAgent.Name, playerAgent);
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

        private void AddEventInstruction(GameEvent gameEvent)
        {
            if (this.multipleEventInstruction != null)
                this.multipleEventInstruction.Add(gameEvent);
            else
                this.currentPlayerAgent.AddInstruction(new EventInstruction(gameEvent));
        }

        private ActionInstruction CreateActionInstruction(ActionInstruction.OperationTypes operation, object[] arguments)
        {
            return new ActionInstruction(operation, arguments);
        }

        private Guid GetPlayerId(string playerName)
        {
            if (!this.playerAgentsByName.ContainsKey(playerName))
                throw new Exception($"Player name {playerName} not recognised.");

            return this.playerAgentsByName[playerName].Id;
        }

        public ScenarioRunner ReceivesResourcesRobbedEvent(string robbedPlayerName, ResourceClutch resource)
        {
            ResourceTypes resourceType;
            if (resource == ResourceClutch.OneBrick)
                resourceType = ResourceTypes.Brick;
            else if (resource == ResourceClutch.OneGrain)
                resourceType = ResourceTypes.Grain;
            else if (resource == ResourceClutch.OneLumber)
                resourceType = ResourceTypes.Lumber;
            else if (resource == ResourceClutch.OneOre)
                resourceType = ResourceTypes.Ore;
            else if (resource == ResourceClutch.OneWool)
                resourceType = ResourceTypes.Wool;
            else
                throw new Exception("Resources should be one-of");

            this.numberGenerator.AddRobbed(this.playerAgentsByName[robbedPlayerName], resourceType);
            var gameEvent = new ResourcesGainedEvent(resource);
            this.AddEventInstruction(gameEvent);

            return this;
        }

        public ScenarioRunner ReceivesResourcesStolenEvent(ResourceClutch resource)
        {
            var gameEvent = new ResourcesLostEvent(resource, this.currentPlayerAgent.Id, ResourcesLostEvent.ReasonTypes.Robbed);
            this.AddEventInstruction(gameEvent);
            return this;
        }
        #endregion
    }
}

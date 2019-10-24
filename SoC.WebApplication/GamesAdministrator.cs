
namespace SoC.WebApplication
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;
    using Jabberwocky.SoC.Library.PlayerActions;
    using Microsoft.AspNetCore.SignalR;
    using Newtonsoft.Json;
    using SoC.WebApplication.Hubs;
    using SoC.WebApplication.Requests;

    public class GamesAdministrator : IGamesAdministrator, IPlayerRequestReceiver
    {
        private readonly CancellationToken cancellationToken;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly IHubContext<GameHub> gameHubContext;
        private readonly ConcurrentDictionary<Guid, GameManagerToken> inPlayGamesById = new ConcurrentDictionary<Guid, GameManagerToken>();
        private List<GameManagerToken> inPlayGames = new List<GameManagerToken>();
        private readonly ConcurrentQueue<GameRequest> gameRequests = new ConcurrentQueue<GameRequest>();
        private readonly ConcurrentDictionary<Guid, GameSessionDetails> gamesToLaunchById = new ConcurrentDictionary<Guid, GameSessionDetails>();
        private readonly Task mainGameTask;
        private readonly INumberGenerator numberGenerator = new NumberGenerator();

        public GamesAdministrator(IHubContext<GameHub> gameHubContext)
        {
            this.gameHubContext = gameHubContext;
            this.cancellationToken = this.cancellationTokenSource.Token;
            this.mainGameTask = Task.Factory.StartNew(o => { this.ProcessInPlayGames(); }, null, this.cancellationToken);
        }

        public void AddGame(GameSessionDetails gameDetails)
        {
            this.gamesToLaunchById.TryAdd(gameDetails.Id, gameDetails);
        }

        public void ConfirmGameJoin(ConfirmGameJoinRequest confirmGameJoinRequest)
        {
            var gameId = Guid.Parse(confirmGameJoinRequest.GameId);
            if (this.gamesToLaunchById.TryGetValue(gameId, out var gameDetails))
            {
                var playerId = Guid.Parse(confirmGameJoinRequest.PlayerId);
                var player = gameDetails.Players.First(pd => pd.Id.Equals(playerId));
                player.ConnectionId = confirmGameJoinRequest.ConnectionId;

                var playerWithoutConnectionId = gameDetails.Players.FirstOrDefault(pd => pd.ConnectionId == null);
                if (playerWithoutConnectionId == null)
                {
                    if (this.gamesToLaunchById.TryRemove(gameDetails.Id, out var gd))
                    {
                        var gameManagerToken = this.LaunchGame(gameDetails);
                        if (this.inPlayGamesById.TryAdd(gameDetails.Id, gameManagerToken))
                            this.inPlayGames.Add(gameManagerToken);
                    }
                }
            }
        }

        public void PlayerAction(PlayerActionRequest playerActionRequest)
        {
            if (this.inPlayGamesById.TryGetValue(playerActionRequest.GameId, out var gameManagerToken))
            {
                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
                jsonSerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
                jsonSerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
                var playerAction = (PlayerAction)JsonConvert.DeserializeObject(playerActionRequest.Data, jsonSerializerSettings);
                gameManagerToken.GameManager.Post(playerAction);
            }
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        private GameManagerToken LaunchGame(GameSessionDetails gameSessionDetails)
        {
            if (gameSessionDetails == null)
                throw new ArgumentNullException(nameof(gameSessionDetails));

            var playerIds = new Queue<Guid>();

            var connectionIdsByPlayerId = new Dictionary<Guid, string>();
            gameSessionDetails.Players.ForEach(player =>
            {
                connectionIdsByPlayerId.Add(player.Id, player.ConnectionId);
                playerIds.Enqueue(player.Id);
            });

            var gameBoard = new GameBoard(BoardSizes.Standard);
            var gameBoardQuery = new GameBoardQuery(gameBoard);

            Dictionary<Guid, IEventReceiver> eventReceiversByPlayerId = null;
            List<Bot> bots = null;
            if (gameSessionDetails.TotalBotCount > 0)
            {
                bots = new List<Bot>();
                eventReceiversByPlayerId = new Dictionary<Guid, IEventReceiver>();
                while (gameSessionDetails.TotalBotCount-- > 0)
                {
                    var bot = new Bot("Bot #" + (bots.Count + 1), gameSessionDetails.Id, this, gameBoardQuery);
                    bots.Add(bot);
                    eventReceiversByPlayerId.Add(bot.Id, bot);
                    playerIds.Enqueue(bot.Id);
                }
            }

            var eventSender = new EventSender(this.gameHubContext, connectionIdsByPlayerId, eventReceiversByPlayerId);

            var gameManager = new GameManager(
                gameSessionDetails.Id,
                this.numberGenerator,
                gameBoard,
                new DevelopmentCardHolder(),
                new PlayerFactory(),
                eventSender,
                new GameOptions
                {
                    Players = gameSessionDetails.NumberOfPlayers,
                    TurnTimeInSeconds = gameSessionDetails.TurnTimeoutInSeconds
                }
            );

            gameManager.SetIdGenerator(() => { return playerIds.Dequeue(); });

            gameSessionDetails.Players.ForEach(player =>
            {
                gameManager.JoinGame(player.UserName);
            });

            if (bots != null)
            {
                bots.ForEach(bot =>
                {
                    gameManager.JoinGame(bot.Name);
                });
            }

            return new GameManagerToken(gameManager, gameManager.StartGameAsync(), eventSender);
        }

        private void ProcessInPlayGames()
        {
            try
            {
                var clearDownCount = 0;
                while (true)
                {
                    for (var index = 0; index < this.inPlayGames.Count; index++)
                    {
                        var gameManagerToken = this.inPlayGames[index];
                        if (gameManagerToken != null)
                        {
                            var gameManagerTask = gameManagerToken.Task;
                            var clearDownGame = false;
                            if (gameManagerTask.IsCompletedSuccessfully)
                            {
                                clearDownGame = true;
                            }
                            else if (gameManagerTask.IsFaulted)
                            {
                                clearDownGame = true;
                                // TODO: Send disconnected message to all players using the event sender
                            }

                            if (clearDownGame)
                            {
                                var gameManager = gameManagerToken.GameManager;
                                if (this.inPlayGamesById.TryRemove(gameManager.Id, out var gmt))
                                {
                                    this.inPlayGames[index] = null;
                                    clearDownCount++;
                                }
                            }
                        }
                    }

                    if (clearDownCount > 10)
                    {
                        this.inPlayGames = this.inPlayGames.Where(token => token != null).ToList();
                        clearDownCount = 0;
                    }

                    Thread.Sleep(100);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        private class GameManagerToken
        {
            public GameManagerToken(GameManager gameManager, Task task, IEventSender eventSender)
            {
                this.GameManager = gameManager;
                this.Task = task;
                this.EventSender = eventSender;
            }

            public GameManager GameManager { get; private set; }
            public Task Task { get; private set; }
            public IEventSender EventSender { get; private set; }
        }

        private class EventSender : IEventSender
        {
            private readonly IHubContext<GameHub> gameHubContext;
            private readonly Dictionary<Guid, string> connectionIdsByPlayerId;
            private readonly Dictionary<Guid, IEventReceiver> eventReceiversByPlayerId;
            public EventSender(IHubContext<GameHub> gameHubContext,
                Dictionary<Guid, string> connectionIdsByPlayerId,
                Dictionary<Guid, IEventReceiver> eventReceiversByPlayerId)
            {
                this.gameHubContext = gameHubContext;
                this.connectionIdsByPlayerId = connectionIdsByPlayerId;
                this.eventReceiversByPlayerId = eventReceiversByPlayerId;
            }

            public void Send(GameEvent gameEvent, Guid playerId)
            {
                if (this.eventReceiversByPlayerId.ContainsKey(playerId))
                {
                    this.eventReceiversByPlayerId[playerId].Post(gameEvent);
                }
                else if (this.connectionIdsByPlayerId.ContainsKey(playerId))
                {
                    var connectionId = this.connectionIdsByPlayerId[playerId];
                    this.gameHubContext.Clients.Client(connectionId).SendAsync("GameEvent", gameEvent);
                }
                else
                {
                    throw new NotImplementedException("Should not get here");
                }
            }
        }
    }
}

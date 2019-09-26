
namespace SoC.WebApplication
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.GameBoards;
    using Jabberwocky.SoC.Library.GameEvents;
    using Jabberwocky.SoC.Library.Interfaces;
    using Microsoft.AspNetCore.SignalR;
    using SoC.WebApplication.Hubs;
    using SoC.WebApplication.Requests;

    public class GamesOrganizer : IGamesOrganizer
    {
        private readonly IHubContext<SetupHub> setupHubContext;
        private readonly IHubContext<GameHub> gameHubContext;  
        private readonly List<GameDetails> waitingGames = new List<GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameDetails> waitingGamesById = new ConcurrentDictionary<Guid, GameDetails>();
        private readonly ConcurrentQueue<GameDetails> startingGames = new ConcurrentQueue<GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameDetails> startingGamesById = new ConcurrentDictionary<Guid, GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameManagerToken> inPlayGames = new ConcurrentDictionary<Guid, GameManagerToken>();
        private Task startingGameTask;
        private Task mainGameTask;
        private ConcurrentQueue<GameRequest> gameRequests = new ConcurrentQueue<GameRequest>();
        private CancellationToken cancellationToken;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly INumberGenerator numberGenerator = new NumberGenerator();

        public GamesOrganizer(IHubContext<SetupHub> setupHubContext)
        {
            this.setupHubContext = setupHubContext;
            this.cancellationToken = this.cancellationTokenSource.Token;
            this.startingGameTask = Task.Factory.StartNew(o => { this.ProcessStartingGames(); }, this, this.cancellationToken);
            this.mainGameTask = Task.Factory.StartNew(o => { this.ProcessInPlayGames(); }, null, CancellationToken.None);
        }

        private void ProcessStartingGames()
        {
            try
            {
                while (true)
                {
                    this.cancellationToken.ThrowIfCancellationRequested();
                    if (this.startingGames.TryPeek(out var gameDetails))
                    {
                        var duration = DateTime.Now - gameDetails.LaunchTime;
                        if (duration.TotalSeconds > 2)
                        {
                            // Launch game
                            this.startingGames.TryDequeue(out var gd);
                            var gameManagerToken = this.LaunchGame(gameDetails);
                            this.inPlayGames.TryAdd(gameDetails.Id, gameManagerToken);
                            for (var index = 0; index < gameDetails.Players.Count; index++)
                            {
                                var connectionId = gameDetails.Players[index].ConnectionId;
                                var gameLaunchedResponse = new GameLaunchedResponse(gameDetails.Id);
                                this.setupHubContext.Clients.Client(connectionId).SendAsync("GameLaunched", gameLaunchedResponse);
                            }
                        }
                    }

                    Thread.Sleep(500);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        private GameManagerToken LaunchGame(GameDetails gameDetails)
        {
            var connectionIdsByPlayerId = new Dictionary<Guid, string>();
            gameDetails.Players.ForEach(player =>
            {
                connectionIdsByPlayerId.Add(player.Id, player.ConnectionId);
            });
            var eventSender = new EventSender(this.setupHubContext, connectionIdsByPlayerId);

            var gameManager = new GameManager(
                this.numberGenerator,
                new GameBoard(BoardSizes.Standard),
                new DevelopmentCardHolder(),
                new PlayerPool(),
                eventSender,
                new GameOptions
                {
                    Players = gameDetails.NumberOfPlayers,
                    TurnTimeInSeconds = 120
                }
            );

            gameDetails.Players.ForEach(player =>
            {
                gameManager.JoinGame(player.UserName);
            });

            var token = new GameManagerToken
            {
                GameManager = gameManager,
                GameManagerTask = gameManager.StartGameAsync()
            };

            return token;
        }

        private void ProcessInPlayGames()
        {
            try
            {
                while(true)
                {
                    while (this.gameRequests.TryDequeue(out var request))
                    {
                        if (!this.inPlayGames.TryGetValue(request.GameId, out var game))
                        {
                            // Game missing so handle this
                        }
                    }

                    Thread.Sleep(50);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        public CreateGameResponse CreateGame(CreateGameRequest createGameRequest)
        {
            var gameDetails = new GameDetails
            {
                Id = Guid.NewGuid(),
                Name = createGameRequest.Name,
                Owner = createGameRequest.UserName,
                Status = GameStatus.Open,
            };
            gameDetails.Players.Add(new PlayerDetails { ConnectionId = createGameRequest.ConnectionId, UserName = createGameRequest.UserName });
            this.waitingGames.Add(gameDetails);
            this.waitingGamesById.TryAdd(gameDetails.Id, gameDetails);
            return new CreateGameResponse(gameDetails.Id);
        }

        public GameInfoListResponse GetWaitingGames()
        {
            var gameInfoResponses = this.waitingGames
                .Select(gd => new GameInfoResponse
                {
                    Id = gd.Id,
                    Owner = gd.Owner,
                    Name = gd.Name,
                    NumberOfPlayers = gd.NumberOfPlayers,
                    NumberOfSlots = gd.NumberOfSlots,
                    Status = gd.Status
                })
                .ToList();
            return new GameInfoListResponse(gameInfoResponses);
        }

        public JoinGameResponse JoinGame(JoinGameRequest joinGameRequest)
        {
            if (!this.waitingGamesById.ContainsKey(joinGameRequest.GameId))
            {
                return null;
            }

            var gameDetails = this.waitingGamesById[joinGameRequest.GameId];
            if (gameDetails.NumberOfSlots == 0 || gameDetails.Status != GameStatus.Open)
            {
                return new JoinGameResponse(gameDetails.Status);
            }

            gameDetails.Players.Add(new PlayerDetails { ConnectionId = joinGameRequest.ConnectionId, UserName = joinGameRequest.UserName });
            if (gameDetails.NumberOfSlots == 0)
            {
                gameDetails.Status = GameStatus.Starting;
                this.waitingGamesById.TryRemove(gameDetails.Id, out var gd);
                this.waitingGames.Remove(gameDetails);
                this.startingGamesById.TryAdd(gameDetails.Id, gameDetails);
                this.startingGames.Enqueue(gameDetails);
                gameDetails.LaunchTime = DateTime.Now;
            }

            return new JoinGameResponse(gameDetails.Status);
        }

        public void SendEvent(GameEvent gameEvent, Guid playerId)
        {
            if (gameEvent is GameJoinedEvent)
                return;
        }

        public void SendEvent(GameEvent gameEvent, IEnumerable<IPlayer> players)
        {
            throw new NotImplementedException();
        }

        private struct GameManagerToken
        {
            public GameManager GameManager;
            public Task GameManagerTask;
        }

        private class EventSender : IEventSender
        {
            private readonly IHubContext<SetupHub> gameHubContext;
            private readonly Dictionary<Guid, string> connectionIdsByPlayerId;
            public EventSender(IHubContext<SetupHub> gameHubContext, Dictionary<Guid, string> connectionIdsByPlayerId)
            {
                this.gameHubContext = gameHubContext;
                this.connectionIdsByPlayerId = connectionIdsByPlayerId;
            }

            public void Send(GameEvent gameEvent, Guid playerId)
            {
                var connectionId = this.connectionIdsByPlayerId[playerId];
                this.gameHubContext.Clients.Client(connectionId).SendAsync("GameEvent", gameEvent);
            }
        }
    }
}

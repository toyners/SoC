
namespace SoC.SignalR.Testbed
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using SoC.SignalR.Testbed.Hubs;
    using SoC.WebApplication.Requests;

    public class GameManager : IGameManager
    {
        private readonly IHubContext<GameHub> hubContext;
        private readonly List<GameDetails> waitingGames = new List<GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameDetails> waitingGamesById = new ConcurrentDictionary<Guid, GameDetails>();
        private readonly ConcurrentQueue<GameDetails> startingGames = new ConcurrentQueue<GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameDetails> startingGamesById = new ConcurrentDictionary<Guid, GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameDetails> inPlayGames = new ConcurrentDictionary<Guid, GameDetails>();
        private Task startingGameTask;
        private Task mainGameTask;
        private ConcurrentQueue<RequestBase> gameRequests = new ConcurrentQueue<RequestBase>();
        private CancellationToken cancellationToken;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public GameManager(IHubContext<GameHub> hubContext)
        {
            this.hubContext = hubContext;
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
                            this.inPlayGames.TryAdd(gameDetails.Id, gameDetails);
                            for (var index = 0; index < gameDetails.Players.Count; index++)
                            {
                                var connectionId = gameDetails.Players[index].ConnectionId;
                                var gameLaunchedResponse = new GameLaunchedResponse(gameDetails.Id);
                                this.hubContext.Clients.Client(connectionId).SendAsync("GameLaunched", gameLaunchedResponse);
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

        private void ProcessInPlayGames()
        {
            try
            {
                while(true)
                {
                    while (this.gameRequests.TryDequeue(out var request))
                    {
                        //this.inPlayGames[]
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
            gameDetails.Players.Add(new PlayerDetails { ConnectionId = createGameRequest.ConnectionId });
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

            gameDetails.Players.Add(new PlayerDetails { ConnectionId = joinGameRequest.ConnectionId });
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
    }
}

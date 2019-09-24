
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

    public class GamesOrganizer : IGamesOrganizer, IEventSender
    {
        private readonly IHubContext<SetupHub> hubContext;
        private readonly List<GameDetails> waitingGames = new List<GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameDetails> waitingGamesById = new ConcurrentDictionary<Guid, GameDetails>();
        private readonly ConcurrentQueue<GameDetails> startingGames = new ConcurrentQueue<GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameDetails> startingGamesById = new ConcurrentDictionary<Guid, GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameManager> inPlayGames = new ConcurrentDictionary<Guid, GameManager>();
        private Task startingGameTask;
        private Task mainGameTask;
        private ConcurrentQueue<GameRequest> gameRequests = new ConcurrentQueue<GameRequest>();
        private CancellationToken cancellationToken;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly INumberGenerator numberGenerator = new NumberGenerator();

        public bool CanSendEvents { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public GamesOrganizer(IHubContext<SetupHub> hubContext)
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
                            var gameManager = this.LaunchGame(gameDetails);
                            this.inPlayGames.TryAdd(gameDetails.Id, gameManager);
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

        private GameManager LaunchGame(GameDetails gameDetails)
        {
            var gameManager = new GameManager(
                this.numberGenerator, 
                new GameBoard(BoardSizes.Standard),
                new DevelopmentCardHolder(),
                new PlayerPool(),
                this,
                new GameOptions
                {
                     MaxPlayers = 4,
                     MaxAIPlayers = 0,
                     TurnTimeInSeconds = 120
                });
            return gameManager;
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

        public void SendEvent(GameEvent gameEvent, Guid playerId)
        {
            throw new NotImplementedException();
        }

        public void SendEvent(GameEvent gameEvent, IEnumerable<IPlayer> players)
        {
            throw new NotImplementedException();
        }
    }
}


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

    public class GameManager : IGameManager
    {
        private readonly IHubContext<GameHub> hubContext;
        private readonly List<GameDetails> waitingGames = new List<GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameDetails> waitingGamesById = new ConcurrentDictionary<Guid, GameDetails>();
        private readonly ConcurrentQueue<GameDetails> startingGames = new ConcurrentQueue<GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameDetails> startingGamesById = new ConcurrentDictionary<Guid, GameDetails>();
        private readonly ConcurrentDictionary<Guid, GameDetails> inPlayGames = new ConcurrentDictionary<Guid, GameDetails>();
        private Task startingGameTask;
        private CancellationToken cancellationToken;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public GameManager(IHubContext<GameHub> hubContext)
        {
            this.hubContext = hubContext;
            this.cancellationToken = this.cancellationTokenSource.Token;
            this.startingGameTask = Task.Factory.StartNew(o => { this.ProcessStartingGames(); }, this, this.cancellationToken);
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

                    }

                    Thread.Sleep(500);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }

        public CreateGameResponse CreateGame(CreateGameRequest createGameRequest)
        {
            var gameInfo = new GameDetails
            {
                Id = Guid.NewGuid(),
                Name = createGameRequest.Name,
                Owner = createGameRequest.UserName,
                Status = GameStatus.Open,
            };
            gameInfo.Players.Add(createGameRequest.ConnectionId);
            this.waitingGames.Add(gameInfo);
            this.waitingGamesById.TryAdd(gameInfo.Id, gameInfo);
            return new CreateGameResponse(gameInfo.Id);
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

        public GameStatus? JoinGame(JoinGameRequest joinGameRequest)
        {
            if (!this.waitingGamesById.ContainsKey(joinGameRequest.GameId))
            {
                return null;
            }

            var gameDetails = this.waitingGamesById[joinGameRequest.GameId];
            if (gameDetails.NumberOfSlots == 0 || gameDetails.Status != GameStatus.Open)
            {
                return gameDetails.Status;
            }

            gameDetails.Players.Add(joinGameRequest.ConnectionId);
            if (gameDetails.NumberOfSlots == 0)
            {
                gameDetails.Status = GameStatus.Starting;
                this.waitingGamesById.TryRemove(gameDetails.Id, out var gd);
                this.startingGamesById.TryAdd(gameDetails.Id, gameDetails);
                this.startingGames.Enqueue(gameDetails);
            }

            return gameDetails.Status;
        }

        public void SendRequest(Response response)
        {

        }
    }

    public class GameDetails
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Owner { get; set; }
        public GameStatus Status { get; set; }
        public int NumberOfPlayers { get { return this.Players.Count; } }
        public int NumberOfSlots { get { return 4 - this.NumberOfPlayers; } }
        public List<string> Players { get; set; } = new List<string>();
    }
}

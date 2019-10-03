
namespace SoC.WebApplication
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.SignalR;
    using SoC.WebApplication.Hubs;
    using SoC.WebApplication.Requests;

    public class GameSessionsOrganizer : IGameSessionsOrganizer
    {
        private readonly IHubContext<GameSessionHub> gameSessionHubContext;
        private readonly IGamesAdministrator gamesAdministrator;
        private readonly ConcurrentDictionary<Guid, GameSessionDetails> waitingGameSessionsById = new ConcurrentDictionary<Guid, GameSessionDetails>();
        private readonly ConcurrentQueue<GameSessionDetails> startingGames = new ConcurrentQueue<GameSessionDetails>();
        private readonly ConcurrentDictionary<Guid, GameSessionDetails> startingGamesById = new ConcurrentDictionary<Guid, GameSessionDetails>();
        private readonly Task startingGameTask;
        private readonly CancellationToken cancellationToken;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public GameSessionsOrganizer(IHubContext<GameSessionHub> gameSessionHubContext, IGamesAdministrator gamesAdministrator)
        {
            this.gameSessionHubContext = gameSessionHubContext;
            this.gamesAdministrator = gamesAdministrator;
            this.cancellationToken = this.cancellationTokenSource.Token;
            this.startingGameTask = Task.Factory.StartNew(o => { this.ProcessStartingGames(); }, this, this.cancellationToken);
        }

        public ResponseBase CreateGameSession(CreateGameSessionRequest createGameRequest)
        {
            var gameDetails = new GameSessionDetails
            {
                Name = createGameRequest.Name,
                Owner = createGameRequest.UserName,
                TotalPlayerCount = createGameRequest.MaxPlayers,
                TotalBotCount = createGameRequest.MaxBots,
            };
            gameDetails.Players.Add(new PlayerDetails { ConnectionId = createGameRequest.ConnectionId, UserName = createGameRequest.UserName });

            if (createGameRequest.MaxPlayers == 1)
            {
                gameDetails.Status = GameStatus.Starting;
                this.gamesAdministrator.AddGame(gameDetails);
                
                var playerDetails = gameDetails.Players[0];
                return new LaunchGameResponse(gameDetails.Status, gameDetails.Id, playerDetails.Id, playerDetails.ConnectionId);
            }
            else
            {
                gameDetails.Status = GameStatus.Open;
                this.waitingGameSessionsById.TryAdd(gameDetails.Id, gameDetails);
                return new CreateGameSessionResponse(gameDetails.Id);
            }
        }

        public GameInfoListResponse GetWaitingGameSessions()
        {
            var gameInfoResponses = this.waitingGameSessionsById.Values
                .Where(gd => gd.Status == GameStatus.Open)
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

        public bool? JoinGameSession(JoinGameSessionRequest joinGameRequest, out JoinGameResponse[] responses)
        {
            responses = null;
            if (!this.waitingGameSessionsById.ContainsKey(joinGameRequest.GameId))
            {
                return null;
            }

            var gameDetails = this.waitingGameSessionsById[joinGameRequest.GameId];
            if (gameDetails.NumberOfSlots == 0 || gameDetails.Status != GameStatus.Open)
            {
                responses = new[] { new JoinGameResponse(gameDetails.Status) };
                return false;
            }

            gameDetails.Players.Add(new PlayerDetails { ConnectionId = joinGameRequest.ConnectionId, UserName = joinGameRequest.UserName });
            if (gameDetails.NumberOfSlots == 0)
            {
                gameDetails.Status = GameStatus.Starting;
                this.waitingGameSessionsById.TryRemove(gameDetails.Id, out var gd);
                this.startingGamesById.TryAdd(gameDetails.Id, gameDetails);
                this.startingGames.Enqueue(gameDetails);
                gameDetails.LaunchTime = DateTime.Now;
            }

            responses = new LaunchGameResponse[gameDetails.NumberOfPlayers];
            for (var index = 0; index < gameDetails.Players.Count; index++)
            {
                var playerDetails = gameDetails.Players[index];
                responses[index] = new LaunchGameResponse(
                    gameDetails.Status, gameDetails.Id, playerDetails.Id, playerDetails.ConnectionId);
            }

            return true;
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
                            this.gamesAdministrator.AddGame(gameDetails);
                            /*var gameManagerToken = this.LaunchGame(gameDetails);
                            this.inPlayGames.TryAdd(gameDetails.Id, gameManagerToken);
                            for (var index = 0; index < gameDetails.Players.Count; index++)
                            {
                                var playerDetails = gameDetails.Players[index];
                                var gameLaunchedResponse = new GameLaunchedResponse(gameDetails.Id, playerDetails.Id);
                                this.setupHubContext.Clients.Client(playerDetails.ConnectionId).SendAsync("GameLaunched", gameLaunchedResponse);
                            }*/
                        }
                    }

                    Thread.Sleep(500);
                }
            }
            catch (OperationCanceledException)
            {

            }
        }
    }
}

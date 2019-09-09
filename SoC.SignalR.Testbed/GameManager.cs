
namespace SoC.SignalR.Testbed
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.SignalR;
    using SoC.SignalR.Testbed.Hubs;

    public class GameManager : IGameManager
    {
        private readonly IHubContext<GameHub> hubContext;
        private readonly List<GameDetails> games = new List<GameDetails>();
        private readonly Dictionary<Guid, GameDetails> gamesById = new Dictionary<Guid, GameDetails>();

        public GameManager(IHubContext<GameHub> hubContext) => this.hubContext = hubContext;

        public CreateGameResponse CreateGame(CreateGameRequest createGameRequest)
        {
            var gameInfo = new GameDetails
            {
                Id = Guid.NewGuid(),
                Name = createGameRequest.Name,
                Owner = createGameRequest.UserName,
                Status = GameStatus.Open,
                NumberOfPlayers = 1,
                NumberOfSlots = 3
            };
            this.games.Add(gameInfo);
            this.gamesById.Add(gameInfo.Id, gameInfo);
            return new CreateGameResponse(gameInfo.Id);
        }

        public JoinGameResponse JoinGame(JoinGameRequest joinGameRequest)
        {
            if (!this.gamesById.ContainsKey(joinGameRequest.GameId))
            {
                return null;
            }

            var gameDetails = this.gamesById[joinGameRequest.GameId];
            if (gameDetails.NumberOfSlots == 0 || gameDetails.Status != GameStatus.Open)
            {
                return new JoinGameResponse(gameDetails.Status);
            }

            gameDetails.NumberOfPlayers++;
            gameDetails.NumberOfSlots--;
            if (gameDetails.NumberOfSlots == 0)
            {
                gameDetails.Status = GameStatus.Starting;
            }

            return new JoinGameResponse(gameDetails.Status);
        }

        public Response ProcessRequest(Request request)
        {
            if  (request is GetWaitingGamesRequest)
            {
                var gameInfoResponses = this.games
                    .Select(gd => new GameInfoResponse {
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

            return null;
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
        public int NumberOfPlayers { get; set; }
        public int NumberOfSlots { get; set; }
    }
}

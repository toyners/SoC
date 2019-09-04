
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

        public GameManager(IHubContext<GameHub> hubContext) => this.hubContext = hubContext;

        public void CreateGame(CreateGameRequest createGameRequest) { }

        public Response ProcessRequest(Request request)
        {
            if  (request is GetWaitingGamesRequest)
            {
                var gameInfoResponses = this.games
                    .Select(gd => new GameInfoResponse {
                        Id = gd.Id,
                        InitiatingPlayer = gd.InitiatingPlayer,
                        Name = gd.Name,
                        NumberOfPlayers = gd.NumberOfPlayers,
                        NumberOfSlots = gd.NumberOfSlots,
                        Status = gd.Status
                    })
                    .ToList();
                return new GameInfoListResponse(gameInfoResponses);
            }

            if (request is CreateGameRequest createGameRequest)
            {
                var gameInfo = new GameDetails
                {
                    Id = Guid.NewGuid(),
                    Name = createGameRequest.Name,
                    Status = GameStatus.Open,
                    NumberOfPlayers = 1,
                    NumberOfSlots = 3
                };
                this.games.Add(gameInfo);
                return new CreateGameResponse(gameInfo.Id);
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
        public string InitiatingPlayer { get; set; }
        public GameStatus Status { get; set; }
        public int NumberOfPlayers { get; set; }
        public int NumberOfSlots { get; set; }
    }
}

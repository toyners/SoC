
namespace SoC.SignalR.Testbed
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.SignalR;
    using SoC.SignalR.Testbed.Hubs;

    public class GameManager : IGameManager
    {
        private readonly IHubContext<GameHub> hubContext;
        private readonly List<GameInfo> games = new List<GameInfo>();

        public GameManager(IHubContext<GameHub> hubContext) => this.hubContext = hubContext;

        public Response ProcessRequest(Request request)
        {
            if  (request is GetWaitingGamesRequest)
            {
                return new GameInfoListResponse(this.games);
            }

            if (request is CreateGameRequest createGameRequest)
            {
                var gameInfo = new GameInfo
                {
                    Id = Guid.NewGuid(),
                    Name = createGameRequest.Name,
                    Status = GameInfo.GameStatus.Open,
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
}


namespace SoC.SignalR.Testbed
{
    using System;
    using Microsoft.AspNetCore.SignalR;
    using SoC.SignalR.Testbed.Hubs;

    public class GameManager : IGameManager
    {
        private readonly IHubContext<GameHub> hubContext;

        public GameManager(IHubContext<GameHub> hubContext) => this.hubContext = hubContext;

        public Response ProcessRequest(Request request)
        {
            if  (request is GetWaitingGamesRequest)
            {
                return new GameInfoListResponse(null);
            }

            if (request is CreateGameRequest createGameRequest)
            {
                return new CreateGameResponse(Guid.NewGuid());
            }

            return null;
        }

        public void SendRequest(Response response)
        {

        }
    }
}

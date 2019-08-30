
namespace SoC.SignalR.Testbed
{
    using Microsoft.AspNetCore.SignalR;
    using SoC.SignalR.Testbed.Hubs;

    public class GameManager : IGameManager
    {
        private readonly IHubContext<GameHub> hubContext;

        public GameManager(IHubContext<GameHub> hubContext) => this.hubContext = hubContext;

        public void ProcessRequest(Request request)
        {

        }

        public void SendRequest(Response response)
        {

        }
    }
}

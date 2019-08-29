
using Microsoft.AspNetCore.SignalR;
using SoC.SignalR.Testbed.Hubs;

namespace SoC.SignalR.Testbed
{
    public interface IGameManager
    {
        void ProcessRequest(Request request);
        void SendRequest(Response response);
    }

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

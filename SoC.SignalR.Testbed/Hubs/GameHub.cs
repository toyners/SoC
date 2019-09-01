using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SoC.SignalR.Testbed.Hubs
{
    public class GameHub : Hub
    {
        private IGameManager gameManager;

        public GameHub(IGameManager gameManager) => this.gameManager = gameManager;

        public void PostRequest(JoinGameRequest joinRequest)
        {
            this.gameManager.ProcessRequest(joinRequest);
        }

        public void GetWaitingGamesRequest(GetWaitingGamesRequest getWaitingGamesRequest)
        {
            this.gameManager.ProcessRequest(getWaitingGamesRequest);
        }
    }
}

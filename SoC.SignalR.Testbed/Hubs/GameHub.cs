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
            var response = this.gameManager.ProcessRequest(joinRequest);
            //await this.Clients.Caller.SendAsync("ReceiveResponse", response);
        }

        public async void GetWaitingGamesRequest(GetWaitingGamesRequest getWaitingGamesRequest)
        {
            var response = this.gameManager.ProcessRequest(getWaitingGamesRequest);
            await this.Clients.Caller.SendAsync("GameListResponse", response);
        }
    }
}

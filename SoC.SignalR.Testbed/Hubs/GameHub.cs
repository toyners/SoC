using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SoC.SignalR.Testbed.Hubs
{
    public class GameHub : Hub
    {
        private IGameManager gameManager;

        public GameHub(IGameManager gameManager) => this.gameManager = gameManager;

        public async void GetWaitingGamesRequest(GetWaitingGamesRequest getWaitingGamesRequest)
        {
            var response = this.gameManager.ProcessRequest(getWaitingGamesRequest);
            await this.Clients.Caller.SendAsync("GameListResponse", response);
        }

        public async void CreateGame(CreateGameRequest createGameRequest)
        {
            createGameRequest.ClientProxy = this.Clients.Caller;
            var createGameResponse = this.gameManager.CreateGame(createGameRequest);
            await this.Clients.Caller.SendAsync("CreateGameResponse", createGameResponse);

            var gameListResponse = this.gameManager.ProcessRequest(new GetWaitingGamesRequest());
            await this.Clients.All.SendAsync("GameListResponse", gameListResponse);
        }

        public async void JoinGame(JoinGameRequest joinGameRequest)
        {
            joinGameRequest.ClientProxy = this.Clients.Caller;
            var joinGameResponse = this.gameManager.JoinGame(joinGameRequest);
            await this.Clients.Caller.SendAsync("JoinGameResponse", joinGameResponse);

            var gameListResponse = this.gameManager.ProcessRequest(new GetWaitingGamesRequest());
            await this.Clients.All.SendAsync("GameListResponse", gameListResponse);
        }
    }
}

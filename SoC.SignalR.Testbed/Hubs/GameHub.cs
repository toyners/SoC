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
            createGameRequest.ConnectionId = this.Context.ConnectionId;
            var createGameResponse = this.gameManager.CreateGame(createGameRequest);
            await this.Clients.Caller.SendAsync("CreateGameResponse", createGameResponse);

            var gameListResponse = this.gameManager.ProcessRequest(new GetWaitingGamesRequest());
            await this.Clients.All.SendAsync("GameListResponse", gameListResponse);
        }

        public async void JoinGame(JoinGameRequest joinGameRequest)
        {
            joinGameRequest.ConnectionId = this.Context.ConnectionId;
            var gameStatus = await this.gameManager.JoinGame(joinGameRequest);
            if (gameStatus == null)
            {
                // Handle bad game id
            }
            else
            {
                var joinGameResponse = new JoinGameResponse(gameStatus.Value);
                await this.Clients.Caller.SendAsync("JoinGameResponse", joinGameResponse);

                var gameListResponse = this.gameManager.ProcessRequest(new GetWaitingGamesRequest());
                await this.Clients.All.SendAsync("GameListResponse", gameListResponse);
            }
        }
    }
}

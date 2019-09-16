using Microsoft.AspNetCore.SignalR;
using SoC.WebApplication.Requests;

namespace SoC.WebApplication.Hubs
{
    public class GameHub : Hub
    {
        private IGameManager gameManager;

        public GameHub(IGameManager gameManager) => this.gameManager = gameManager;

        public async void CreateGame(CreateGameRequest createGameRequest)
        {
            createGameRequest.ConnectionId = this.Context.ConnectionId;
            var createGameResponse = this.gameManager.CreateGame(createGameRequest);
            await this.Clients.Caller.SendAsync("GameCreated", createGameResponse);

            var gameListResponse = this.gameManager.GetWaitingGames();
            await this.Clients.All.SendAsync("GameList", gameListResponse);
        }

        public async void GetWaitingGames(GetWaitingGamesRequest getWaitingGamesRequest)
        {
            var response = this.gameManager.GetWaitingGames();
            await this.Clients.Caller.SendAsync("GameList", response);
        }

        public async void JoinGame(JoinGameRequest joinGameRequest)
        {
            joinGameRequest.ConnectionId = this.Context.ConnectionId;
            var joinGameResponse = this.gameManager.JoinGame(joinGameRequest);
            if (joinGameResponse == null)
            {
                // Handle bad game id
            }
            else
            {
                await this.Clients.Caller.SendAsync("GameJoined", joinGameResponse);

                var gameListResponse = this.gameManager.GetWaitingGames();
                await this.Clients.All.SendAsync("GameList", gameListResponse);
            }
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using SoC.WebApplication.Requests;

namespace SoC.WebApplication.Hubs
{
    public class SetupHub : Hub
    {
        private IGamesOrganizer gamesOrganizer;

        public SetupHub(IGamesOrganizer gamesOrganizer) => this.gamesOrganizer = gamesOrganizer;

        public async void CreateGame(CreateGameRequest createGameRequest)
        {
            createGameRequest.ConnectionId = this.Context.ConnectionId;
            var createGameResponse = this.gamesOrganizer.CreateGame(createGameRequest);
            await this.Clients.Caller.SendAsync("GameCreated", createGameResponse);

            var gameListResponse = this.gamesOrganizer.GetWaitingGames();
            await this.Clients.All.SendAsync("GameList", gameListResponse);
        }

        public async void GetWaitingGames(GetWaitingGamesRequest getWaitingGamesRequest)
        {
            var response = this.gamesOrganizer.GetWaitingGames();
            await this.Clients.Caller.SendAsync("GameList", response);
        }

        public async void JoinGame(JoinGameRequest joinGameRequest)
        {
            joinGameRequest.ConnectionId = this.Context.ConnectionId;
            var joinGameResponse = this.gamesOrganizer.JoinGame(joinGameRequest);
            if (joinGameResponse == null)
            {
                // Handle bad game id
            }
            else
            {
                await this.Clients.Caller.SendAsync("GameJoined", joinGameResponse);

                var gameListResponse = this.gamesOrganizer.GetWaitingGames();
                await this.Clients.All.SendAsync("GameList", gameListResponse);
            }
        }
    }
}

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
            var response = this.gamesOrganizer.CreateGame(createGameRequest);

            var createGameResponse = response as CreateGameResponse;
            if (createGameResponse != null)
            {
                await this.Clients.Caller.SendAsync("GameCreated", (CreateGameResponse)response);

                var gameListResponse = this.gamesOrganizer.GetWaitingGames();
                await this.Clients.All.SendAsync("GameList", gameListResponse);
                return;
            }

            var launchGameResponse = response as LaunchGameResponse;
            if (launchGameResponse != null)
            {
                await this.Clients.Caller.SendAsync("GameJoined", launchGameResponse);
                return;
            }

            throw new System.Exception("Unexpected response");
        }

        public async void GetWaitingGames(GetWaitingGamesRequest getWaitingGamesRequest)
        {
            var response = this.gamesOrganizer.GetWaitingGames();
            await this.Clients.Caller.SendAsync("GameList", response);
        }

        public async void JoinGame(JoinGameRequest joinGameRequest)
        {
            joinGameRequest.ConnectionId = this.Context.ConnectionId;
            var result = this.gamesOrganizer.JoinGame(joinGameRequest, out var joinGameResponses);
            if (result == null)
            {
                // Handle bad game id
            }
            else
            {
                if (!result.Value)
                {
                    await this.Clients.Caller.SendAsync("GameJoined", joinGameResponses[0]);
                }
                else
                {
                    foreach (var joinGameResponse in joinGameResponses)
                    {
                        var launchGameResponse = (LaunchGameResponse)joinGameResponse;
                        await this.Clients
                            .Client(launchGameResponse.ConnectionId)
                            .SendAsync("GameJoined", launchGameResponse);
                    }
                }

                var gameListResponse = this.gamesOrganizer.GetWaitingGames();
                await this.Clients.All.SendAsync("GameList", gameListResponse);
            }
        }
    }
}

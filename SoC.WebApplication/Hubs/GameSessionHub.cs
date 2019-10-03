using Microsoft.AspNetCore.SignalR;
using SoC.WebApplication.Requests;

namespace SoC.WebApplication.Hubs
{
    public class GameSessionHub : Hub
    {
        private IGameSessionsOrganizer gamesOrganizer;

        public GameSessionHub(IGameSessionsOrganizer gamesOrganizer) => this.gamesOrganizer = gamesOrganizer;

        public async void CreateGameSession(CreateGameSessionRequest createGameSessionRequest)
        {
            createGameSessionRequest.ConnectionId = this.Context.ConnectionId;
            var response = this.gamesOrganizer.CreateGameSession(createGameSessionRequest);

            var createGameSessionResponse = response as CreateGameSessionResponse;
            if (createGameSessionResponse != null)
            {
                await this.Clients.Caller.SendAsync("GameSessionCreated", createGameSessionResponse);

                var gameListResponse = this.gamesOrganizer.GetWaitingGameSessions();
                await this.Clients.All.SendAsync("GameSessionList", gameListResponse);
                return;
            }

            var launchGameResponse = response as LaunchGameResponse;
            if (launchGameResponse != null)
            {
                await this.Clients.Caller.SendAsync("GameSessionJoined", launchGameResponse);
                return;
            }

            throw new System.Exception("Unexpected response");
        }

        public async void GetWaitingGameSessions(GetWaitingGameSessionsRequest getWaitingGamesRequest)
        {
            var response = this.gamesOrganizer.GetWaitingGameSessions();
            await this.Clients.Caller.SendAsync("GameSessionList", response);
        }

        public async void JoinGameSession(JoinGameSessionRequest joinGameSessionRequest)
        {
            joinGameSessionRequest.ConnectionId = this.Context.ConnectionId;
            var result = this.gamesOrganizer.JoinGameSession(joinGameSessionRequest, out var joinGameResponses);
            if (result == null)
            {
                // Handle bad game id
            }
            else
            {
                if (!result.Value)
                {
                    await this.Clients.Caller.SendAsync("GameSessionJoined", joinGameResponses[0]);
                }
                else
                {
                    foreach (var joinGameResponse in joinGameResponses)
                    {
                        var launchGameResponse = (LaunchGameResponse)joinGameResponse;
                        await this.Clients
                            .Client(launchGameResponse.ConnectionId)
                            .SendAsync("GameSessionJoined", launchGameResponse);
                    }
                }

                var gameListResponse = this.gamesOrganizer.GetWaitingGameSessions();
                await this.Clients.All.SendAsync("GameSessionList", gameListResponse);
            }
        }
    }
}

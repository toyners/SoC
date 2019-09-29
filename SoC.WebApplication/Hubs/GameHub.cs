using Microsoft.AspNetCore.SignalR;
using SoC.WebApplication.Requests;

namespace SoC.WebApplication.Hubs
{
    public class GameHub : Hub
    {
        private IGamesOrganizer gameOrganizer;

        public GameHub(IGamesOrganizer gameOrganizer) => this.gameOrganizer = gameOrganizer;

        public async void ConfirmGameJoin(ConfirmGameJoinRequest confirmGameJoinRequest)
        {
            confirmGameJoinRequest.ConnectionId = this.Context.ConnectionId;
            this.gameOrganizer.ConfirmGameJoin(confirmGameJoinRequest);
        }
    }
}

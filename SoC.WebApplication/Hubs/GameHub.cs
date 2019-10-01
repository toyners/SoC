using Microsoft.AspNetCore.SignalR;
using SoC.WebApplication.Requests;

namespace SoC.WebApplication.Hubs
{
    public class GameHub : Hub
    {
        private IGamesAdministrator gamesAdministrator;

        public GameHub(IGamesAdministrator gamesAdministrator) => this.gamesAdministrator = gamesAdministrator;

        public async void ConfirmGameJoin(ConfirmGameJoinRequest confirmGameJoinRequest)
        {
            confirmGameJoinRequest.ConnectionId = this.Context.ConnectionId;
            this.gamesAdministrator.ConfirmGameJoin(confirmGameJoinRequest);
        }
    }
}

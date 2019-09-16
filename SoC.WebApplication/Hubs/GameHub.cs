using Microsoft.AspNetCore.SignalR;

namespace SoC.WebApplication.Hubs
{
    public class GameHub : Hub
    {
        private IGamesOrganizer gameManager;

        public GameHub(IGamesOrganizer gameManager) => this.gameManager = gameManager;
    }
}

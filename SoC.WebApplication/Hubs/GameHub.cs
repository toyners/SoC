using Microsoft.AspNetCore.SignalR;

namespace SoC.WebApplication.Hubs
{
    public class GameHub : Hub
    {
        private IGameManager gameManager;

        public GameHub(IGameManager gameManager) => this.gameManager = gameManager;
    }
}

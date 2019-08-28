using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SoC.Server.Testbed.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessageAsync(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}

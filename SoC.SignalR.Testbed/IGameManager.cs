
using Microsoft.AspNetCore.SignalR;
using SoC.SignalR.Testbed.Hubs;

namespace SoC.SignalR.Testbed
{
    public interface IGameManager
    {
        void ProcessRequest(Request request);
        void SendRequest(Response response);
    }
}

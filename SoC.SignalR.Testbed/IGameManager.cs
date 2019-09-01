
namespace SoC.SignalR.Testbed
{
    public interface IGameManager
    {
        void ProcessRequest(Request request);
        void SendRequest(Response response);
    }
}

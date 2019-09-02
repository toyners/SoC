
namespace SoC.SignalR.Testbed
{
    public interface IGameManager
    {
        Response ProcessRequest(Request request);
        void SendRequest(Response response);
    }
}



namespace SoC.SignalR.Testbed
{
    public interface IGameManager
    {
        void CreateGame(CreateGameRequest createGameRequest);
        Response ProcessRequest(Request request);
        void SendRequest(Response response);
    }
}



namespace SoC.SignalR.Testbed
{
    public interface IGameManager
    {
        CreateGameResponse CreateGame(CreateGameRequest createGameRequest);
        JoinGameResponse JoinGame(JoinGameRequest joinGameRequest);
        Response ProcessRequest(Request request);
        void SendRequest(Response response);
    }
}


namespace SoC.SignalR.Testbed
{
    public interface IGameManager
    {
        CreateGameResponse CreateGame(CreateGameRequest createGameRequest);
        GameInfoListResponse GetWaitingGames();
        JoinGameResponse JoinGame(JoinGameRequest joinGameRequest);
        //void SendRequest(Response response);
    }
}


namespace SoC.SignalR.Testbed
{
    public interface IGameManager
    {
        CreateGameResponse CreateGame(CreateGameRequest createGameRequest);
        GameInfoListResponse GetWaitingGames();
        GameStatus? JoinGame(JoinGameRequest joinGameRequest);
        void SendRequest(Response response);
    }
}

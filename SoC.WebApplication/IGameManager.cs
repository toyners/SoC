
using SoC.WebApplication.Requests;

namespace SoC.SignalR.Testbed
{
    public interface IGameManager
    {
        CreateGameResponse CreateGame(CreateGameRequest createGameRequest);
        GameInfoListResponse GetWaitingGames();
        JoinGameResponse JoinGame(JoinGameRequest joinGameRequest);
    }
}

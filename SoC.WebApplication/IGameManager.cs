
using SoC.WebApplication.Requests;

namespace SoC.WebApplication
{
    public interface IGameManager
    {
        CreateGameResponse CreateGame(CreateGameRequest createGameRequest);
        GameInfoListResponse GetWaitingGames();
        JoinGameResponse JoinGame(JoinGameRequest joinGameRequest);
    }
}

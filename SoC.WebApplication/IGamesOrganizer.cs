
using SoC.WebApplication.Requests;

namespace SoC.WebApplication
{
    public interface IGamesOrganizer
    {
        CreateGameResponse CreateGame(CreateGameRequest createGameRequest);
        GameInfoListResponse GetWaitingGames();
        JoinGameResponse JoinGame(JoinGameRequest joinGameRequest);
    }
}

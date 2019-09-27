
using SoC.WebApplication.Requests;

namespace SoC.WebApplication
{
    public interface IGamesOrganizer
    {
        CreateGameResponse CreateGame(CreateGameRequest createGameRequest);
        GameInfoListResponse GetWaitingGames();
        bool? JoinGame(JoinGameRequest joinGameRequest, out JoinGameResponse[] responses);
    }
}

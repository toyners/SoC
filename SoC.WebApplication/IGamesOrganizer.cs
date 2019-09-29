
using SoC.WebApplication.Requests;

namespace SoC.WebApplication
{
    public interface IGamesOrganizer
    {
        ResponseBase CreateGame(CreateGameRequest createGameRequest);
        GameInfoListResponse GetWaitingGames();
        bool? JoinGame(JoinGameRequest joinGameRequest, out JoinGameResponse[] responses);
        void ConfirmGameJoin(ConfirmGameJoinRequest confirmGameJoinRequest);
    }
}

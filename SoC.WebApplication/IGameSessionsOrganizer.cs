
using SoC.WebApplication.Requests;

namespace SoC.WebApplication
{
    public interface IGameSessionsOrganizer
    {
        ResponseBase CreateGameSession(CreateGameSessionRequest createGameRequest);
        GameInfoListResponse GetWaitingGameSessions();
        bool? JoinGameSession(JoinGameSessionRequest joinGameRequest, out JoinGameResponse[] responses);
    }
}

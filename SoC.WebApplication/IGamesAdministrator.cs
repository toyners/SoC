
namespace SoC.WebApplication
{
    using SoC.WebApplication.Requests;

    public interface IGamesAdministrator
    {
        void AddGame(GameDetails gameDetails);
        void ConfirmGameJoin(ConfirmGameJoinRequest confirmGameJoinRequest);
        void Shutdown();
    }
}


namespace SoC.WebApplication
{
    using SoC.WebApplication.Requests;

    public interface IGamesAdministrator : IPlayerRequestReceiver
    {
        void AddGame(GameDetails gameDetails);
        void ConfirmGameJoin(ConfirmGameJoinRequest confirmGameJoinRequest);
        void Shutdown();
    }

    public interface IPlayerRequestReceiver
    {
        void PlayerAction(PlayerActionRequest playerActionRequest);
    }
}

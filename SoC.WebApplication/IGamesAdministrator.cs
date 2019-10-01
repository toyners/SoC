
namespace SoC.WebApplication
{
    public interface IGamesAdministrator
    {
        void AddGame(GameDetails gameDetails);
        void Shutdown();
    }
}

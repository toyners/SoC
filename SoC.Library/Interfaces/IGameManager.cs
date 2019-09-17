
namespace Jabberwocky.SoC.Library.Interfaces
{
    using System.Threading.Tasks;

    public interface IGameManager
    {
        void LaunchGame(GameOptions gameOptions = null);

        void Quit();

        Task StartGameAsync();
    }
}

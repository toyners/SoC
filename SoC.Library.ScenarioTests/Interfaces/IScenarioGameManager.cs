
namespace SoC.Library.ScenarioTests.Interfaces
{
    using Jabberwocky.SoC.Library;
    using Jabberwocky.SoC.Library.Interfaces;

    public interface IScenarioGameManager : IGameManager
    {
        void AddResourcesToPlayer(string playerName, ResourceClutch value);
    }
}

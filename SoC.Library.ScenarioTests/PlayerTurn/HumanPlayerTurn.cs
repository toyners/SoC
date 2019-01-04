using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class HumanPlayerTurn : BasePlayerTurn
    {
        public HumanPlayerTurn(LocalGameControllerScenarioRunner runner, IPlayer player) : base(runner, player)
        {
        }
    }
}

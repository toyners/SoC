using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class HumanPlayerTurn : PlayerTurn
    {
        public HumanPlayerTurn(LocalGameControllerScenarioRunner runner, IPlayer player) : base(runner, player)
        {
        }
    }
}

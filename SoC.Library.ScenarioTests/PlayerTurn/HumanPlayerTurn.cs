using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class HumanPlayerTurn : BasePlayerTurn
    {
        public HumanPlayerTurn(IPlayer player, LocalGameControllerScenarioRunner runner, int roundNumber, int turnNumber) : base(player, runner)
        {
        }
    }
}

using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class HumanPlayerTurn : BasePlayerTurn
    {
        public HumanPlayerTurn(IPlayer player, uint dice1, uint dice2, LocalGameControllerScenarioRunner runner) : base(player, dice1, dice2, runner)
        {
        }
    }
}

using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests
{
    internal class ComputerPlayerTurn : PlayerTurn
    {
        private readonly ScenarioComputerPlayer computerPlayer;

        public ComputerPlayerTurn(LocalGameControllerScenarioRunner runner, IPlayer player) : base(runner, player)
        {
            this.computerPlayer = (ScenarioComputerPlayer)player;
        }

        public void ResolveActions()
        {
            this.computerPlayer.AddActions(this.actions.ToArray());
        }
    }
}

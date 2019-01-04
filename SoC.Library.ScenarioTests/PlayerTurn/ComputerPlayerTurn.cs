using Jabberwocky.SoC.Library;
using Jabberwocky.SoC.Library.Interfaces;

namespace SoC.Library.ScenarioTests.PlayerTurn
{
    internal class ComputerPlayerTurn : BasePlayerTurn
    {
        private readonly ScenarioComputerPlayer computerPlayer;

        public ComputerPlayerTurn(LocalGameControllerScenarioRunner runner, IPlayer player) : base(runner, player)
        {
            this.computerPlayer = (ScenarioComputerPlayer)player;
        }

        public override void ResolveActions(TurnToken turnToken, LocalGameController localGameController)
        {
            this.computerPlayer.AddActions(this.actions.ToArray());
        }
    }
}

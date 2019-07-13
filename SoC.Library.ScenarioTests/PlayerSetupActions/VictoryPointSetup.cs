
namespace SoC.Library.ScenarioTests.PlayerSetupActions
{
    internal class VictoryPointSetup : IPlayerSetupAction
    {
        private uint victoryPoints;
        public VictoryPointSetup(uint victoryPoints) => this.victoryPoints = victoryPoints;
        public void Process(ScenarioPlayer player) => player.SetVictoryPoints(this.victoryPoints);
    }
}
